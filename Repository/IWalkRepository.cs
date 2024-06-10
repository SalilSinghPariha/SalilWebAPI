using NZWalks.Models.Domain;

namespace NZWalks.Repository
{
    public interface IWalkRepository
    {
       Task<Walk> createWalkAsync(Walk walk);
        Task<List<Walk>> GetWalkAsync(string? filterOn=null, string? filterQuery=null,
            string? sortBy=null, bool isAscendng=true,
            int pageNumber=1, int pageSize=1000);

        Task<Walk?> GetWalkByIdAsync(Guid id);

        Task<Walk?> UpdateWalkAsync(Guid id, Walk walk);

        Task<Walk?> DeleteWalkAsync(Guid id);
    }
}
