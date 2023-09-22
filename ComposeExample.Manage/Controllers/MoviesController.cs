using ComposeExample.MoviesClient;
using Microsoft.AspNetCore.Mvc;

namespace ComposeExample.Manage.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieClient _client;

        public MoviesController(IMovieClient client)
        {
            _client = client;
        }

        // GET: MoviesController
        public async Task<ActionResult> Index()
        {
            var movieResponse = await _client.GetAll();

            if (!movieResponse.IsSuccessStatusCode)
                return View(null);

            return View(movieResponse.Content);
        }

        // GET: MoviesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MoviesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(Movie movie)
        {
            movie.Id = 0;

            var response = await _client.Insert(movie);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(response.Error?.Message))
                ModelState.AddModelError("", response.Error.Message);

            return View(movie);
        }

        // GET: MoviesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var movieResponse = await _client.Get(id);

            if (!movieResponse.IsSuccessStatusCode)
                return NotFound();

            return View(movieResponse.Content);
        }

        // POST: MoviesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id)
                return BadRequest();

            var response = await _client.Update(movie);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(response.Error?.Message))
                ModelState.AddModelError("", response.Error.Message);

            return View(movie);
        }

        // GET: MoviesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var movieResponse = await _client.Get(id);

            if (!movieResponse.IsSuccessStatusCode)
                return NotFound();

            return View(movieResponse.Content);
        }

        // POST: MoviesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Movie movie)
        {
            if (id != movie.Id)
                return BadRequest();

            var response = await _client.Delete(id);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(response.Error?.Message))
                ModelState.AddModelError("", response.Error.Message);

            return View(movie);
        }
    }
}
