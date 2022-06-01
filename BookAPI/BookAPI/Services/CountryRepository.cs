using BookAPI.Models;

namespace BookAPI.Services
{
    public class CountryRepository : ICountryRepository
    {
        private readonly BookDbContext countryContext;

        public CountryRepository(BookDbContext countryContext)
        {
            this.countryContext = countryContext;
        }

        public bool CountryExist(int countryId)
        {
            return countryContext.Countries.Any(c => c.Id == countryId);
        }

        public bool CreateCountry(Country country)
        {
            countryContext.Add(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            countryContext.Remove(country);
            return Save();
        }

        public ICollection<Author> GetAuthorsFromCountry(int countryId)
        {
            return countryContext.Authors.Where(c => c.Country.Id == countryId).ToList();
        }

        public ICollection<Country> GetCountries()
        {
            return countryContext.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return countryContext.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public Country GetCountryOfAuthor(int authorId)
        {
            return countryContext.Authors.Where(a => a.Id == authorId)
                .Select(c => c.Country).FirstOrDefault();
        }

        public bool IsDuplicateCountryName(int countryId, string countryName)
        {
            var country = countryContext.Countries.Where(c => c.Name.Trim().ToUpper() ==
                countryName.Trim().ToUpper() && c.Id != countryId).FirstOrDefault();
            return country == null ? false : true;
        }

        public bool Save()
        {
            var isSaved = countryContext.SaveChanges();
            return isSaved > 0 ? true : false;
        }

        public bool UpdateCountry(Country country)
        {
            countryContext.Update(country);
            return Save();
        }
    }
}
