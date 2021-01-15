using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MonitorFolderService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _emailSender;
        private FileSystemWatcher watcher;
        private readonly string directory = @"C:\Users\samja\Desktop\Escape";

        public Worker(ILogger<Worker> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher
            {
                Path = directory
            };
            watcher.Created += OnChanged;
            return base.StartAsync(cancellationToken);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            SendEmail(e.FullPath);
        }

        private void SendEmail(string fullPath)
        {
            _logger.LogInformation("A new message about to be sent at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] {
                "bekzatakhmetov02@gmail.com"}, "Hiiii baby", "This is the content for the SE-1907", fullPath);
            _emailSender.SendEmail(message);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                watcher.EnableRaisingEvents = true; //starts listening 
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }



    }
}
