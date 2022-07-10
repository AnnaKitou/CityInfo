using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterest : ControllerBase
    {
        private readonly ILogger<PointsOfInterest> _logger;
        private readonly IMailService _mailService;

        public CitiesDataStore _citiesDataStore;

        public PointsOfInterest(ILogger<PointsOfInterest> logger,IMailService mailService, CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore??throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDTO>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"City with id {cityId} wasn't found when accessing points of interest.", ex);
                return StatusCode(500, "A problem happened while handling your request");
            }

        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDTO> GetPointOfInterest(int cityId, int pointofInterestId)
        {

            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            //find point of interest
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointofInterestId);


            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDTO> CreatePointOfInterest(int cityId, PointOfInterestForCreationDTO pointofInterest)
        {

            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
                c => c.PointsOfInterest).Max(x => x.Id);
            var finalPointOfInterest = new PointOfInterestDTO()
            {
                Id = ++maxPointOfInterestId,
                Name = pointofInterest.Name,
                Description = pointofInterest.Description,
            };
            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointofInterestId = finalPointOfInterest.Id
                }
                , finalPointOfInterest);
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityid, int pointOfInterestId,
            PointOfInterestForUpdateDTO pointOfInterest)
        {
            var city = _citiesDataStore.Cities.
                FirstOrDefault(x => x.Id == cityid);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.
                FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;
            return NoContent();

        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityid, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDTO> patchDocument)
        {

            var city = _citiesDataStore.Cities.
                FirstOrDefault(x => x.Id == cityid);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.
             FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointofInterestToPatch =
                new PointOfInterestForUpdateDTO()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };
            patchDocument.ApplyTo(pointofInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(pointofInterestToPatch))
            {
                return BadRequest(ModelState);
            }
            pointOfInterestFromStore.Name = pointofInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointofInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityid, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.
                FirstOrDefault(x => x.Id == cityid);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.
             FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }
            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest was deleted",$"Point of interest  {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted");
            return NoContent();
        }
    }
}
