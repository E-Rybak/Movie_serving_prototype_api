using MovieServingApiPrototype.Dtos;
using MovieServingApiPrototype.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieServingApiPrototype.Services
{
    public interface IMovieService
    {
        void SetApiKey(string apiKey);

        MovieDto GetById(int id);
        IEnumerable<MovieDto> GetRunning(int page);
        IEnumerable<MovieDto> GetPopular(int page);
        IEnumerable<MovieDto> SearchByTitle();
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
            movie.Description = jsonData[MovieJsonKey.Overview].ToString();

            //Populate movie's genres list
            var genres = jsonData[MovieJsonKey.Genres].ToList();
            foreach (var genre in genres)
            {
                var genreDto = new GenreDto() { Name = genre[MovieJsonKey.GenreName].ToString() };
                movie.Genres.Add(genreDto);
            }

            var posterFilePath = (string)jsonData[MovieJsonKey.PosterPath]; // poster file path

            //Fetch configuration for baseurl and size, to build a full poster image path
            url = "https://api.themoviedb.org/3/configuration?api_key=" + _apiKey;
            var uri = new Uri(url);
            _client.BaseUrl = uri;
            response = _client.Execute(_request);
            jsonData = JObject.Parse(response.Content);
            
            var baseSecureUrl = jsonData[MovieJsonKey.Images][MovieJsonKey.SecureBaseUrl].ToString();

            movie.posterUrl = baseSecureUrl + "w342" + posterFilePath; // creates full poster image path and assigns it the movie

            return movie;
        }

        public IEnumerable<MovieDto> GetRunning(int page)
        {
            return GetMoviesWithGenres(page, "https://api.themoviedb.org/3/movie/now_playing");
        }

        public IEnumerable<MovieDto> GetPopular(int page)
        {
            return GetMoviesWithGenres(page, "https://api.themoviedb.org/3/movie/popular");
        }

        public IEnumerable<MovieDto> SearchByTitle()
        {
            throw new System.NotImplementedException();
        }

        //Private helper methods
        private List<GenreDto> genreListFromIds(int[] ids, IRestResponse response)
        {
            //Parses data from response into list of jtokens, each of which contain a genre name
            var jsonData = JObject.Parse(response.Content);
            var genres = jsonData[MovieJsonKey.Genres].ToList();
            var returnGenreList = new List<GenreDto>();

            //runs through the different genre ids, finds them in the list of genres and adds the found genre to the 
            //list of genres which is then returned the the movieDto.
            foreach (var id in ids)
            {
                var genre = genres.Find(g => (int)g[MovieJsonKey.Id] == id);
                if (genre != null)
                {
                    var genreDto = new GenreDto() { Name = genre[MovieJsonKey.GenreName].ToString() };
                    returnGenreList.Add(genreDto);
                }
            }
            return returnGenreList;
        }
        private IEnumerable<MovieDto> GetMoviesWithGenres(int page, string path)
        {
            //Requests running movies from tmdb and parses json response into JObject
            _request = new RestRequest(Method.GET);
            var url = path + "?api_key=" + _apiKey + "&page=" + page + "&region=dk";
            _client = new RestClient(url);
            var response = _client.Execute(_request);
            var jsonData = JObject.Parse(response.Content);

            //Parse the result array of movies into a list of jtoken movies.
            var results = jsonData[MovieJsonKey.Results].ToList();

            //Request the genres and save the response 
            _request = new RestRequest(Method.GET);
            url = "https://api.themoviedb.org/3/genre/movie/list?api_key=" + _apiKey + "&language=en-us";
            _client = new RestClient(url);
            response = _client.Execute(_request);

            //Runs through each movie in the result list and creates a new Dto witch is populated and added the movies list
            //The genres prop is calling a private helper method, genreListFromIds, witch populates the movieDto's list of genres
            //from the genre_ids which were passed in the results list. 
            var movies = new List<MovieDto>();
            foreach (var result in results)
            {
                var ids = result[MovieJsonKey.GenreIds].Values<int>().ToArray();
                var movie = new MovieDto
                {
                    Title = result[MovieJsonKey.MovieTitle].ToString(),
                    Description = result[MovieJsonKey.Overview].ToString(),
                    Id = (int)result[MovieJsonKey.Id],
                    posterUrl = "https://image.tmdb.org/t/p/w342" + result[MovieJsonKey.PosterPath].ToString(),
                    Genres = genreListFromIds(ids, response),
                    Director = ""
                };
                movies.Add(movie);
            }
            return movies;
        }
    }
}
