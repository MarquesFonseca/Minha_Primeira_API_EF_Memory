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
        /// Informar para nossa aplica��o que temos um DataContext, 
        /// e que vamos trabalhar com o Entity Framework,
        /// e que usaremos o nosso banco de dados em Mem�ria.
        /// Estamos dando o nome para o nosso banco de dados de "DataBasw"
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("DataBase"));
            
            //Deixar no DataContext dispon�vel atrav�s do "addScoped", que � inje��o de depend�ncia do .net Core.
            //Significa que, se em algum lugar da nossa aplica��o solicitar o DataContext e vai deixar em mem�ria, 
            //onde n�o cria uma nova vers�o toda vez que requisitar, ou seja, n�o vai abrir uma nova conex�o no banco.
            //e assim que a requisi��o terminar, a aplica��o vai destruir o DataContext, para n�o deixar vest�gios na mem�ria, liberando recurso 
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
