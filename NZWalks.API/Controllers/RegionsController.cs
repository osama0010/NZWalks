using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper, ILogger<RegionsController> logger)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
            ;
        }


        // https://localhost:portnumber/api/Regions
        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                logger.LogInformation("GetAllRegions Action method was invoked");
                #region MinimumLevel.Warning()/Error
                //logger.LogWarning("This is a Warning Log");
                //logger.LogError("This is and Error Log"); 
                #endregion

                //Get Data from Database (domain models)
                var regionsDomain = await regionRepository.GetAllAsync();

                logger.LogInformation($"Finished GetAllRegions Request with this Data: {JsonSerializer.Serialize(regionsDomain)}");

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
            catch (Exception exc)
            {
                logger.LogError(exc, exc.Message);
                throw;
            }


        }

        // https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader")]
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
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionDto addRegionRequestDto)
        {

            //map Dto to domain model
            var RegionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            //use domain model to create Region
            RegionDomainModel = await regionRepository.CreateAsync(RegionDomainModel);

            // map domain back to a dto 
            var regionDto = mapper.Map<RegionDto>(RegionDomainModel);

            return CreatedAtAction(nameof(GetById), new { Id = regionDto.Id }, regionDto);//201

        }


        // https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
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
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteAsync(id);
            if (region == null) return NotFound();

            var regionDto = mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }


    }
}
