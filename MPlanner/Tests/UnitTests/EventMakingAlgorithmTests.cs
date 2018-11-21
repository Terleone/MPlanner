using MPlanner.Models;
using MPlanner.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.UnitTests
{
    public class EventMakingAlgorithmTests
    {
        private readonly EventAlgorithm eventAlgorithm = new EventAlgorithm();

        [Fact]
        public void EmptyChoice()
        {
            Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability = new Dictionary<DayOfWeek, (DateTime?, DateTime?, int)>()
            {
                { DayOfWeek.Monday, (null, null, 0) },
                { DayOfWeek.Tuesday, (null, null, 0) },
                { DayOfWeek.Wednesday, (null, null, 0) },
                { DayOfWeek.Thursday, (null, null, 0) },
                { DayOfWeek.Friday, (null, null, 0) },
                { DayOfWeek.Saturday, (null, null, 0) },
                { DayOfWeek.Sunday, (null, null, 0) }
            };

            Assert.Equal($"BEGIN:VCALENDAR\r\n" +
                $"PRODID:-//github.com/rianjs/ical.net//NONSGML ical.net 4.0//EN\r\n" +
                $"VERSION:2.0\r\n" +
                $"END:VCALENDAR\r\n",
                eventAlgorithm.Execute(new List<Movie>(), availability, /*Not important in this test*/ new DateTime()));
        }

        [Fact]
        public void TwoMoviesOnTheSameDayAndOneOnAnother()
        {
            List<Movie> movies = new List<Movie>
            {
                new Movie() { Time = 120, Title = "A" },
                new Movie() { Time = 105, Title = "B" },
                new Movie() { Time = 140, Title = "C"}
            };

            Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability = new Dictionary<DayOfWeek, (DateTime?, DateTime?, int)>()
            {
                { DayOfWeek.Monday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 17, 0, 0), 120) },
                { DayOfWeek.Tuesday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 19, 5, 0), 245) },
                { DayOfWeek.Wednesday, (null, null, 0) },
                { DayOfWeek.Thursday, (null, null, 0) },
                { DayOfWeek.Friday, (null, null, 0) },
                { DayOfWeek.Saturday, (null, null, 0) },
                { DayOfWeek.Sunday, (null, null, 0) }
            };

            Assert.Equal($"BEGIN:VCALENDAR\r\n" +
                $"PRODID:-//github.com/rianjs/ical.net//NONSGML ical.net 4.0//EN\r\n" +
                $"VERSION:2.0\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181126T170000\r\n" +
                $"DTSTART:20181126T150000\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:A\r\n" +
                $"END:VEVENT\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181127T164500\r\n" +
                $"DTSTART:20181127T150000\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:B\r\n" +
                $"END:VEVENT\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181127T190500\r\n" +
                $"DTSTART:20181127T164500\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:C\r\n" +
                $"END:VEVENT\r\n" +
                $"END:VCALENDAR\r\n",
                RemoveNotReproducableElements(eventAlgorithm.Execute(movies, availability, new DateTime(2018, 11, 21))));
        }

        [Fact]
        public void TwoMoviesOnTheSameDay()
        {
            List<Movie> movies = new List<Movie>
            {
                new Movie() { Time = 120, Title = "A" },
                new Movie() { Time = 105, Title = "B" },
            };

            Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability = new Dictionary<DayOfWeek, (DateTime?, DateTime?, int)>()
            {
                { DayOfWeek.Monday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 20, 0, 0), 120) },
                { DayOfWeek.Tuesday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 19, 5, 0), 245) },
                { DayOfWeek.Wednesday, (null, null, 0) },
                { DayOfWeek.Thursday, (null, null, 0) },
                { DayOfWeek.Friday, (null, null, 0) },
                { DayOfWeek.Saturday, (null, null, 0) },
                { DayOfWeek.Sunday, (null, null, 0) }
            };

            Assert.Equal($"BEGIN:VCALENDAR\r\n" +
                $"PRODID:-//github.com/rianjs/ical.net//NONSGML ical.net 4.0//EN\r\n" +
                $"VERSION:2.0\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181126T170000\r\n" +
                $"DTSTART:20181126T150000\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:A\r\n" +
                $"END:VEVENT\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181127T164500\r\n" +
                $"DTSTART:20181127T150000\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:B\r\n" +
                $"END:VEVENT\r\n" +
                $"END:VCALENDAR\r\n",
                RemoveNotReproducableElements(eventAlgorithm.Execute(movies, availability, new DateTime(2018, 11, 21))));
        }

        [Fact]
        public void MoviesThatDontFit()
        {
            List<Movie> movies = new List<Movie>
            {
                new Movie() { Time = 400, Title = "A" },
                new Movie() { Time = 500, Title = "B" },
            };

            Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability = new Dictionary<DayOfWeek, (DateTime?, DateTime?, int)>()
            {
                { DayOfWeek.Monday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 20, 0, 0), 120) },
                { DayOfWeek.Tuesday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 19, 5, 0), 245) },
                { DayOfWeek.Wednesday, (null, null, 0) },
                { DayOfWeek.Thursday, (null, null, 0) },
                { DayOfWeek.Friday, (null, null, 0) },
                { DayOfWeek.Saturday, (null, null, 0) },
                { DayOfWeek.Sunday, (null, null, 0) }
            };

            Assert.Equal($"BEGIN:VCALENDAR\r\n" +
                $"PRODID:-//github.com/rianjs/ical.net//NONSGML ical.net 4.0//EN\r\n" +
                $"VERSION:2.0\r\n" +
                $"END:VCALENDAR\r\n",
                eventAlgorithm.Execute(movies, availability, new DateTime(2018, 11, 21)));
        }

        [Fact]
        public void MoviesThatFitAndOneThatDoesnt()
        {
            List<Movie> movies = new List<Movie>
            {
                new Movie() { Time = 120, Title = "A" },
                new Movie() { Time = 105, Title = "B" },
                new Movie() { Time = 700, Title = "C"}
            };

            Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability = new Dictionary<DayOfWeek, (DateTime?, DateTime?, int)>()
            {
                { DayOfWeek.Monday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 20, 0, 0), 120) },
                { DayOfWeek.Tuesday, (new DateTime(2000, 01, 01, 15, 0, 0), new DateTime(2000, 01, 01, 19, 5, 0), 245) },
                { DayOfWeek.Wednesday, (null, null, 0) },
                { DayOfWeek.Thursday, (null, null, 0) },
                { DayOfWeek.Friday, (null, null, 0) },
                { DayOfWeek.Saturday, (null, null, 0) },
                { DayOfWeek.Sunday, (null, null, 0) }
            };

            Assert.Equal($"BEGIN:VCALENDAR\r\n" +
                $"PRODID:-//github.com/rianjs/ical.net//NONSGML ical.net 4.0//EN\r\n" +
                $"VERSION:2.0\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181126T170000\r\n" +
                $"DTSTART:20181126T150000\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:A\r\n" +
                $"END:VEVENT\r\n" +
                $"BEGIN:VEVENT\r\n" +
                $"DESCRIPTION:Seance generated with MPlanner\r\n" +
                $"DTEND:20181127T164500\r\n" +
                $"DTSTART:20181127T150000\r\n" +
                $"SEQUENCE:0\r\n" +
                $"SUMMARY:B\r\n" +
                $"END:VEVENT\r\n" +
                $"END:VCALENDAR\r\n",
                RemoveNotReproducableElements(eventAlgorithm.Execute(movies, availability, new DateTime(2018, 11, 21))));
        }

        private string RemoveNotReproducableElements(string input)
        {
            List<string> output = new List<string>(input.Split("\r\n"));
            output.RemoveAll(x => x.Contains("DTSTAMP:") || x.Contains("UID:"));
            return string.Join("\r\n", output);
        }
    }
}
