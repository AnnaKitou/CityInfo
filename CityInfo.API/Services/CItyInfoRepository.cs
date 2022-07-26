using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CItyInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CItyInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
        {
            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrWhiteSpace(searchQuery))
            {
                return await GetCitiesAsync();
            }

            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(x => x.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(x => x.Name.Contains(searchQuery)
                || (x.Description != null && x.Description.Contains(searchQuery)));
            }

            return await collection.OrderBy(x => x.Name).ToListAsync(); ;
        }
        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(x => x.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(x => x.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsForCityAsync(int cityId)
        {
            return await _context.PointOfInterest
                .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestsForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterest
                         .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterest.Remove(pointOfInterest);
        }
    }
}
