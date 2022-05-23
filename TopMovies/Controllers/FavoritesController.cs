using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TopMovies.Dtos;
using TopMovies.Models;

namespace TopMovies.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FavoritesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<int> favs = GetFavList();
            List<Movie> movies = _db.Movies
                .Where(x => favs.Contains(x.Id))
                .ToList();
            return View(movies);
        }

        [HttpPost]
        public IActionResult Toggle(int movieId)
        {
            List<int> favs = GetFavList();
            bool favorited;

            if (favs.Contains(movieId))
            {
                favs.Remove(movieId);
                favorited = false;
            }
            else
            {
                favs.Add(movieId);
                favorited = true;
            }

            SaveFavList(favs);

            return Json(new FavoriteToggleResult() { Favorited = favorited });
        }

        private void SaveFavList(List<int> favorites)
        {
            string favs = string.Join('-', favorites);
            HttpContext.Response.Cookies.Append("favs", favs, new CookieOptions() { Expires = DateTime.Now.AddYears(10) });
        }

        private List<int> GetFavList()
        {
            var favs = HttpContext.Request.Cookies["favs"];

            if (string.IsNullOrEmpty(favs)) return new List<int>();

            try
            {
                return favs.Split('-').Select(s => Convert.ToInt32(s)).ToList();
            }
            catch (Exception)
            {
                return new List<int>();
            }
        }
    }
}
