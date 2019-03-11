﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace ConsoleExample
{
    internal class Program
    {
        private static void Main()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var servicesProvider = BuildDi();
                using (servicesProvider as IDisposable)
                {
                    var runner = servicesProvider.GetRequiredService<Runner>();
                    runner.DoAction("Action1");

                    //Console.WriteLine("Press ANY key to exit");
                    //Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
                // moved here so it appears after the log console output
                Console.WriteLine("Press ANY key to exit");
                Console.ReadKey();
            }
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            // Runner is the custom class
            services.AddTransient<Runner>();

            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // configure Logging with NLog
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }

    public class Runner
    {
        private readonly ILogger<Runner> _logger;

        public Runner(ILogger<Runner> logger)
        {
            _logger = logger;
        }

        public void DoAction(string name)
        {
            _logger.LogDebug(20, "LogDebug {Action}", name);
            _logger.LogInformation(21, "LogInformation {Action}", name);
            _logger.LogWarning(22, "LogWarning {Action}", name);
            _logger.LogError(23, "LogError {Action}", name);
            _logger.LogCritical(24, "LogCritical {Action}", name);
        }
    }
}