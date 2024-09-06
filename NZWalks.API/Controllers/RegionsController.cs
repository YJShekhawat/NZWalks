using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(IRegionRepository regionRepository,
            IMapper mapper,ILogger<RegionsController> logger)
        {
            //this.nZWalksDbContext = nZWalksDbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {   //hardcord code
            //var regions = new List<Region>
            //{
            //    new Region
            //    { Id= Guid.NewGuid(),
            //      Code="12345",
            //      Name="Vijay",
            //      RegionImageUrl="https/Vijay7"
            //    },
            //    new Region
            //    { Id=Guid.NewGuid(),
            //      Code="43434",
            //      Name="dfdf",
            //      RegionImageUrl="https/Vijay7"
            //    }
            //};

            //return Ok(regions);
            logger.LogInformation("GetAll regions method got invoked");
            //logger.LogError("error got");
            var regionsDomain = await regionRepository.GetAllAsync();


            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            logger.LogInformation($"GetAll regions request data: {JsonSerializer.Serialize(regionsDto)}");

            //converting to DTO old way


            //var regionsDto = new List<RegionDto>();
            //foreach (var regiondomain in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = regiondomain.Id,
            //        Name = regiondomain.Name,
            //        Code = regiondomain.Code,
            //        RegionImageUrl = regiondomain.RegionImageUrl
            //    });
            //}
            logger.LogInformation("GetAll regions method execution got completed");

            return Ok(regionsDto);

        }


        //id:Guid for type safety we can use id only also
        [HttpGet]
        [Route("{Id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid Id)
        {
            //var regions = nZWalksDbContext.Regions.ToList().Where(i=>i.Id==id);
            //var regions = nZWalksDbContext.Regions.Find(id);
            var regiondomain = await regionRepository.GetByIdAsync(Id);

            if (regiondomain == null)
                return NotFound();

            //domain to dto mapping using automapper

            var regionDto=mapper.Map<RegionDto>(regiondomain);

            //domain to dto mapping old way
            //var regionDto = new RegionDto()
            //{

            //    Id = regiondomain.Id,
            //    Name = regiondomain.Name,
            //    Code = regiondomain.Code,
            //    RegionImageUrl = regiondomain.RegionImageUrl

            //};


            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {

                //converting dto to domain
                var region = mapper.Map<Region>(addRegionRequestDto);
                //conver to domain model old way
                //var region =  new Region
                //{
                //    Code = addRegionRequestDto.Code,
                //    Name = addRegionRequestDto.Name,
                //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
                //};

                region = await regionRepository.CreateAsync(region);
                ////adding to db
                //await nZWalksDbContext.Regions.AddAsync(region);
                //await nZWalksDbContext.SaveChangesAsync();

                //Domain to dto using automapper

                var regionDto = mapper.Map<RegionDto>(region);

                //we have to return thr response to client map again to client old way
                //var regionDto = new RegionDto
                //{
                //    Id = region.Id,
                //    Name = region.Name,
                //    Code = region.Code,
                //    RegionImageUrl = region.RegionImageUrl
                //};

                return CreatedAtAction(nameof(GetById), new { Id = regionDto.Id }, regionDto);


        }

        [HttpPut]
        [Route("{Id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid Id, UpdateRegionRequestDto updateRegionRequestDto)
        {
           
                //conver to domain old way
                //var region = new Region
                //{   
                //    Code = updateRegionRequestDto.Code,
                //    Name = updateRegionRequestDto.Name,
                //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
                //};

                //convert to domain using automapper

                var region = mapper.Map<Region>(updateRegionRequestDto);

                region = await regionRepository.UpdateAsync(Id, region);

                if (region == null)
                {
                    return NotFound();
                }


                //mapping to dto to return client old way

                //var regionDto = new RegionDto
                //{
                //    Id = region.Id,
                //    Name = region.Name,
                //    Code = region.Code,
                //    RegionImageUrl = region.RegionImageUrl
                //};

                //using autopmapper converting to dto
                var regionDto = mapper.Map<RegionDto>(region);

                return Ok(regionDto);
        }

        [HttpDelete]
        [Route("{Id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid Id)
        {
            var region = await regionRepository.DeleteAsync(Id);
            if(region==null)
            {
                return NotFound();
            }


            //if we weant to pass the delete object to cilent map dto old way
            //var regionDto = new RegionDto
            //{
            //    Id= region.Id,
            //    Code = region.Code,
            //    Name = region.Name,
            //    RegionImageUrl = region.RegionImageUrl

            //};

            //using automapper conversion to dto
            var regionDto = mapper.Map<RegionDto>(region);


            return Ok(regionDto);
        }

    }
}
