using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MPlanner.Data;
using MPlanner.Models;
using MPlanner.Services;

namespace MPlanner.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IEventAlgorithm _eventAlgorithm;

        public MoviesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEventAlgorithm eventAlgorithm)
        {
            _context = context;
            _userManager = userManager;
            _eventAlgorithm = eventAlgorithm;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var user = GetCurrentUserAsync().Result;
            return View(await _context.Movie.Where(x => x.UserId == user.Id).OrderBy(x => x.Position).ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Genre,Time,Director,Year,Actors,Description")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUserAsync().Result;
                movie.UserId = user.Id;
                movie.Position = _context.Movie.Where(x => x.UserId == user.Id).Count();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Genre,Time,Director,Year,Actors,Description")] Movie movie)
        {
            var currentUser = GetCurrentUserAsync();

            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                movie.UserId = currentUser.Result.Id; 
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
        public async Task<IActionResult> Find([Bind("Genre,MinTime,MaxTime,Director")] SearchDataModel searchData)
        {
            var user = GetCurrentUserAsync().Result;
            List<Movie> movies = await _context.Movie.Where(x => x.UserId == user.Id
                && (searchData.Genre != null ? x.Genre == searchData.Genre : true)
                && (searchData.MinTime != null ? x.Time >= searchData.MinTime : true)
                && (searchData.MaxTime != null ? x.Time <= searchData.MaxTime : true)
                && (searchData.Director != null ? x.Director == searchData.Director : true)).ToListAsync();

            if (movies.Count == 0)
                return View("MovieNotFound");

            Movie result = movies.OrderBy(x => x.Position).ElementAt(0);
            return View("MovieFound", result);
        }

        public IActionResult Export()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export([Bind("MondayStartTime,MondayEndTime,TuesdayStartTime,TuesdayEndTime," +
            "WednesdayStartTime,WednesdayEndTime,ThursdayStartTime,ThursdayEndTime,FridayStartTime,FridayEndTime," +
            "SaturdayStartTime,SaturdayEndTime,SundayStartTime,SundayEndTime")] ExportDataModel exportData)
        {
            var user = GetCurrentUserAsync().Result;
            List<Movie> movies = await _context.Movie.Where(x => x.UserId == user.Id)
                .OrderBy(x => x.Position).ToListAsync();

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

            return File(System.Text.Encoding.ASCII.GetBytes(_eventAlgorithm.Execute(movies, availability)), "application/octet-stream", "mplan.ical");
        }

        // GET: Movies/MoveUp/5
        public async Task<IActionResult> MoveUp(int? id)
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

            if (movie.Position != 0)
            {
                var user = GetCurrentUserAsync().Result;
                List<Movie> movies = await _context.Movie.Where(x => x.UserId == user.Id).OrderBy(x => x.Position).ToListAsync();
                Movie previousMovie = movies[movies.IndexOf(movie) - 1];

                SwapPositions(movie, previousMovie);
                _context.Update(movie);
                _context.Update(previousMovie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/MoveUp/5
        public async Task<IActionResult> MoveDown(int? id)
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

            var user = GetCurrentUserAsync().Result;
            List<Movie> movies = await _context.Movie.Where(x => x.UserId == user.Id).OrderBy(x => x.Position).ToListAsync();
            if (movie.Position != movies.Count - 1)
            {
                Movie nextMovie = movies[movies.IndexOf(movie) + 1];

                SwapPositions(movie, nextMovie);
                _context.Update(movie);
                _context.Update(nextMovie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void SwapPositions(Movie m1, Movie m2)
        {
            int position = m1.Position;
            m1.Position = m2.Position;
            m2.Position = position;
        }

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
