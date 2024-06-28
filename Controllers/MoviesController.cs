using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppMovieTop.Models;

namespace WebAppMovieTop.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _environment;

        public MoviesController(MovieContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View(_context.Movies.ToList());
        }

        public IActionResult Create()
        {
            Console.WriteLine("Create action called.");

            
            var testFilePath = Path.Combine(_environment.WebRootPath, "uploads", "test.txt");
            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "uploads")))
            {
                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "uploads"));
                Console.WriteLine("Uploads directory created in Create action.");
            }
            System.IO.File.WriteAllText(testFilePath, "This is a test file.");
            Console.WriteLine($"Test file created at: {testFilePath}");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var movie = new Movie
                    {
                        Title = model.Title,
                        Director = model.Director,
                        Genre = model.Genre,
                        Year = model.Year,
                        Description = model.Description
                    };

                    if (model.Poster != null)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "uploads");

                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        var uniqueFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(model.Poster.FileName);
                        var filePath = Path.Combine(uploads, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Poster.CopyToAsync(fileStream);
                        }

                        movie.PosterPath = "/uploads/" + uniqueFileName;
                    }

                    _context.Movies.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the movie.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                   
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var model = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Director = movie.Director,
                Genre = movie.Genre,
                Year = movie.Year,
                PosterPath = movie.PosterPath,
                Description = movie.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MovieViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var movie = await _context.Movies.FindAsync(id);
                    if (movie == null)
                    {
                        return NotFound();
                    }

                    movie.Title = model.Title;
                    movie.Director = model.Director;
                    movie.Genre = model.Genre;
                    movie.Year = model.Year;
                    movie.Description = model.Description;

                    if (model.Poster != null)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "uploads");

                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        var uniqueFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(model.Poster.FileName);
                        var filePath = Path.Combine(uploads, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Poster.CopyToAsync(fileStream);
                        }

                        movie.PosterPath = "/uploads/" + uniqueFileName;
                    }
                    else
                    {
                        movie.PosterPath = model.PosterPath;
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while editing the movie.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
    }
}