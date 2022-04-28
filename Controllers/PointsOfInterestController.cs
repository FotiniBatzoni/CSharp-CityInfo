using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityid}/pointsofinterest")]
    [Authorize(Policy ="MustBeFromAntwerp")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        //ILogger<PointsOfInterestController>  ---  ILogger type PointsOfInterestController
        //tj.   PointsOfInterestContoller : ILogger
        private readonly ILogger<PointsOfInterestController> _logger;

        private readonly IMailService _mailService;
        // private readonly CitiesDataStore _citiesDataStore;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        //CONSTRACTOR
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService
                ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository
                ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            //1st way
            // try
            // {
            //throw new Exception("Exeption sample");
            //     var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

            //     if (city == null)
            //     {
            //         _logger.LogInformation($"City with id {cityid} wasn't found");
            //         return NotFound();
            //     }
            //     return Ok(city.PointsOfInterest);
            // }
            //catch(Exception ex)
            // {
            //only for development enviroment
            //     _logger.LogCritical($"Exception while getting points of interest for city with id {cityid}",ex);
            //    return StatusCode(500,"A problem happened while hadling your request");
            // }

            //2nd way

            //only the users in the city have access in the points of interest
            var cityName = User.Claims.FirstOrDefault(
                c => c.Type == "city")?.Value;

            
            if(!(await _cityInfoRepository.CityMatchesCityId(cityName, cityId))){
                return Forbid();
            };  

            if (!await _cityInfoRepository
                .CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} " +
                    $"wasn't found when accessing" +
                    $"points of interest");
                return NotFound();
            }


            var pointsOfInterestForCity = await
                _cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId);


            return Ok(
                _mapper.Map<
                    IEnumerable<PointOfInterestDto>>(
                    pointsOfInterestForCity));

        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            //1st way
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            //if (pointOfInterest == null)
            //{
            //    return NotFound();
            //}
            //return Ok(pointOfInterest);

            //2nd way
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await
                _cityInfoRepository
                .GetPointOfInterestForCityAsync(
                    cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<
                PointOfInterestDto>(pointOfInterest));

        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            /*       //Older version
             *       //Checks if data from from body are valid---not necessary
                        // if(!ModelState.IsValid)
                        // {
                        //     return BadRequest();
                        // }

                        //find the city
                        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                        if (city == null)
                        {
                            return NotFound();
                        }

                        //demo purposes - to be improved
                        var maxPointOfIterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

                        //model Point Of interest?
                        var finalPointOfInterst = new PointOfInterestDto()
                        {
                            Id = ++maxPointOfIterestId,
                            Name = pointOfInterest.Name,
                            Description = pointOfInterest.Description,
                        };

                        //Add new point Of interest
                        city.PointsOfInterest.Add(finalPointOfInterst);

                        //???
                        return CreatedAtRoute("GetPointOfInterest",
                            new
                            {
                                cityid = cityId,
                                pointOfInterestId = finalPointOfInterst.Id,
                            },
                            finalPointOfInterst
                            );*/

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(
                cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn =
            _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);



        }
        /*
                [HttpPut("{pointOfInterestId}")]
                public ActionResult UpdatePointOfInterest (
                  int cityId,
                  int pointOfInterestId,
                  [FromBody] PointOfInterestForUpdateDto pointOfInterest)
                {
                    //find the city
                    var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                    if (city == null)
                    {
                        return NotFound();
                    }

                    //find point of interest
                    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

                    if(pointOfInterestFromStore == null)
                    {
                        return NotFound(); 
                    }



                    //update 
                    pointOfInterestFromStore.Name = pointOfInterest.Name;

                    //gets the default value if we uodate only name
                    //default value is null
                    pointOfInterestFromStore.Description = pointOfInterest.Description;

                    //returns status 204 No Content
                    return NoContent();
                }
        */
        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(
                 int cityId,
                 int pointOfInterestId,
                 [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            //find the city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            //find point of interest
            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId,
                pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }


            _mapper.Map(pointOfInterest,
                pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();


            //returns status 204 No Content
            return NoContent();
        }
        /*
                [HttpPatch("{pointOfInterestId}")]

                public ActionResult PartiallyUpdatePointOfInterest(
                    int cityId,
                    int pointofInterestId,
                    JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
                    )
                {
                    var city = CitiesDataStore.Current.Cities.FirstOrDefault(c=> c.Id == cityId);

                    if(city== null)
                    {
                        return NotFound();
                    }

                    var pointofInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofInterestId);

                    if( pointofInterestFromStore == null)
                    {
                        return NotFound();
                    }

                    var pointOfIntersetToPatch = new PointOfInterestForUpdateDto()
                    {
                        Name = pointofInterestFromStore.Name,
                        Description = pointofInterestFromStore.Description,
                    };

                    patchDocument.ApplyTo(pointOfIntersetToPatch, ModelState);

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (!TryValidateModel(pointOfIntersetToPatch))
                    {
                        return BadRequest(ModelState);
                    }

                    pointofInterestFromStore.Name = pointOfIntersetToPatch.Name;
                    pointofInterestFromStore.Description = pointOfIntersetToPatch.Description;

                    return NoContent();

                }
        */
        [HttpPatch("{pointOfInterestId}")]

        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
                   int cityId,
                   int pointofInterestId,
                   JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
                   )
        {

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                    .GetPointOfInterestForCityAsync(cityId, pointofInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                _mapper.Map<PointOfInterestForUpdateDto>(
                    pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch,
                pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();

        }

        /*
        [HttpDelete("{pointOfInterestId}")]
                public ActionResult DeletePointOfInterest(
                    int cityId,
                    int pointOfInterestid)
                {
                    var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                    if(city == null)
                    {
                        return NotFound();
                    }

                    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestid);    

                    if(pointOfInterestFromStore == null)
                    {
                        return NotFound();
                    }

                    city.PointsOfInterest.Remove(pointOfInterestFromStore);

                    city.PointsOfInterest.Remove(pointOfInterestFromStore);
                    _mailService.Send(
                        "Points of interest deleted",
                        $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted");

                    return NoContent();

                }*/

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(
                    int cityId,
                    int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await
                _cityInfoRepository.GetPointOfInterestForCityAsync(
                    cityId,
                    pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(
                pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();   

            
                        _mailService.Send(
                "Points of interest deleted",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted");

            return NoContent();
        }
    }


};
