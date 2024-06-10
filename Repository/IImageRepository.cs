using NZWalks.Models.Domain;

namespace NZWalks.Repository
{
    public interface IImageRepository
    {

        Task<Image> Upload(Image image);
    }
}
