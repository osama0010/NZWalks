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

        public async Task<List<Walk>> GetAllAsync()
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
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
