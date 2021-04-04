using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Services;

namespace GardnerCsvParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var serviceProvider = new ServiceCollection()
                .AddScoped<IConsole, ConsoleProvider>()
                .AddScoped<ICsvService, CsvService>()
                .AddScoped<IEnrollmentObjectService, EnrollmentObjectService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IJsonService, JsonService>()
                .AddScoped<IRunService, RunService>()
                .AddScoped<IUserFeedbackService, UserFeedbackService>()
                .BuildServiceProvider();

            var runService = serviceProvider.GetService<IRunService>();
            var enrollmentService = serviceProvider.GetService<IEnrollmentObjectService>();

            var program = new RunProgram(runService, enrollmentService);
            
            try
            {
                await program.RunAsync(args, new CancellationToken());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("The file is invalid: " + ex.Message);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unknown exception occured: " + ex.Message);
                Environment.Exit(1);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Environment.Exit(0);
        }
    }
}
