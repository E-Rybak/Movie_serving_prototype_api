using MovieServingApiPrototype.Dtos;
using MovieServingApiPrototype.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MovieServingApiPrototype.Services
{
    public interface IMovieService
    {
        IEnumerable<MovieDto> GetAll();
        MovieDto GetById(int id);
        void SetApiKey(string apiKey);
    }

    public class MovieService : IMovieService
    {
        private string _apiKey;
        private RestClient _client;
        private RestRequest _request;

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        public MovieDto GetById(int id)
        {
            _request = new RestRequest(Method.GET);
            var url = "https://api.themoviedb.org/3/movie/" + id + "?api_key=" + _apiKey;
            _client = new RestClient(url);
            var movie = new MovieDto();

            //Execute query to The Movie DB Api.
            var response = _client.Execute(_request);
            //Parse string response into JObject.
            var jsonData = JObject.Parse(response.Content);

            //Populate movie with response data
            movie.Title = jsonData[MovieJsonKey.MovieTitle].ToString();
            movie.Id = (int)jsonData[MovieJsonKey.Id];
            movie.RunningTime = (int)jsonData[MovieJsonKey.RunningTime];

            //Populate movie's genres list
            var genres = jsonData[MovieJsonKey.Genres].ToList();
            foreach (var genre in genres)
            {
                var genreDto = new GenreDto() { Name = genre[MovieJsonKey.GenreName].ToString() };
                movie.Genres.Add(genreDto);
            }

            return movie;
        }

        public IEnumerable<MovieDto> GetAll()
        {
            //Populate IEnumerable of movies. Populate IEnumerable of Genres.

            return Enumerable.Empty<MovieDto>();
        }

    }
}
