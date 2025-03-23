namespace AnkiCSVImporter.Components.Definition
{
    internal interface IAnkiDeckImportComponent
    {
        Task<bool> ImportDeck<T>(string deckName, string model, List<T> cards);
    }
}
