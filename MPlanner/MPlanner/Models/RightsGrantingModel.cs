using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MPlanner.Models
{
    [NotMapped]
    public class RightsGrantingModel
    {
        [Required]
        public string Email { get; set; }
    }
}
