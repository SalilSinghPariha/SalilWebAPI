using Microsoft.EntityFrameworkCore;
using NZWalks.data;
using NZWalks.Models.Domain;

namespace NZWalks.Repository
{
    public class SqlWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        public SqlWalkRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<Walk> createWalkAsync(Walk walk)
        {
           await nZWalksDbContext.walks.AddAsync(walk);
            await nZWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteWalkAsync(Guid id)
        {
            var existingWalk = await nZWalksDbContext.walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWalk == null)
            {
                return null;
            }

             nZWalksDbContext.walks.Remove(existingWalk);

            await nZWalksDbContext.SaveChangesAsync();
            return existingWalk;

        }

        public async Task<List<Walk>> GetWalkAsync(string? filterOn=null, string? filterQuery=null,
            string? sortBy=null, bool isAscending=true,int pageNumber=1,int PageSize=1000)
        {
            // this will just give walk infor not difficulty and region even though we are having relation
            // between that so now if we want that then we need to modify this with include and table name then it willl fetch same
            //return await nZWalksDbContext.walks.ToListAsync();

            //return await nZWalksDbContext.walks.Include("Difficulty").Include("Region").ToListAsync();

            var walks= nZWalksDbContext.walks.Include("Difficulty").Include("Region").AsQueryable();

            // filtering

            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                   walks= walks.Where(x => x.Name == filterQuery);
                }
            }
            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks= isAscending ? walks.OrderBy(x => x.Name) :walks.OrderByDescending(x => x.Name);
                }
            }

            //Pagination

            var skipResults = (pageNumber - 1) * PageSize;


            return await walks.Skip(skipResults).Take(PageSize).ToListAsync()   ;
        }

        public async Task<Walk?> GetWalkByIdAsync(Guid id)
        {
            return await nZWalksDbContext.walks
                .Include("Difficulty")
                .Include("Region")
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateWalkAsync(Guid id, Walk walk)
        {
            var existingWalk = await nZWalksDbContext.walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWalk == null)
            {
                return null;
            }

            existingWalk.Name = walk.Name;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImangeUrl = walk.WalkImangeUrl;
            existingWalk.Description = walk.Description;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;

            await nZWalksDbContext.SaveChangesAsync();

            return existingWalk;

        }
    }
}
