using System.Collections.Generic;
using System.Reflection;

using GardnerCsvParser.Models;

namespace GardnerCsvParser.Contracts
{
    public interface ICsvService
    {
        void RemoveEmptyRows(List<string> rows);
        List<string> GetAssumedHeaderRow(List<string> rows);
        bool IsHeaderRow(IEnumerable<string> rowItems);
        Enrollment ParseRow(Dictionary<PropertyInfo, int> indexes, List<string> rowItems);
        IEnumerable<Enrollment> ParseRows(Dictionary<PropertyInfo, int> indexes, List<string> rows);
        void RemoveHeaderRow(List<string> rows);
        List<string> SeparateIntoRows(string csv, char separator);
    }
}
