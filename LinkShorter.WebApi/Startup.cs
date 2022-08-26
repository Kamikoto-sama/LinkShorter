using LinkShorter.Core;
using LinkShorter.Export;
using LinkShorter.Export.Models;
using LinkShorter.WebApi.Models;
using LinkShorter.WebApi.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LinkShorter.WebApi
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(builder =>
            {
                if (env.IsDevelopment())
                    builder.UseSqlite(configuration.GetConnectionString("sqlite"));
                else
                    builder.UseNpgsql(configuration.GetConnectionString("postgres"));
            });

            services.Configure<AuthSettings>(configuration.GetSection(nameof(AuthSettings)));
            services.Configure<CustomTagsSettings>(configuration.GetSection(nameof(CustomTagsSettings)));
            services.Configure<ExportSettings>(configuration.GetSection(nameof(ExportSettings)));

            services.AddScoped<AccessKeyProvider>();
            services.AddScoped<VisitManager>();
            services.AddScoped<ExportManager>();
            services.AddScoped<LinkManager>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var storage = scope.ServiceProvider.GetRequiredService<DataContext>();
                storage.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}