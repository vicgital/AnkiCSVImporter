namespace AnkiCSVImporter.Importers.Definition
{
    internal interface IDeckImporter<T>
    {
        Task<bool> ImportDeck();
    }
}
