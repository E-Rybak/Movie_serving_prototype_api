using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieServingApiPrototype.Dtos;
using MovieServingApiPrototype.Helpers;
using MovieServingApiPrototype.Services;
using System.Collections.Generic;

namespace MovieServingApiPrototype.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private IMovieService _movieService;
        private readonly AppSettings _appSettings;

        public MoviesController(IMovieService movieService, IOptions<AppSettings> appSettings)
        {
            _movieService = movieService;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IEnumerable<MovieDto> GetAll()
        {
            var key = _appSettings.ApiKey;
            _movieService.SetApiKey(key);

            return _movieService.GetAll();
        }
    }
}