using MovieServingApiPrototype.Dtos;
using MovieServingApiPrototype.Helpers;
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
        void Configure(Method method, string uri);
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
            //Populate MovieDto
            _request = new RestRequest(Method.GET);
            _client = new RestClient()
            var result = _client.Execute(_request);
            throw new System.NotImplementedException();
        }

        public IEnumerable<MovieDto> GetAll()
        {
            //Populate IEnumerable of movies. Populate IEnumerable of Genres.

            return Enumerable.Empty<MovieDto>();
        }
    }
}
