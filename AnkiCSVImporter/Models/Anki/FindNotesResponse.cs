using Newtonsoft.Json;

namespace AnkiCSVImporter.Models.Anki
{
    internal class FindNotesResponse
    {
        [JsonProperty("result")]
        public List<long>? Result { get; set; }
    }
}
