using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GardnerCsvParser.Contracts;

namespace GardnerCsvParser.Services
{
    public class FileService : IFileService
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public async Task<string> GetContents(string filePath)
        {
            var records = string.Empty;

            using (var stream = new StreamReader(filePath))
            {
                records = await stream.ReadToEndAsync();
            }

            return records;
        }

        public string GetOutputDirectory(string inputFile)
        {
            return Path.GetDirectoryName(inputFile);
        }

        public string GetOuputFilePath(string outputDirectory, string companyName)
        {
            return Path.Combine(outputDirectory, $"{companyName}.json");
        }

        public async Task WriteOutputFile(string enrollments, string filePath, CancellationToken cancellationToken)
        {
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var bytes = Encoding.ASCII.GetBytes(enrollments);
                await stream.WriteAsync(bytes, cancellationToken);
            }
        }
    }
}
