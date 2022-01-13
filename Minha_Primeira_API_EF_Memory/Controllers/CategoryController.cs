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
    [Route("categorias")]
    public class CategoryController : ControllerBase
    {
        //private DataContext _dataContext;   

        //public CategoryController(DataContext dataContext)
        //{
        //    _dataContext = dataContext;
        //}

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext dataContext)
        {
            var categories = await dataContext.Categories.AsNoTracking().ToListAsync();
            return categories;
        }

        [HttpGet]
        [Route("")]
        private async Task<ActionResult<List<Category>>> GetOrderByDesc([FromServices] DataContext dataContext)
        {
            var categories = await dataContext.Categories.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
            return categories;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById([FromServices] DataContext context, int id)
        {
            var categoria = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return categoria;
        }

        [HttpGet]
        [Route("{id:int}")]
        private async Task<ActionResult<bool>> VerificaSeExiste([FromServices] DataContext context, int id)
        {
            bool existe = await context.Categories.AnyAsync(x => x.Id == id);
            return existe;
        }

        ////retorna o último inserido
        //[HttpPost]
        //[Route("")]
        //public async Task<ActionResult<Category>> PostRetornaUltimo(
        //    [FromServices] DataContext context,
        //    [FromBody] Category model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //System.Guid g = System.Guid.NewGuid();
        //        //model.Id = g.ToString();
        //        context.Categories.Add(model);
        //        await context.SaveChangesAsync();
        //        return model;
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }
        //}

        //retorna uma lista de todos em ordem decrescente.
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Post([FromServices] DataContext context, [FromBody] Category model)
        {
            if (ModelState.IsValid)
            {
                //System.Guid g = System.Guid.NewGuid();
                //model.Id = g.ToString();
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return await GetOrderByDesc(context);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Route("alterar/{id:int}")]
        public async Task<ActionResult<Category>> Alterar([FromServices] DataContext context, [FromBody] Category model, int id)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            bool existe = await context.Categories.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var categoria = await context.Categories.FindAsync(id);
                if (categoria == null)
                {
                    //return BadRequest();
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var retorno = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return retorno;
        }


        [HttpDelete("{id}")]
        [Route("deletar/{id:int}")]
        public async Task<IActionResult> Deletar([FromServices] DataContext context, int id)
        {
            var categoria = await context.Categories.FindAsync(id);
            if (categoria == null)
            {
                //return BadRequest();
                return NotFound();
            }

            context.Categories.Remove(categoria);
            await context.SaveChangesAsync();

            //return NoContent();
            return Ok();
        }
    }
}