using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Repository;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        //Inject Repository to access the repository

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("Upload")]

        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto uploadRequestDto)
        {

            ValidateFileUpload(uploadRequestDto);

            if (ModelState.IsValid)
            {
                //Convert dto to domain model

                var imageDomainModel = new Image
                {
                    File=uploadRequestDto.File,
                    FileExtension=Path.GetExtension(uploadRequestDto.File.FileName),
                    FileSizeIbByte = uploadRequestDto.File.Length,
                    FileDescription=uploadRequestDto.FileDescription,
                    FileName=uploadRequestDto.File.FileName,
                   
                };

                //Pass domain model and call repository method

                await imageRepository.Upload(imageDomainModel);

                return Ok(imageDomainModel);    
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto uploadRequestDto)
        {
            var allowedExtension = new string[] {".jpg",".jpeg",".PNG" };

            if (!allowedExtension.Contains(Path.GetExtension(uploadRequestDto.File.FileName)))
            {
                ModelState.AddModelError("file","Unsupported File Extension");
            }

            if (uploadRequestDto.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size is more than 10 MB, please provide smaller size file");
            }
        }
    }
}
