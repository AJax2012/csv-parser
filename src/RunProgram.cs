using System.Threading;
using System.Threading.Tasks;

using GardnerCsvParser.Contracts;

namespace GardnerCsvParser
{
    public class RunProgram
    {
        private readonly IRunService _runService;
        private readonly IEnrollmentObjectService _enrollmentService;

        public RunProgram(IRunService runService, IEnrollmentObjectService enrollmentService)
        {
            _runService = runService;
            _enrollmentService = enrollmentService;
        }

        public async Task RunAsync(string[] args, CancellationToken cancellationToken)
        {
            var filePath = string.Empty;
            char separator = default(char);

            if (args.Length > 0)
            {
                filePath = args[0];
                _ = char.TryParse(args[1], out separator);
            }

            var userInputDto = _runService.GetUserInputValues(filePath, separator);
            var fileContents = await _runService.GetFileContents(userInputDto.InputFilePath);
            var enrollmentRows = _runService.GetEnrollmentRows(fileContents, userInputDto.RowSeparator);
            var headers = _runService.GetHeaderRow(enrollmentRows);
            _runService.CleanCsv(enrollmentRows);
            _runService.ValidateRowLengths(enrollmentRows, headers.Count);
            var indexes = _enrollmentService.GetIndexValues(headers);
            var enrollments = _runService.GetEnrollments(indexes, enrollmentRows);
            await _runService.CreateResponse(enrollments, userInputDto.OutputDirectory, cancellationToken);
        }
    }
}
