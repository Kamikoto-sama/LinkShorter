using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkShorter.WebApi.Utils
{
    public class FileStorageCleaner: IHostedService
    {
        private const string FileStorageDir = "~/storage";
        private readonly TimeSpan checkInterval;
        private readonly TimeSpan fileExpatriationTime;
        private readonly ILogger<FileStorageCleaner> logger;
        private readonly CancellationTokenSource cancellationTokenSource;

        public FileStorageCleaner(FileStorageCleanerSettings settings, ILogger<FileStorageCleaner> logger)
        {
            this.logger = logger;
            checkInterval = TimeSpan.FromDays(settings.CheckIntervalDays);
            fileExpatriationTime = TimeSpan.FromDays(settings.FileExpirationDays);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        private async void Run()
        {
            var ct = cancellationTokenSource.Token;
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(checkInterval, ct);
                Clean(FileStorageDir);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();
            return Task.CompletedTask;
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