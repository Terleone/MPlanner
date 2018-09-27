using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MPlanner.Data;
using MPlanner.Models;

namespace MPlanner.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.Where(x => x.UserName == User.Identity.Name).ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Genre,Time,Director,Year,Actors,Description")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.UserName = User.Identity.Name;
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Genre,Time,Director,Year,Actors,Description")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.MovieId == id);
        }

        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find([Bind("Genre,MinTime,MaxTime,Director")] SearchData searchData)
        {
            List<Movie> movies = await _context.Movie.Where(x => x.UserName == User.Identity.Name
                && (searchData.Genre != null ? x.Genre == searchData.Genre : true)
                && (searchData.MinTime != null ? x.Time >= searchData.MinTime : true)
                && (searchData.MaxTime != null ? x.Time <= searchData.MaxTime : true)
                && (searchData.Director != null ? x.Director == searchData.Director : true)).ToListAsync();

            if (movies.Count == 0)
                return View("MovieNotFound");

            Movie result = movies.OrderBy(x => x.MovieId).ElementAt(0);
            return View("MovieFound", result);
        }

        public IActionResult Export()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export([Bind("MondayStartTime,MondayEndTime,TuesdayStartTime,TuesdayEndTime," +
            "WednesdayStartTime,WednesdayEndTime,ThurdsayStartTime,ThursdayEndTime,FridayStartTime,FridayEndTime," +
            "SaturdayStartTime,SaturdayEndTime,SundayStartTime,SundayEndTime")] ExportData exportData)
        {
            List<Movie> movies = await _context.Movie.Where(x => x.UserName == User.Identity.Name)
                .OrderBy(x => x.MovieId).ToListAsync();

            Dictionary<DayOfWeek, (DateTime? startTime, DateTime? endTime, int amount)> availability = new Dictionary<DayOfWeek, (DateTime?, DateTime?, int)>()
            {
                { DayOfWeek.Monday, (exportData.MondayStartTime, exportData.MondayEndTime, exportData.MondayAmount) },
                { DayOfWeek.Tuesday, (exportData.TuesdayStartTime, exportData.TuesdayEndTime, exportData.TuesdayAmount) },
                { DayOfWeek.Wednesday, (exportData.WednesdayStartTime, exportData.WednesdayEndTime, exportData.WednesdayAmount) },
                { DayOfWeek.Thursday, (exportData.ThursdayStartTime, exportData.ThursdayEndTime, exportData.ThursdayAmount) },
                { DayOfWeek.Friday, (exportData.FridayStartTime, exportData.FridayEndTime, exportData.FridayAmount) },
                { DayOfWeek.Saturday, (exportData.SaturdayStartTime, exportData.SaturdayEndTime, exportData.SaturdayAmount) },
                { DayOfWeek.Sunday, (exportData.SundayStartTime, exportData.SundayEndTime, exportData.SundayAmount) }
            };

            List<Movie> notMappedMovies = new List<Movie>();
            Calendar calendar = new Calendar();

            DateTime iterator = DateTime.Now;
            DateTime? lastMapping = null;
            foreach (Movie movie in movies)
            {
                CalendarEvent calEvent = null;
                for (int i = 1; i < 8; i++)
                {
                    iterator = iterator.AddDays(i);
                    var dayInfo = availability[iterator.DayOfWeek];
                    if (dayInfo.amount >= movie.Time)
                    {
                        DateTime startTime = new DateTime(iterator.Year, iterator.Month, iterator.Day, dayInfo.startTime.Value.Hour,
                            dayInfo.startTime.Value.Minute, 0);
                        DateTime endTime = new DateTime(iterator.Year, iterator.Month, iterator.Day, dayInfo.endTime.Value.Hour,
                            dayInfo.endTime.Value.Minute, 0);

                        calEvent = new CalendarEvent
                        {
                            Start = new CalDateTime(startTime),
                            End = new CalDateTime(endTime)//,
                            //Name = movie.Title
                        };

                        calendar.Events.Add(calEvent);
                    }

                    if (calEvent == null)
                    {
                        iterator = lastMapping ?? DateTime.Now;
                        notMappedMovies.Add(movie);
                    }
                }
            }

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            return File(System.Text.Encoding.ASCII.GetBytes(serializedCalendar), "application/octet-stream", "mplan.ical");
        }
    }
}
