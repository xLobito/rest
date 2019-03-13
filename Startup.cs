using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CarreraContext>(opt => opt.UseInMemoryDatabase("XE"));
            services.AddDbContext<UsuarioContext>(opt => opt.UseInMemoryDatabase("XE"));
            services.AddDbContext<IngresoContext>(opt => opt.UseInMemoryDatabase("XE"));
            services.AddDbContext<CursoContext>(opt => opt.UseInMemoryDatabase("XE")); 
            services.AddDbContext<InscritoContext>(opt => opt.UseInMemoryDatabase("XE")); 
            services.AddDbContext<NotaContext>(opt => opt.UseInMemoryDatabase("XE"));     
            services.AddDbContext<AvisoContext>(opt => opt.UseInMemoryDatabase("XE"));      
            services.AddDbContext<ModuloContext>(opt => opt.UseInMemoryDatabase("XE"));  
            services.AddDbContext<BeneficioContext>(opt => opt.UseInMemoryDatabase("XE"));   
            services.AddDbContext<BecadoContext>(opt => opt.UseInMemoryDatabase("XE"));      
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
