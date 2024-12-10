using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public WalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<List<Walk>> GetAllAsync(string? FilterOn, string? FilterQuery,
                                                  string? sortBy, bool isAscending,
                                                  int pageNumber = 1, int pageSize = 1000)
        {
            //return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            // Filtering
            if (!(string.IsNullOrWhiteSpace(FilterOn)) && !(string.IsNullOrWhiteSpace(FilterQuery)))
            {
                if(FilterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(FilterQuery));
                }
            }
            // Sorting
            if (!(string.IsNullOrWhiteSpace(sortBy)))
            {
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);

                }
            }
            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetById(Guid id)
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAync(Guid id, Walk walk)
        {
            var existedWalk = await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            if (existedWalk == null) return null;



            existedWalk.Name = walk.Name;
            existedWalk.Description = walk.Description;
            existedWalk.LengthInKm = walk.LengthInKm;
            existedWalk.WalkImageUrl = walk.WalkImageUrl;
            existedWalk.RegionId = walk.RegionId;
            existedWalk.DifficultyId = walk.DifficultyId;

            await dbContext.SaveChangesAsync();

            return existedWalk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existedWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if(existedWalk == null) return null;

            dbContext.Remove(existedWalk);
            await dbContext.SaveChangesAsync();
            return existedWalk;
        }

    }
}
