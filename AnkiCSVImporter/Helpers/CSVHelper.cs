using System.Globalization;
using System.Text;
using CsvHelper;

namespace AnkiCSVImporter.Helpers
{
    internal class CSVHelper
    {
        public static List<T> ReadCsv<T>(string filePath)
        {
            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return new List<T>(csv.GetRecords<T>());
        }
    }
}
