using System.Linq;
using System.Threading.Tasks;
using AzurePizza01.Data;
using AzurePizza01.DTOs;
using AzurePizza01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzurePizza01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatratterController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public MatratterController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/matratter
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matratter = await _dbContext.Matratter
                .Include(m => m.Ingredients)
                .Select(m => new MatrattDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Description = m.Description,
                    Category = m.Category,
                    Ingredients = m.Ingredients.Select(i => i.Name).ToList()
                })
                .ToListAsync();

            return Ok(matratter);
        }

        // GET: api/matratter/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var m = await _dbContext.Matratter
                .Include(x => x.Ingredients)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (m == null)
                return NotFound();

            var dto = new MatrattDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                Description = m.Description,
                Category = m.Category,
                Ingredients = m.Ingredients.Select(i => i.Name).ToList()
            };
            return Ok(dto);
        }

        // POST: api/matratter
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MatrattDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matratt = new Matratt
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Category = model.Category,
                Ingredients = model.Ingredients.Select(n => new Ingredient { Name = n }).ToList()
            };

            await _dbContext.Matratter.AddAsync(matratt);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = matratt.Id }, model);
        }

        // PUT: api/matratter/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MatrattDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _dbContext.Matratter
                .Include(m => m.Ingredients)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existing == null)
                return NotFound();

            existing.Name = model.Name;
            existing.Price = model.Price;
            existing.Description = model.Description;
            existing.Category = model.Category;

            // Radera gamla ingredienser:
            _dbContext.Ingredients.RemoveRange(existing.Ingredients);

            // Lägg in nya ingredienser:
            existing.Ingredients = model.Ingredients.Select(n => new Ingredient { Name = n }).ToList();

            _dbContext.Matratter.Update(existing);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/matratter/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _dbContext.Matratter
                .Include(m => m.Ingredients)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existing == null)
                return NotFound();

            // EF tar bort ingredienser automatiskt via kaskad eller så kan du göra:
            _dbContext.Ingredients.RemoveRange(existing.Ingredients);
            _dbContext.Matratter.Remove(existing);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
