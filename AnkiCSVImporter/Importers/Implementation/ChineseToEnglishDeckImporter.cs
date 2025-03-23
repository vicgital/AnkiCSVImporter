using AnkiCSVImporter.Components.Definition;
using AnkiCSVImporter.Importers.Definition;
using AnkiCSVImporter.Models.Cards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AnkiCSVImporter.Importers.Implementation
{
    internal class ChineseToEnglishDeckImporter(
        ILogger<ChineseToEnglishDeckImporter> logger,
        IAnkiDeckImportComponent ankiDeckImporter,
        IConfiguration configuration) : IDeckImporter<ChineseToEnglishFlashcard>
    {
        private readonly ILogger<ChineseToEnglishDeckImporter> _logger = logger;
        private readonly IAnkiDeckImportComponent _ankiDeckImporter = ankiDeckImporter;


        private readonly string ChineseToEnglishDeckName = configuration["ChineseToEnglishDeck:Name"] ?? throw new ArgumentException("ChineseToEnglishDeck:Name Setting not found");
        private readonly string ChineseToEnglishDeckModelName = configuration["ChineseToEnglishDeck:Model"] ?? throw new ArgumentException("ChineseToEnglishDeck:Model Setting not found");
        private readonly string ChineseToEnglishDeckCsvPath = configuration["ChineseToEnglishDeck:CsvPath"] ?? throw new ArgumentException("ChineseToEnglishDeck:CsvPath Setting not found");


        public async Task<bool> ImportDeck()
        {
            try
            {
                _logger.LogInformation("START Importing cards to {deck}", ChineseToEnglishDeckName);
                // Get Cards from CSV
                List<ChineseToEnglishFlashcard> cards = Helpers.CSVHelper.ReadCsv<ChineseToEnglishFlashcard>(ChineseToEnglishDeckCsvPath);
                _logger.LogInformation("Importing {count} cards to {deck}", cards.Count, ChineseToEnglishDeckName);

                return await _ankiDeckImporter.ImportDeck(ChineseToEnglishDeckName, ChineseToEnglishDeckModelName, cards);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportDeck()");
                throw;
            }

        }
    }
}
