using AnkiCSVImporter.Clients.Definition;
using AnkiCSVImporter.Components.Definition;
using Microsoft.Extensions.Logging;

namespace AnkiCSVImporter.Components.Implementation
{
    internal class AnkiDeckImportComponent(ILogger<AnkiDeckImportComponent> logger, IAnkiConnectClient ankiConnectClient) : IAnkiDeckImportComponent
    {
        private readonly ILogger<AnkiDeckImportComponent> _logger = logger;
        private readonly IAnkiConnectClient _ankiConnectClient = ankiConnectClient;

        public async Task<bool> ImportDeck<T>(string deckName, string model, List<T> cards)
        {
            try
            {
                var index = 0;
                foreach (var card in cards)
                {
                    _logger.LogInformation("Importing card {index} of {count}: {card}", index++, cards.Count, card);
                    await ImportCard(deckName, model, card);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportDeck()");
                return false;
            }
        }

        private async Task ImportCard<T>(string deckName, string model, T? card)
        {
            try
            {
                if (card is null)
                    return;
                string fieldName = GetCardFieldNameIdentifier<T>(card);
                string value = GetCardFieldValue<T>(card, fieldName);
                var cardExists = await _ankiConnectClient.CardExists(deckName, fieldName, value);
                if (!cardExists)
                {
                    var success = await _ankiConnectClient.AddCard(deckName, model, card);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportCard({card})", card);
                throw;
            }
            
        }

        private static string GetCardFieldValue<T>(T card, string fieldName)
        {
            return card?.GetType().GetProperty(fieldName)?.GetValue(card)?.ToString() ?? throw new Exception("Unable to find card field value");
        }

        private static string GetCardFieldNameIdentifier<T>(T card)
        {
            return card?.GetType().GetProperties().FirstOrDefault()?.Name ?? throw new Exception("Unable to find card field identifier name");
        }

    }
}
