using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Serilog;

public class Program
{
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("garda.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"garda.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build();

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithThreadId()
            .CreateLogger();

        try
        {
            Log.Information("Getting the motors running...");

            BuildWebHost(args).Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseUrls($"http://localhost:{Configuration.GetValue<int>("ConnectionStrings:Kestrel")}")
            .UseKestrel(k => k.AddServerHeader = false)
            .UseStartup<garda.Startup>()
            .UseConfiguration(Configuration)
            .UseSerilog()
            .Build();
}








    