using Microsoft.EntityFrameworkCore;
using Minha_Primeira_API_EF_Memory.Models;

namespace Minha_Primeira_API_EF_Memory.Data
{
    /// <summary>
    /// Classe DataContext (Representação do nosso banco de dados, que usaremos o banco em memória.)
    /// Erda o DBContext
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Aqui também usamos a conectionString(conexão com o banco, SQLServer, Oracle, Mysql, etc)
        /// </summary>
        /// <param name="options">Nesse exemplo não repassaremos nenhuma opção para o DataContext.</param>
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
            { }

        /// <summary>
        /// Coleções de tabelas do nosso banco de dados. Aqui definimos quais tabelas vamos usar
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
