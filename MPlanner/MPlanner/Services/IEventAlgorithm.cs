using MPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPlanner.Services
{
    public interface IEventAlgorithm
    {
         string Execute(List<Movie> movies, Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability, DateTime begin);
    }
}
