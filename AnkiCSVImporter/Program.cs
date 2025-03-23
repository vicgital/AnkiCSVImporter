using AnkiCSVImporter.Clients.Definition;
using AnkiCSVImporter.Clients.Implementation;
using AnkiCSVImporter.Components.Definition;
using AnkiCSVImporter.Components.Implementation;
using AnkiCSVImporter.Importers.Definition;
using AnkiCSVImporter.Importers.Implementation;
using AnkiCSVImporter.Models;
using AnkiCSVImporter.Models.Cards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Compact;

class Program
{
    static async Task Main()
    {
        #region Setup Job
        IServiceCollection services = new ServiceCollection();
        IConfiguration configuration = GetConfiguration();
        AddServices(services, configuration);
        #endregion

        var scope = services.BuildServiceProvider().CreateScope();
        Log.Information("START AnkiCSV Importer..");
        CsvImporter importer = GetImporter();
        switch (importer)
        {
            case CsvImporter.ChineseToEnglish:
                var chineseToEnglishDeckImporter = scope.ServiceProvider.GetRequiredService<IDeckImporter<ChineseToEnglishFlashcard>>();
                await chineseToEnglishDeckImporter.ImportDeck();
                break;
            case CsvImporter.ChineseToPinyin:
                var chineseToPinyinDeckImporter = scope.ServiceProvider.GetRequiredService<IDeckImporter<ChineseToPinyinFlashcard>>();
                await chineseToPinyinDeckImporter.ImportDeck();
                break;
            default:
                Console.WriteLine("Invalid selection");
                break;
        }
    }

    private static CsvImporter GetImporter()
    {
        Console.WriteLine("TYPE SELECT IMPORTER:");
        Console.WriteLine("1. Chinese to English");
        Console.WriteLine("2. Chinese to Pinyin");
        var importer = Console.ReadLine();
        if (int.TryParse(importer, out int importerId))
        {
            var cvsImporter = (CsvImporter)importerId;
            if (
                cvsImporter != CsvImporter.ChineseToEnglish &&
                cvsImporter != CsvImporter.ChineseToPinyin
                )
            {
                Console.WriteLine("Invalid selection");
                return GetImporter();
            }
            return cvsImporter;
        }
        else
        {
            Console.WriteLine("Invalid selection");
            return GetImporter();
        }
        
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddHttpClient("AnkiConnect", options =>
        {
            var ankiConnectUrl = configuration["AnkiConnect:Url"] ?? throw new Exception("AnkiConnect url not found");
            options.BaseAddress = new Uri(ankiConnectUrl);
        });
        services
            .AddSingleton<IAnkiConnectClient, AnkiConnectClient>()
            .AddSingleton<IAnkiDeckImportComponent, AnkiDeckImportComponent>()
            .AddSingleton<IDeckImporter<ChineseToPinyinFlashcard>, ChineseToPinyinDeckImporter>()
            .AddSingleton<IDeckImporter<ChineseToEnglishFlashcard>, ChineseToEnglishDeckImporter>();

        AddLogging(services, configuration);

    }

    private static void AddLogging(IServiceCollection services, IConfiguration configuration)
    {
        var logFilePath = configuration["Logger:FilePath"] ?? throw new Exception("Logger:FilePath not found");
        Log.Logger = new LoggerConfiguration()
              .WriteTo.Console(new CompactJsonFormatter())
              .WriteTo.File($"{logFilePath}\\AnkiiCsvImporter_{DateTime.Now.ToFileTime()}.log")
              .ReadFrom.Configuration(configuration)
              .Enrich.WithSpan()
              .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            loggingBuilder.AddDebug();
            loggingBuilder.AddSerilog();
        });
    }

    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }




}