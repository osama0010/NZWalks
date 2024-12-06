using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        // https://localhost:portnumber/api/Regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await dbContext.Regions.ToListAsync();

            //map domain model to DTO
            var regionsDto = new List<RegionDto>();
            foreach (var item in regions)
            {
                regionsDto.Add(new RegionDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code = item.Code,
                    RegionImageUrl = item.RegionImageUrl
                });
            }
            //Return DTO
            return Ok(regionsDto);
        }

        // https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            var region = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (region == null) 
            { 
                return NotFound();
            }

            var regionDto = new RegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                RegionImageUrl = region.RegionImageUrl
            };

            return Ok(regionDto);
        }


        // https://localhost:portnumber/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionDto addRegionRequestDto)
        {
            //map Dto to domain model
            var RegionDomainModel = new Region
            {
                Name = addRegionRequestDto.Name,
                Code = addRegionRequestDto.Code,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };
            //use domain model to create Region
            await dbContext.AddAsync(RegionDomainModel);
            await dbContext.SaveChangesAsync();

            var regionDto = new RegionDto
            {
                Id = RegionDomainModel.Id,
                Name = RegionDomainModel.Name,
                Code = RegionDomainModel.Code,
                RegionImageUrl = RegionDomainModel.RegionImageUrl
            };

            return CreatedAtAction(nameof(GetById), new{ Id = regionDto.Id}, regionDto);//201
        }


        // https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
        {
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            regionDomainModel.Code = updateRegionDto.Code;
            regionDomainModel.Name = updateRegionDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return Ok(regionDto);
        }

        [HttpDelete]
        // https://localhost:portnumber/api/regions
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (region == null) return NotFound();

            dbContext.Regions.Remove(region);
            await dbContext.SaveChangesAsync();
            return Ok();
        }


    }
}
