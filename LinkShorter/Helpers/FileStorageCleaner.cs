using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace LinkShorter.Helpers
{
    public class FileStorageCleaner
    {
        private readonly IWebHostEnvironment env;
        private readonly TimeSpan checkInterval;
        private readonly TimeSpan fileExpatriation;

        public FileStorageCleaner(FileStorageCleanerSettings settings, IWebHostEnvironment env)
        {
            this.env = env;
            checkInterval = TimeSpan.FromDays(settings.CheckIntervalDays);
            fileExpatriation = TimeSpan.FromDays(settings.FileExpatriationDays);
        }

        public async void Start()
        {
            var fileStorageDir = Path.Combine(env.WebRootPath, "storage");
            while (true)
            {
                await Task.Delay(checkInterval);
                Clean(fileStorageDir);
            }
        }

        private void Clean(string fileStorageDir)
        {
            var files = Directory.GetFiles(fileStorageDir);
        }
    }

    public record FileStorageCleanerSettings
    {
        public int FileExpatriationDays { get; init; }
        public int CheckIntervalDays { get; init; }
    }
}