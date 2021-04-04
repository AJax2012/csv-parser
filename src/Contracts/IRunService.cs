using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using GardnerCsvParser.Models;

namespace GardnerCsvParser.Contracts
{
    public interface IRunService
    {
        void CleanCsv(List<string> enrollmentRows);
        Task CreateResponse(IEnumerable<Enrollment> enrollments, string outputFilePath, CancellationToken cancellationToken);
        List<string> GetEnrollmentRows(string fileContents, char rowSeparator);
        IEnumerable<Enrollment> GetEnrollments(Dictionary<PropertyInfo, int> indexes, List<string> rows);
        Task<string> GetFileContents(string inputFilePath);
        List<string> GetHeaderRow(List<string> enrollmentRows);
        string GetInputFilePath(string filepath);
        char GetRowSeperator(char seperator);
        UserInputDto GetUserInputValues(string filePath, char seperator);
        void ValidateRowLengths(List<string> rows, int headerRowCount);
    }
}
