using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }


        // https://localhost:portnumber/api/Regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Get Data from Database (domain models)
            var regionsDomain = await regionRepository.GetAllAsync();
            //map domain model to DTO
            #region manual mapping
            //var regionsDto = new List<RegionDto>();
            //foreach (var region in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto
            //    {
            //        Id = region.Id,
            //        Name = region.Name,
            //        Code = region.Code,
            //        RegionImageUrl = region.RegionImageUrl
            //    });
            //} 
            #endregion

            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
            //Return DTO
            return Ok(regionsDto);
        }

        // https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            var region = await regionRepository.GetByIdAsync(id);
            if (region == null) 
            { 
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }


        // https://localhost:portnumber/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionDto addRegionRequestDto)
        {
            //map Dto to domain model

            var RegionDomainModel = mapper.Map<Region>(addRegionRequestDto);
            //use domain model to create Region
            RegionDomainModel = await regionRepository.CreateAsync(RegionDomainModel);

            var regionDto = mapper.Map<RegionDto>(RegionDomainModel);

            return CreatedAtAction(nameof(GetById), new{ Id = regionDto.Id}, regionDto);//201
        }


        // https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
        {
            var regionDomainModel = mapper.Map<Region>(updateRegionDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }


        [HttpDelete]
        // https://localhost:portnumber/api/regions
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteAsync(id);
            if (region == null) return NotFound();

            var regionDto = mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }


    }
}
