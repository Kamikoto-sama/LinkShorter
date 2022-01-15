using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkShorter.Helpers
{
    public class FileStorageCleaner
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<FileStorageCleaner> logger;
        private readonly TimeSpan checkInterval;
        private readonly TimeSpan fileExpatriationTime;

        public FileStorageCleaner(FileStorageCleanerSettings settings, IWebHostEnvironment env, ILogger<FileStorageCleaner> logger)
        {
            this.env = env;
            this.logger = logger;
            checkInterval = TimeSpan.FromDays(settings.CheckIntervalDays);
            fileExpatriationTime = TimeSpan.FromDays(settings.FileExpirationDays);
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
            if (!Directory.Exists(fileStorageDir))
                return;

            var files = Directory.GetFiles(fileStorageDir)
                .Where(file => DateTime.UtcNow - File.GetCreationTimeUtc(file) > fileExpatriationTime)
                .ToList();
            foreach (var file in files)
                File.Delete(file);
            if (files.Count > 0)
                logger.LogInformation($"{files.Count} files have been deleted from storage due to expiration: {string.Join(", ", files.Select(Path.GetFileName))}");
            foreach (var directory in Directory.GetDirectories(fileStorageDir))
                Clean(directory);
        }
    }

    public class FileStorageCleanerSettings
    {
        public int FileExpirationDays { get; init; }
        public int CheckIntervalDays { get; init; }
    }
}