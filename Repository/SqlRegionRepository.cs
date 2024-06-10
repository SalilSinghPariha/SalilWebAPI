using Microsoft.EntityFrameworkCore;
using NZWalks.data;
using NZWalks.Models.Domain;

namespace NZWalks.Repository
{
    public class SqlRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        public SqlRegionRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<Region> CreateRegionAsync(Region region)
        {
           await nZWalksDbContext.regions.AddAsync(region);
           await nZWalksDbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteRegionAsync(Guid regionId)
        {
            var existingRegion = await nZWalksDbContext.regions.FirstOrDefaultAsync(x => x.Id == regionId);

            if (existingRegion == null)
            {
                return null;
            }

            nZWalksDbContext.regions.Remove(existingRegion);
            await nZWalksDbContext.SaveChangesAsync();

            return existingRegion;

        }

        public async Task<List<Region>> GetRegionAsync()
        {
            return await nZWalksDbContext.regions.ToListAsync();
        }

        public async Task<Region?> GetRegionByIdAsync(Guid id)
        {
            return await nZWalksDbContext.regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region?> UpdateRegionAsync(Guid id, Region region)
        {
            var existingRegion = await nZWalksDbContext.regions.FirstOrDefaultAsync(x=>x.Id == id);
            if (existingRegion == null)
            {
                return null;
            }

            existingRegion.Code = region.Code;
            existingRegion.Name = region.Name;
            existingRegion.RegionImageurl = region.RegionImageurl;

            await nZWalksDbContext.SaveChangesAsync();
            return existingRegion;

           
        }
    }
}
