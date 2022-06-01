using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        private readonly ICountryRepository countryRepository;
        private readonly IAuthorRepository authorRepository;

        public CountriesController(ICountryRepository countryRepository,
            IAuthorRepository authorRepository)
        {
            this.countryRepository = countryRepository;
            this.authorRepository = authorRepository;
        }

        // /api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = countryRepository.GetCountries().ToList();
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countriesDto = new List<CountryDto>();
            foreach (var country in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name
                });
            }

            return Ok(countriesDto);
        }

        // /api/countries/countryId
        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId)
        {
            if (!countryRepository.CountryExist(countryId))
                return NotFound();

            var country = countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto() {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        // /api/countries/authors/authorId
        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAuthor(int authorId)
        {
            if (!authorRepository.AuthorExist(authorId))
                return NotFound();

            var country = countryRepository.GetCountryOfAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        // /api/countries/countryId/authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsFromCountry(int countryId)
        {
            if (!countryRepository.CountryExist(countryId))
                return NotFound();

            var authors = countryRepository.GetAuthorsFromCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();
            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }

        // /api/countries
        [HttpPost]
        [ProducesResponseType(500)]  
        [ProducesResponseType(422)]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Country))] //created
        public IActionResult CreateCountry([FromBody]Country creatingCountry)
        {
            if (creatingCountry == null)
                return BadRequest(ModelState);

            var country = countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() ==
                creatingCountry.Name.Trim().ToUpper()).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", $"Country {creatingCountry.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!countryRepository.CreateCountry(creatingCountry))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't create {creatingCountry.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCountry", new { countryId = creatingCountry.Id }, creatingCountry);
        }

        // /api/countries/countryId
        [HttpPut("{countryId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult UpdateCountry(int countryId, [FromBody]Country updatingCountry)
        {
            if (updatingCountry == null)
                return BadRequest(ModelState);

            if (countryId != updatingCountry.Id)
                return BadRequest(ModelState);

            if (!countryRepository.CountryExist(countryId))
                return NotFound();

            if (countryRepository.IsDuplicateCountryName(countryId, updatingCountry.Name))
            {
                ModelState.AddModelError("", $"Country {updatingCountry.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!countryRepository.UpdateCountry(updatingCountry))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't update {updatingCountry.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // /api/countries/countryId
        [HttpDelete("{countryId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)] // duplicates
        [ProducesResponseType(409)] // conflict 
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult DeleteCountry(int countryId)
        {
            if (!countryRepository.CountryExist(countryId))
                return NotFound();

            var deletingCountry = countryRepository.GetCountry(countryId);

            if (countryRepository.GetAuthorsFromCountry(countryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingCountry.Name}."
                    + " It is used by at least one author.");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!countryRepository.DeleteCountry(deletingCountry))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingCountry.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
