using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MPlanner.Models
{
    [NotMapped]
    public class SearchDataModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        [Display(Name = "Minimum time [minutes]")]
        public int? MinTime { get; set; }
        [Display(Name = "Maximum time [minutes]")]
        public int? MaxTime { get; set; }
        public string Director { get; set; }
    }
}
