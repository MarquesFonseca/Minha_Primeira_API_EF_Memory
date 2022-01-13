using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Minha_Primeira_API_EF_Memory.Data;
using Minha_Primeira_API_EF_Memory.Models;

namespace Minha_Primeira_API_EF_Memory.Controllers
{
    [ApiController]
    [Route("produtos")]
    public class ProductController : Controller
    {
        /// <summary>
        /// Retorna todos os produtos
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Produtos</returns>
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

        /// <summary>
        /// Retorna todos os produtos de uma Categoria específica
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns>Retorna uma lista de produtos de uma mesma categoria.</returns>
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

        /// <summary>
        /// Insere um novo Produto
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns>Retorna o produto recém inserido.</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post(
        [FromServices] DataContext context,
        [FromBody] Product model)
        {
            bool existeACategoriaInformada = await context.Categories.AnyAsync(x => x.Id == model.CategoryId);
            if (existeACategoriaInformada)//vai inserir um novo produto se existir a categoria informada.
            {
                if (ModelState.IsValid)
                {
                    context.Products.Add(model);
                    await context.SaveChangesAsync();
                    var categoria = await context.Categories.FindAsync(model.Id);
                    model.Category = categoria;
                    return model;
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
