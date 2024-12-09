﻿using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk> CreateAsync(Walk walk);
        Task<List<Walk>> GetAllAsync();
        Task<Walk?> GetById(Guid id);
        Task<Walk?> UpdateAync(Guid id, Walk walk);
        Task<Walk?> DeleteAsync(Guid id);
    }
}