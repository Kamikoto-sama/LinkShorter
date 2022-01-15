using LinkShorter.Helpers;
using LinkShorter.Models;
using LinkShorter.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LinkShorter
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StorageContext>(builder => builder.UseSqlite(configuration.GetConnectionString("sqlite")));

            services.Configure<AuthSettings>(configuration.GetSection(nameof(AuthSettings)));
            services.Configure<CustomTagsSettings>(configuration.GetSection(nameof(CustomTagsSettings)));
            services.Configure<ExportSettings>(configuration.GetSection(nameof(ExportSettings)));
            services.AddSingleton<FileStorageCleanerSettings>();

            services.AddScoped<AccessKeyProvider>();
            services.AddScoped<VisitManager>();
            services.AddScoped<ExportManager>();
            services.AddScoped<LinkManager>();

            var cleanerSettings = configuration.GetSection(nameof(FileStorageCleanerSettings)).Get<FileStorageCleanerSettings>();
            services.AddSingleton(cleanerSettings);
            services.AddSingleton<FileStorageCleaner>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var storage = scope.ServiceProvider.GetRequiredService<StorageContext>();
                // storage.Database.EnsureDeleted();
                storage.Database.EnsureCreated();
            }

            app.ApplicationServices.GetRequiredService<FileStorageCleaner>().Start();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}