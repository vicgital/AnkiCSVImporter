namespace AnkiCSVImporter.Clients.Definition
{
    internal interface IAnkiConnectClient
    {
        Task<bool> CardExists(string deck, string fieldName, string value);

        Task<bool> AddCard<T>(string deck, string model, T card);
    }
}
