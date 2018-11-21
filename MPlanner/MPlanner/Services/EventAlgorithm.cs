using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using MPlanner.Models;

namespace MPlanner.Services
{
    public class EventAlgorithm : IEventAlgorithm
    {
        public string Execute(List<Movie> movies, Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability, DateTime begin)
        {
            List<Movie> notMappedMovies = movies.Where(m => !availability.Any(day => day.Value.amount >= m.Time)).ToList();
            movies = movies.Except(notMappedMovies).ToList();

            Calendar calendar = new Calendar();
            if (movies.Count != 0)
            {
                int index = 0;
                Movie movie = movies[index];
                for (DateTime iterator = begin; index < movies.Count; iterator = iterator.AddDays(1))
                {
                    var (availableStart, availableEnd, availableAmount) = availability[iterator.DayOfWeek];
                    while (availableAmount >= movie.Time.Value)
                    {
                        DateTime startTime = new DateTime(iterator.Year, iterator.Month, iterator.Day, availableStart.Value.Hour,
                            availableStart.Value.Minute, 0);
                        DateTime endTime = startTime.AddMinutes(movie.Time.Value);
                        availableAmount -= movie.Time.Value;
                        availableStart = endTime;

                        CalendarEvent calEvent = new CalendarEvent
                        {
                            Start = new CalDateTime(startTime),
                            End = new CalDateTime(endTime),
                            Description = "Seance generated with MPlanner",
                            Summary = movie.Title
                        };

                        calendar.Events.Add(calEvent);
                        index++;
                        if (index >= movies.Count)
                            break;
                        movie = movies[index];
                    }
                }
            }

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }
    }
}
