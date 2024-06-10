using NZWalks.data;
using NZWalks.Models.Domain;

namespace NZWalks.Repository
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetRegionAsync();
        Task<Region?> GetRegionByIdAsync(Guid id);

        Task<Region> CreateRegionAsync(Region region);

        Task<Region?> UpdateRegionAsync(Guid id, Region region);
        Task<Region?> DeleteRegionAsync(Guid regionId);
    }
}
