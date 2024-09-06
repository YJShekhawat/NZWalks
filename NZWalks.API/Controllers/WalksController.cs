using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository walkRepository,
            IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
          [FromQuery] string? sortBy, [FromQuery] bool? isAscending,[FromQuery] int pageNumber=1,[FromQuery] int pageSize=1000  )
        {
            var walks = await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending ?? true,pageNumber,pageSize);
            if(walks==null)
                return NotFound();

            var walksDto=mapper.Map<List<WalkDto>>(walks);

            throw new Exception("Sudden Error");
            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{Id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid Id)
        {
            var walk = await walkRepository.GetByIdAsync(Id);
            if (walk == null)
                return NotFound();

            var walksDto = mapper.Map<WalkDto>(walk);

            return Ok(walksDto);
        }

        [HttpPut]
        [Route("{Id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid Id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
                var walk = mapper.Map<Walk>(updateWalkRequestDto);
                walk = await walkRepository.UpdateAsync(Id, walk);
                if (walk == null)
                    return NotFound();

                var walksDto = mapper.Map<WalkDto>(walk);

                return Ok(walksDto); 
        }


        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {

            var walk = mapper.Map<Walk>(addWalkRequestDto);

            walk = await walkRepository.CreateAsync(walk);

            var walkDto = mapper.Map<WalkDto>(walk);
            return Ok(walkDto);
        }


        [HttpDelete]
        [Route("{Id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid Id)
        {
            var walk = await walkRepository.DeleteAsync(Id);
            if (walk == null)
                return NotFound();

            var walksDto = mapper.Map<WalkDto>(walk);

            return Ok(walksDto);
        }

    }
}
