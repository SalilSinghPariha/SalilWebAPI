using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.data;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using NZWalks.Repository;
using AutoMapper;
using NZWalks.CustomActionFilters;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize] //this will give 401 if we are trying to access api method without JWT
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext nZWalksDbContext; // it should be readonly which
                                                            // is to initialized the DB context property

        private readonly IRegionRepository regionRepository; // initialezed the region repository

        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        //Inject all these to region controller
        public RegionsController(NZWalksDbContext nZWalksDbContext
            , IRegionRepository regionRepository
            ,IMapper mapper
            ,ILogger<RegionsController> logger) 
        { 
            this.nZWalksDbContext= nZWalksDbContext;
            this.regionRepository= regionRepository;
            this.mapper= mapper;
            this.logger = logger;
        }

        //With Async : Aynchronos programing and it is good practice so that main thread
        //won't get blocked and we can achive this by making method async with TASK and making
        //db call as awaits wherever it's being used
       [HttpGet]
      //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetRegion()
        {
            //Get Data from databse - Database model

            //var regions = await nZWalksDbContext.regions.ToListAsync();
            // since now we are having repository so we can use that with the help of depedency injection

            //get data from database - Domain Model
            logger.LogInformation("Get Region call Processing before repository call!! ");
            var regions = await regionRepository.GetRegionAsync();

            logger.LogInformation($"Finished get region request call with data:{JsonSerializer.Serialize(regions)}");

            //Map domainmodel to DTOs

            //var regionDto = new List<RegionDto>(); // create new list of dto 
            //foreach (var region in regions)
            //{
            //    // assiging particular model to dto
            //    regionDto.Add(new RegionDto()
            //    {
            //        Id = region.Id,
            //        Name = region.Name,
            //        RegionImageurl = region.RegionImageurl,
            //        Code = region.Code,
            //    });
            //}

            // Map domain model to dto using mapper class
            //var regionDto = mapper.Map<List<RegionDto>>(regions);
            // return dto
            //return Ok(regionDto);

            // we can do this in one line
            return Ok(mapper.Map<List<RegionDto>>(regions));


        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetRegionById([FromRoute]Guid id)
        {
            //var regions = await nZWalksDbContext.regions.FirstOrDefaultAsync(x => x.Id == id); // get region based on ID
            
            //get data from Database - Model
            var regions= await regionRepository.GetRegionByIdAsync(id);

            if (regions== null)
            {
                return NotFound();
            }

            //var regionDto = new RegionDto
            //{
            //    Id= regions.Id,
            //    Name = regions.Name,
            //    RegionImageurl = regions.RegionImageurl,
            //    Code = regions.Code,

            //};
            // Map domain model to dto using mapper class
            //return dto back to client
            return Ok(mapper.Map<RegionDto>(regions));
        }
        //without Async and above with async
        [HttpPost]
        [Route("CreateRegionWithoutAsync")]
        public IActionResult CreateRegionWithoutAsync([FromBody] RegionRequest regionRequest)
        {

            //Convert dto to domain model

            var regionDomainModel = new Region
            {
                Name = regionRequest.Name,
                Code = regionRequest.Code,
                RegionImageurl  =regionRequest.RegionImageurl
              
            };

            // Use Domain Model to create region

           nZWalksDbContext.Add(regionDomainModel);
            nZWalksDbContext.SaveChanges();

            // map  model map to dto 

            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,  
                Code=regionDomainModel.Code,
                RegionImageurl=regionDomainModel.RegionImageurl
            };

            return CreatedAtAction(nameof(GetRegionById), new { id = regionDto.Id },regionDto);

            // This crrateAtAction will give us 201 response not 200 and with location
            // using that we can browse and get the response and we can use that 


        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        [ValidateModel]
        public async Task<IActionResult> CreateRegion([FromBody] RegionRequest regionRequest)
        {
                //Convert dto to domain model

                //var regionDomainModel = new Region
                //{
                //    Name = regionRequest.Name,
                //    Code = regionRequest.Code,
                //    RegionImageurl = regionRequest.RegionImageurl

                //};
                // Map regionRequest to domain model
                var regionDomainModel = mapper.Map<Region>(regionRequest);

                // Use Domain Model to create region

                //await nZWalksDbContext.AddAsync(regionDomainModel);
                //await nZWalksDbContext.SaveChangesAsync();

                // Now with repository Pattern

                regionDomainModel = await regionRepository.CreateRegionAsync(regionDomainModel);

                // map  model map to dto 

                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Name = regionDomainModel.Name,
                //    Code = regionDomainModel.Code,
                //    RegionImageurl = regionDomainModel.RegionImageurl
                //};
                //Map Domain model to dto

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetRegionById), new { id = regionDto.Id }, regionDto);

                // This crrateAtAction will give us 201 response not 200 and with location using that we can browse and get the response and we can use that 

                //           content - type: application / json; charset = utf - 8
                //date: Sat,27 Jan 2024 10:47:53 GMT
                //location: https://localhost:7149/api/Regions/c7f5c756-2e0f-49c6-8efb-08dc1f25681d 
                //       server: Kestrel
           
        }
        [HttpPut]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id,[FromBody] UpdateRegionDto updateRegionDto)
        {
            
                //Check if region exist
                //map dto to domain model

                //var regionDomainModel = new Region
                //{
                //    Code = updateRegionDto.Code,
                //    Name = updateRegionDto.Name,
                //    RegionImageurl  =updateRegionDto.RegionImageurl

                //};

                var regionDomainModel = mapper.Map<Region>(updateRegionDto);

                //var regionDomainModel = await nZWalksDbContext.regions.FirstOrDefaultAsync(x => x.Id == id);

                regionDomainModel = await regionRepository.UpdateRegionAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                // convert Domain Model to DTO

                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Name = regionDomainModel.Name, 
                //    Code=regionDomainModel.Code,
                //};


                return Ok(mapper.Map<RegionDto>(regionDomainModel));
            
        }

        [HttpDelete]
        [Authorize(Roles = "Writer")]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            //var regionDomainModel = await nZWalksDbContext.regions.FirstOrDefaultAsync(x => x.Id == id);

            var regionDomainModel = await regionRepository.DeleteRegionAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // delete region
            // remove is synchronous so it won't have async remove so we will use same 
           // nZWalksDbContext.regions.Remove(regionDomainModel);
           //await nZWalksDbContext.SaveChangesAsync();

            // return delted dto

            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageurl = regionDomainModel.RegionImageurl

            //};

            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }
    
    }
}
