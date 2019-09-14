using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VDMP.DataAccess;

namespace VDMP.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "(localdb)\\MSSQLLocalDB",
                InitialCatalog = "VDMP",
                IntegratedSecurity = true
            };
            services.AddDbContext<VDMPContext>(opt =>
                opt.UseSqlServer(builder.ConnectionString, b => b.MigrationsAssembly("VDMP.Api")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}