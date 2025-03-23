using AnkiCSVImporter.Models.Anki;

namespace AnkiCSVImporter.Models.Cards
{
    internal class ChineseToPinyinFlashcard : Card
    {
        public string? Chinese { get; set; }
        public string? Pinyin { get; set; }
        public string? Meaning { get; set; }

        public override string ToString() =>
            Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
