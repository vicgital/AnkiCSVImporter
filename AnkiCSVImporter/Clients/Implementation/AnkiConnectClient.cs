using System.Text;
using AnkiCSVImporter.Clients.Definition;
using AnkiCSVImporter.Models.Anki;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AnkiCSVImporter.Clients.Implementation
{
    internal class AnkiConnectClient(IHttpClientFactory httpClientFactory, ILogger<AnkiConnectClient> logger) : IAnkiConnectClient
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<AnkiConnectClient> _logger = logger;

        public async Task<bool> AddCard<T>(string deck, string model, T card)
        {
            try
            {
                var request = new
                {
                    action = "addNote",
                    version = 6,
                    @params = new
                    {
                        note = new
                        {
                            deckName = deck,
                            modelName = model,
                            fields = card,
                            options = new { allowDuplicate = false },
                            tags = new[] { "imported" }
                        }
                    }
                };
                await SendRequest(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddCard()");
                throw;
            }
        }

        public async Task<bool> CardExists(string deckName, string fieldName, string value)
        {
            try
            {
                var query = $@"""deck:{deckName}"" {fieldName}:{value}";
                var request = new { action = "findNotes", version = 6, @params = new { query } };
                string response = await SendRequest(request);
                var result = JsonConvert.DeserializeObject<FindNotesResponse>(response);
                return result?.Result?.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CardExists");
                throw;
            }
        }


        private async Task<string> SendRequest(object requestObj)
        {
            string json = JsonConvert.SerializeObject(requestObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient("AnkiConnect");

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content
            };

            var response = await client.SendAsync(message);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
