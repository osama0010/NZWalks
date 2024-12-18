﻿using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLRegionRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await dbContext.Regions.AddAsync(region);
            await dbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var existRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (existRegion == null) return null;

            dbContext.Regions.Remove(existRegion);
            await dbContext.SaveChangesAsync();
            return existRegion;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await dbContext.Regions.ToListAsync();

        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var existRegion = await dbContext.Regions.FirstOrDefaultAsync(y => y.Id == id);
            if (existRegion == null) { return null; }

            existRegion.Code = region.Code;
            existRegion.Name = region.Name;
            existRegion.RegionImageUrl = region.RegionImageUrl;

            await dbContext.SaveChangesAsync();
            return existRegion;
        }
    }
}
