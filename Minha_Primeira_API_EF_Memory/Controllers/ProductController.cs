using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using testeef.Data;
using testeef.Models;

namespace testeef.Controllers
{
    [ApiController]
    [Route("produtos")]
    public class ProductController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var produtos = await context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();
            return produtos;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id)
        {
            var produto = await context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return produto;
        }

        [HttpGet]
        [Route("categorias/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            var produtos = await context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();
            return produtos;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post(
        [FromServices] DataContext context,
        [FromBody] Product model)
        {
            if (ModelState.IsValid)
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                //model.Category = GetCategoryById(context.Categories, model.CategoryId);
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
