using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    //   [Authorize(Policy = "MustBeFromAntwerp")]
    [ApiVersion("2.0")]
    [ApiController]
    public class PointsOfInterest : ControllerBase
    {
        private readonly ILogger<PointsOfInterest> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterest(ILogger<PointsOfInterest> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDTO>>> GetPointsOfInterest(int cityId)
        {
            //var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            //if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            //{
            //    return Forbid();
            //}

            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interes.");
                return NotFound();
            }
            var pointsOfInterest = await _cityInfoRepository
                  .GetPointOfInterestsForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDTO>>(pointsOfInterest));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDTO>> GetPointOfInterest(int cityId, int pointofInterestId)
        {


            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }
            //find point of interest
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestsForCityAsync(cityId, pointofInterestId);


            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDTO>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDTO>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDTO pointofInterest)
        {

            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointofInterest);
            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDTO>(finalPointOfInterest);


            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointofInterestId = createPointOfInterestToReturn.Id
                }
                , createPointOfInterestToReturn);
        }
        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDTO pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestsForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDTO> patchDocument)
        {

            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestsForCityAsync(cityId, pointOfInterestId);


            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDTO>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }


            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestsForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send("Point of interest was deleted",
                $"Point of interest  {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted");
            return NoContent();
        }

    }
}