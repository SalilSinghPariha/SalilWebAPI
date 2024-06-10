using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.CustomActionFilters;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Repository;
using System.Globalization;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IWalkRepository walkRepository;
        public WalksController(IMapper mapper,IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        [HttpPost]
        [ValidateModel]
        [Route("CreateWalk")]
        public async Task<IActionResult> Create([FromBody] AddWalksRequestDto addWalksRequestDto)
        {
            
                //map dto to domain which is AddWalksRequestDtoto walk
                var walkDomainModel = mapper.Map<Walk>(addWalksRequestDto);

                await walkRepository.createWalkAsync(walkDomainModel);

                //Map Domain Model to dto

                return Ok(mapper.Map<WalkDto>(walkDomainModel));
            
           
        }

        [HttpGet]
        [Route("GetAllWalks")]
        //GET: /api/walks? filterOn = Name & filterQuery = Track for filtering
        //if we need to do sorting then it will be
        //GET: /api/walks? filterOn = CoulmnName & filterQuery = Track&SortBy=ColumnName&&isAscending=true 
        //Pagination in API
        //GET: /api/walks? filterOn = Name & filterQuery = Track & SortBy = Name && isAscending = true && PageNumber = 1 && PageSize = 5
        public async Task<IActionResult> GetAllWalks([FromQuery] string? filterOn, 
               [FromQuery] string? filterQuery,
               [FromQuery] string? SortBy, [FromQuery] bool? isAscending,
               int pageNumner=1, [FromQuery] int PageSize=1000)
        {
                // isAscending ?? true this we have done since we are getting error so we are passing like if null then pass true

                var walkDomainModel = await walkRepository.GetWalkAsync(filterOn, filterQuery, SortBy, isAscending ?? true, pageNumner, PageSize);

                // Create new exception as this is to test global exception if we are not using try catch and using global excpetion

                //throw new Exception("There is new exception!!");

                //map domain model to dto and return the dto

                return Ok(mapper.Map<List<WalkDto>>(walkDomainModel));
           
        }

        [HttpGet]
        [Route("{id:Guid}")]

        public async Task<IActionResult> GetWalkById([FromRoute] Guid id)
        {
            var walkDomainModel= await walkRepository.GetWalkByIdAsync(id);

            if (walkDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        [HttpPut]
        [Route("{id:Guid}")]

        public async Task<IActionResult> UpdateWalkById([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            if (ModelState.IsValid)
            {
                // Map DTO to domain model

                var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

                walkDomainModel = await walkRepository.UpdateWalkAsync(id, walkDomainModel);

                if (walkDomainModel == null)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<WalkDto>(walkDomainModel));
            }
            else
            {
                return BadRequest(ModelState);
            }
          
        }

        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteWalkById([FromRoute] Guid id)
        {
            var deletedDomainModel = await walkRepository.DeleteWalkAsync(id);

            if(deletedDomainModel == null)
            { 
                return NotFound();
            }

            return Ok(mapper.Map<WalkDto>(deletedDomainModel));
        }
    }
}
