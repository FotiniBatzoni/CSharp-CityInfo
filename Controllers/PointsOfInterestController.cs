using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityid}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        //ILogger<PointsOfInterestController>  ---  ILogger type PointsOfInterestController
        //tj.   PointsOfInterestContoller : ILogger
        private readonly ILogger<PointsOfInterestController> _logger;

        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;


        //CONSTRACTOR
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityid)
        {
            try
            {

                //throw new Exception("Exeption sample");
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityid} wasn't found");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
           catch(Exception ex)
            {
                //only for development enviroment
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityid}",ex);
               return StatusCode(500,"A problem happened while hadling your request");
            }

     
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterest(
            int cityid, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(
            int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            //Checks if data from from body are valid---not necessary
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
                );
        }

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
           
        }
    }

};
