using AnkiCSVImporter.Components.Definition;
using AnkiCSVImporter.Importers.Definition;
using AnkiCSVImporter.Models.Cards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AnkiCSVImporter.Importers.Implementation
{
    internal class ChineseToPinyinDeckImporter(
        ILogger<ChineseToPinyinDeckImporter> logger,
        IAnkiDeckImportComponent ankiDeckImporter,
        IConfiguration configuration) : IDeckImporter<ChineseToPinyinFlashcard>
    {

        private readonly ILogger<ChineseToPinyinDeckImporter> _logger = logger;
        private readonly IAnkiDeckImportComponent _ankiDeckImporter = ankiDeckImporter;

        private readonly string ChineseToPinyinDeckName = configuration["ChineseToPinyinDeck:Name"] ?? throw new ArgumentException("ChineseToPinyinDeck:Name Setting not found");
        private readonly string ChineseToPinyinDeckModelName = configuration["ChineseToPinyinDeck:Model"] ?? throw new ArgumentException("ChineseToPinyinDeck:Model Setting not found");
        private readonly string ChineseToPinyinDeckCsvPath = configuration["ChineseToPinyinDeck:CsvPath"] ?? throw new ArgumentException("ChineseToPinyinDeck:CsvPath Setting not found");


        public async Task<bool> ImportDeck()
        {
            try
            {
                _logger.LogInformation("START Importing cards to {deck}", ChineseToPinyinDeckName);
                // Get Cards from CSV
                List<ChineseToPinyinFlashcard> cards = Helpers.CSVHelper.ReadCsv<ChineseToPinyinFlashcard>(ChineseToPinyinDeckCsvPath);
                _logger.LogInformation("Importing {count} cards to {deck}", cards.Count, ChineseToPinyinDeckModelName);

                return await _ankiDeckImporter.ImportDeck(ChineseToPinyinDeckName, ChineseToPinyinDeckModelName, cards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportDeck()");
                return false;
            }
        }
    }
}
