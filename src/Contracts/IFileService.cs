using System.Threading;
using System.Threading.Tasks;

namespace GardnerCsvParser.Contracts
{
    public interface IFileService
    {
        Task<string> GetContents(string filePath);
        bool DirectoryExists(string path);
        bool FileExists(string filePath);
        Task WriteOutputFile(string enrollments, string filePath, CancellationToken cancellationToken);
        string GetOutputDirectory(string directoryPath);
        string GetOuputFilePath(string outputDirectory, string companyName);
    }
}
