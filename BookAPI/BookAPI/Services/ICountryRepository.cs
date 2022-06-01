using BookAPI.Models;

namespace BookAPI.Services
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int countryId);
        Country GetCountryOfAuthor(int authorId);
        ICollection<Author> GetAuthorsFromCountry(int countryId);
        bool CountryExist(int countryId);
        bool IsDuplicateCountryName(int countryId, string countryName);
        bool CreateCountry(Country country);
        bool UpdateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();
    }
}
