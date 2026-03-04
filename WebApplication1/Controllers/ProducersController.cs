using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Cinema;

namespace WebApplication1.Controllers
{
    public class ProducersController : Controller
    {
        // === Injection de dépendance du contexte ===
        CinemaDbContext _context;
        public ProducersController(CinemaDbContext context)
        {
            _context = context;
        }

        // ========== A. INDEX — Lister les producteurs ==========
        public IActionResult Index()
        {
            var producers = _context.Producers.ToList();
            return View(producers);
        }

        // ========== E. DETAILS — Détailler un producteur ==========
        public IActionResult Details(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        // ========== B. CREATE GET — Formulaire de création ==========
        public IActionResult Create()
        {
            return View();  // *** Garder telle quelle ***
        }

        // ========== B. CREATE POST — Insérer dans la BD ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producer producer)
        {
            ModelState.Remove("Movies");

            if (!ModelState.IsValid)
            {
                // Afficher TOUTES les erreurs dans la page
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToList();

                foreach (var error in errors)
                {
                    foreach (var e in error.Errors)
                    {
                        ViewBag.DebugError += $"Champ: {error.Key} | Erreur: {e.ErrorMessage} | ";
                    }
                }
                return View(producer);
            }

            _context.Producers.Add(producer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ========== C. EDIT GET — Formulaire de modification ==========
        public IActionResult Edit(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        // ========== C. EDIT POST — Mettre à jour dans la BD ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producer producer)
        {
            if (ModelState.IsValid)
            {
                _context.Producers.Update(producer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        // ========== D. DELETE GET — Confirmation de suppression ==========
        public IActionResult Delete(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        // ========== D. DELETE POST — Supprimer de la BD ==========
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer != null)
            {
                _context.Producers.Remove(producer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ProdsAndTheirMovies()
        {
            // Include() charge la collection Movies de chaque Producer
            var producers = _context.Producers
                                   .Include(p => p.Movies)
                                   .ToList();
            return View(producers);
        }
        public IActionResult ProdsAndTheirMovies_UsingModel()
        {
            var result = (from p in _context.Producers
                          join m in _context.Movies
                          on p.Id equals m.ProducerId
                          select new ProdMovie
                          {
                              pId = p.Id,
                              pName = p.Name,
                              pNat = p.Nationality,
                              mTitle = m.Title,
                              mGenre = m.Genre
                          }).ToList();
            return View(result);
        }
        public IActionResult MyMovies(int id)
        {
            var result = (from p in _context.Producers
                          join m in _context.Movies
                          on p.Id equals m.ProducerId
                          where p.Id == id   // <- filtre sur l'ID
                          select new ProdMovie
                          {
                              pName = p.Name,
                              pNat = p.Nationality,
                              mTitle = m.Title,
                              mGenre = m.Genre
                          }).ToList();
            return View(result);
        }



    }
}
