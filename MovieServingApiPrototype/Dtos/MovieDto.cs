using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieServingApiPrototype.Dtos
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int RunningTime { get; set; }
        public string Director { get; set; }
        public List<GenreDto> Genres { get; set; }

        public MovieDto()
        {
            Genres = new List<GenreDto>();
        }
    }
}
