using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using testeef.Data;

namespace testeef
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Informar para nossa aplicação que temos um DataContext, 
        /// e que vamos trabalhar com o Entity Framework,
        /// e que usaremos o nosso banco de dados em Memória.
        /// Estamos dando o nome para o nosso banco de dados de "DataBasw"
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("DataBase"));
            
            //Deixar no DataContext disponível através do "addScoped", que é injeção de dependência do .net Core.
            //Significa que, se em algum lugar da nossa aplicação solicitar o DataContext e vai deixar em memória, 
            //onde não cria uma nova versão toda vez que requisitar, ou seja, não vai abrir uma nova conexão no banco.
            //e assim que a requisição terminar, a aplicação vai destruir o DataContext, para não deixar vestígios na memória, liberando recurso 
            services.AddScoped<DataContext, DataContext>();
            services.AddControllers();
            //services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
