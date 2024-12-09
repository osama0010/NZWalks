﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        // https://localhost:portnumber/api/Walks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkDto addWalkdto)
        {
            // map from dto to domain
            var walkDomain = mapper.Map<Walk>(addWalkdto);

            await walkRepository.CreateAsync(walkDomain);

            return Ok(mapper.Map<WalkDto>(walkDomain));
        }

        //localhost:portnum/api/walks/
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var walksDomainModel = await walkRepository.GetAllAsync();

            var walksDto = mapper.Map<List<WalkDto>>(walksDomainModel);

            return Ok(walksDto);
        }

        //localhost:portnum/api/walks/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.GetById(id);

            if (walkDomainModel == null) return NotFound();

            var walkDto = mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkDto updateWalkDto)
        {
            var walkModelDomain = mapper.Map<Walk>(updateWalkDto);

            walkModelDomain = await walkRepository.UpdateAync(id, walkModelDomain);

            if(walkModelDomain == null) return NotFound();
            
            return Ok(mapper.Map<WalkDto>(walkModelDomain));
        
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.DeleteAsync(id);

            if (walkDomainModel == null) return NotFound();

            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

    }
}