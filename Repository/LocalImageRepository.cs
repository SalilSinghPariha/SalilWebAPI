using NZWalks.data;
using NZWalks.Models.Domain;

namespace NZWalks.Repository
{
    public class LocalImageRepository : IImageRepository
    {
        //In order to acces application folder specific folder, we need to inject IWebHostEnvironment which will locate to path

        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NZWalksDbContext _nZWalksDbContext;
        public LocalImageRepository(IWebHostEnvironment environment ,
            IHttpContextAccessor httpContextAccessor,NZWalksDbContext nZWalksDbContext)
        {

            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _nZWalksDbContext = nZWalksDbContext;
        }


        public async Task<Image> Upload(Image image)
        {
            var localFilepath = Path.Combine(_environment.ContentRootPath, "Images",
                $"{image.FileName}{image.FileExtension}");

            //upload image to local path

            var stream = new FileStream(localFilepath, FileMode.Create);

            await image.File.CopyToAsync(stream);

            //Save these image like //https/localhost:1234//image/image.jpg
            // In order to set this we need to provide or add httpcontent to program.cs file

            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;

            //save changes to database

            await _nZWalksDbContext.images.AddAsync(image);

            await _nZWalksDbContext.SaveChangesAsync();

            return image;

        }
    }
}
