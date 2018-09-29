using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MPlanner.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        [Display(Name = "Time [minutes]")]
        public int? Time { get; set; }
        public string Director { get; set; }
        public string Year { get; set; }
        public string Actors { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }

        public int Position { get; set; }
    }
}
