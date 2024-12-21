using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
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
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkDto addWalkdto)
        {
            // map from dto to domain model
            var walkDomain = mapper.Map<Walk>(addWalkdto);

            // use domain model to create a walk
            await walkRepository.CreateAsync(walkDomain);

            // map domain back to a dto 
            return Ok(mapper.Map<WalkDto>(walkDomain));

        }

        //localhost:portnum/api/walks/
        //localhost:portnum/api/walks?FilterOn=name&FilterQuery=Mount&sortBy=Name&isAscending=true&pageNumber=1&pageSize=5
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? FilterOn, [FromQuery] string? FilterQuery,
                                [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
                                [FromQuery] int pageNumber=1, [FromQuery] int pageSize=100)
        {
            var walksDomainModel = await walkRepository.GetAllAsync(FilterOn,FilterQuery,sortBy,isAscending ?? true,
                                                                    pageNumber,pageSize);

            // Create an Exception
            //throw new Exception("This is a new exception");

            // Map domain model to Dto
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
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkDto updateWalkDto)
        {
            var walkModelDomain = mapper.Map<Walk>(updateWalkDto);

            walkModelDomain = await walkRepository.UpdateAync(id, walkModelDomain);

            if (walkModelDomain == null) return NotFound();

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
