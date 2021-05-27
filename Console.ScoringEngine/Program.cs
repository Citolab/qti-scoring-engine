using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Console.Scoring
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var argList = new List<string>();
            var packageLocationFromArgs = "";
            var assessmentLocationFromArgs = "";

            if (args != null)
            {
                argList = args.ToList();
                if (argList.FirstOrDefault(a => a.Replace("-", "").ToLower() == "help") != null)
                {
                    System.Console.WriteLine(@"dotnet run Console.ScoringEngine c:/mypackage.zip c:/assessmentResults");
                } if (argList.Count == 2)
                {
                    packageLocationFromArgs = argList[0];
                    assessmentLocationFromArgs = argList[1];
                }
            }
            try
            {
                var config = new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.local.json", optional: true)
                   .Build();
                // TODO get package location and assessmentResults from args
                var processingSettings = new ProcessingSetting
                {
                    PackageFileLocation = !string.IsNullOrEmpty(packageLocationFromArgs) ? packageLocationFromArgs : config.GetValue<string>("AppSettings:PackageLocation"),
                    AssessmentResultFolderLocation = !string.IsNullOrEmpty(assessmentLocationFromArgs) ? assessmentLocationFromArgs :  config.GetValue<string>("AppSettings:AssessmentResultFolder")
                };
                var servicesProvider = BuildDi(config, processingSettings);

                using (servicesProvider as IDisposable)
                {

                    var runner = servicesProvider.GetRequiredService<Processor>();
                    runner.Process();

                    System.Console.WriteLine("Done! Press ANY key to exit");
                    System.Console.ReadKey();

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
            }
            //var logger = new NLog.Fi();
            //if (args.Length == 2 && args[1].ToLower().IndexOf("help", StringComparison.Ordinal) >= 0)
            //{
            //    // give help
            //    Console.WriteLine(@"dotnet run QtiPackageConverter package.zip task:(30)");
            //}

            //{
            //    var storeNewPackage = true;
            //    var appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location ?? string.Empty);

            //    var packages = new List<string>();
            //    if (File.Exists(args[0]))
            //    {
            //        packages.Add(args[0]);
            //    }
            //    else if (Directory.Exists(args[0]))
            //    {
            //        packages.AddRange(Directory.GetFiles(args[0], "*.zip"));
            //    }

            //    foreach (var package in packages)
            //    {
            //        var packageRef = package;
            //        if (!Path.IsPathRooted(packageRef)) packageRef = Path.Combine(appPath, packageRef);
            //        if (!File.Exists(packageRef)) throw new Exception($"cannot find package file: {packageRef}");
            //        var packageFolder = ExtractPackage(packageRef);

            //    }
        }

        private static IServiceProvider BuildDi(IConfiguration config, ProcessingSetting settings)
        {
            return new ServiceCollection()
               .AddTransient<Processor>() // Runner is the custom class
               .AddLogging(loggingBuilder =>
               {
                   // configure Logging with NLog
                   loggingBuilder.ClearProviders();
                   loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   loggingBuilder.AddNLog(config);
               })
               .AddSingleton(settings)
               .BuildServiceProvider();
        }
    }
}