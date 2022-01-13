using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using testeef.Data;
using testeef.Models;

namespace testeef.Controllers
{
    [ApiController] //definindo para a classe Contoller, que usaremos o ApiController
    [Route("categorias")] //Como não definimos nenhuma rota no endPoints(Startup.cs), o mapeamento das rotas serão pela rotas do controller atual
    public class CategoryController : ControllerBase
    {
        ///Metódo tradicional onde tinhamos a variável DataContext que era repassado no contrutor da classe.
        ///Esta maneira deixou de ser usado na nova versão no .net core,
        ///deixou o código mais simples
        //private DataContext _dataContext;   

        //public CategoryController(DataContext dataContext)
        //{
        //    _dataContext = dataContext;
        //}

        /// <summary>
        /// Retorna uma lista de Categorias, usando o Task de forma assíncrona. 
        /// </summary>
        /// <param name="dataContext">Acessar os dados. o "[FromServices]" indica que vai utilizar o DataContext que já está em memória</param>
        /// <returns>Retorna uma lista de Categorias</returns>
        [HttpGet]//definindo o verbo utilizado. Se não colocar nada, por padrão ele assume o GET
        [Route("")]//rota vazia, ou seja, será a mesma rota definida no controller
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext dataContext)
        {
            var categories = await dataContext.Categories.AsNoTracking().ToListAsync();
            return categories;
        }

        /// <summary>
        /// Retorna uma Lista de Categorias em ordem Decrescente.
        /// É um método privado, usado internamente.
        /// </summary>
        /// <param name="dataContext">Acessar os dados. o "[FromServices]" indica que vai utilizar o DataContext que já está em memória</param>
        /// <returns>Retorna uma lista de Categorias em ordem Descrescente</returns>
        [HttpGet]
        [Route("")]
        private async Task<ActionResult<List<Category>>> GetOrderByDesc([FromServices] DataContext dataContext)
        {
            var categories = await dataContext.Categories.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
            return categories;
        }

        /// <summary>
        /// Retorna uma Categoria informando o Id da categoria desejada
        /// </summary>
        /// <param name="context">Acessar os dados. o "[FromServices]" indica que vai utilizar o DataContext que já está em memória</param>
        /// <param name="id">Informe o Id da categoria desejado</param>
        /// <returns>Retorna uma Categoria</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById([FromServices] DataContext context, int id)
        {
            var categoria = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return categoria;
        }

        /// <summary>
        /// Verifica se o Id da Categoria existe na base de dados.
        /// </summary>
        /// <param name="context">Acessar os dados. o "[FromServices]" indica que vai utilizar o DataContext que já está em memória</param>
        /// <param name="id">Informe o Id da Categoria desejado.</param>
        /// <returns>Retorna "Verdadeiro / Falso"</returns>
        [HttpGet]
        [Route("{id:int}")]
        private async Task<ActionResult<bool>> VerificaSeExiste([FromServices] DataContext context, int id)
        {
            bool existe = await context.Categories.AnyAsync(x => x.Id == id);
            return existe;
        }

        ///// <summary>
        ///// Retorna o último objeto inserido.
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="model"></param>
        ///// <returns></returns>
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

        /// <summary>
        /// Grava na base uma nova Categoria.
        /// E retorna toda a lista das Categorias já cadastradas.
        /// </summary>
        /// <param name="context">Representação do banco de dados em memória</param>
        /// <param name="model">Modelo via json</param>
        /// <returns>Retorna uma lista de todos.</returns>        
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

        /// <summary>
        /// Altera os dados da Categoria atual
        /// </summary>
        /// <param name="context">Representação do banco de dados</param>
        /// <param name="model">Dados a ser alterado no modelo json pelo body</param>
        /// <param name="id">Informe o Id da Categoria desejada</param>
        /// <returns>Retorna a categoria já alterada.</returns>
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

        /// <summary>
        /// Deleta/Remove a Categoria repassando via "Rota" o id desejado para exclusão
        /// </summary>
        /// <param name="context">Representação do nosso banco de dados</param>
        /// <param name="id">Informe o Id desejado para exclusão</param>
        /// <returns>Não retorna nada.</returns>
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