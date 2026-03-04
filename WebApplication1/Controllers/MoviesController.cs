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

}
