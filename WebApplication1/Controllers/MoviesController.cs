using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Cinema;

public class MoviesController : Controller
{
    private readonly CinemaDbContext _context;

    public MoviesController(CinemaDbContext context)
    {
        _context = context;
    }

    // GET: /Movies — Liste avec le producteur inclus
    public async Task<IActionResult> Index()
    {
        var movies = _context.Movies.Include(m => m.Producer);
        return View(await movies.ToListAsync());
    }

    // GET: /Movies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var movie = await _context.Movies
            .Include(m => m.Producer)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    // GET: /Movies/Create
    public IActionResult Create()
    {
        // Charge la liste des producteurs pour la dropdown
        ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Name");
        return View();
    }

    // POST: /Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Genre,ProducerId")] Movie movie)
    {
        ModelState.Remove("Producer");

        if (ModelState.IsValid)
        {
            _context.Add(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Name", movie.ProducerId);
        return View(movie);
    }

    // GET: /Movies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();
        ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Name", movie.ProducerId);
        return View(movie);
    }

    // POST: /Movies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ProducerId")] Movie movie)
    {
        if (id != movie.Id) return NotFound();

        ModelState.Remove("Producer");

        if (ModelState.IsValid)
        {
            _context.Update(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Name", movie.ProducerId);
        return View(movie);
    }

    // GET: /Movies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var movie = await _context.Movies
            .Include(m => m.Producer)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    // POST: /Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie != null) _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // Jointure Film -> Producteur via propriété de navigation
    public IActionResult MoviesAndTheirProds()
    {
        // Include() charge la propriété de navigation Producer
        var movies = _context.Movies
                            .Include(m => m.Producer)
                            .ToList();
        return View(movies);
    }
    public IActionResult MoviesAndTheirProds_UsingModel()
    {
        var result = (from m in _context.Movies
                      join p in _context.Producers
                      on m.ProducerId equals p.Id
                      select new ProdMovie
                      {
                          mTitle = m.Title,
                          mGenre = m.Genre,
                          pName = p.Name,
                          pNat = p.Nationality
                      }).ToList();
        return View(result);
    }
    public IActionResult SearchByTitle(string Critere)
    {
        var movies = (from m in _context.Movies
                      where m.Title.Contains(Critere ?? "")
                      select m).ToList();
        return View(movies);
    }

    public IActionResult SearchByGenre(string Critere)
    {
        var movies = (from m in _context.Movies
                      where m.Genre.Contains(Critere ?? "")
                      select m).ToList();
        return View(movies);
    }

    public IActionResult SearchBy2(string genre, string title)
    {
        var genres = _context.Movies
                            .Select(m => m.Genre)
                            .Distinct()
                            .ToList();
        genres.Insert(0, "All");
        ViewBag.Genres = new SelectList(genres);

        var movies = _context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(genre) && genre != "All")
            movies = movies.Where(m => m.Genre.Contains(genre));

        if (!string.IsNullOrEmpty(title))
            movies = movies.Where(m => m.Title.Contains(title));

        return View(movies.ToList());
    }
}
