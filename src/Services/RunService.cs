using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Models;

namespace GardnerCsvParser.Services
{
    public class RunService : IRunService
    {
        private readonly ICsvService _csvService;
        private readonly IEnrollmentObjectService _enrollmentService;
        private readonly IFileService _fileService;
        private readonly IUserFeedbackService _userFeedbackService;
        private readonly IJsonService _jsonService;

        public RunService(
            ICsvService csvService,
            IEnrollmentObjectService enrollmentService,
            IFileService fileService,
            IUserFeedbackService userFeedbackService,
            IJsonService jsonService)
        {
            _csvService = csvService;
            _enrollmentService = enrollmentService;
            _fileService = fileService;
            _userFeedbackService = userFeedbackService;
            _jsonService = jsonService;
        }

        public UserInputDto GetUserInputValues(string filePath, char seperator)
        {
            var userInputDto = new UserInputDto();
            userInputDto.InputFilePath = GetInputFilePath(filePath);
            userInputDto.OutputDirectory = _fileService.GetOutputDirectory(userInputDto.InputFilePath);
            userInputDto.RowSeparator = GetRowSeperator(seperator);
            return userInputDto;
        }

        public string GetInputFilePath(string filepath)
        {
            if (!string.IsNullOrWhiteSpace(filepath))
            {
                return filepath;
            }

            return _userFeedbackService.GetInputFileLocation();
        }

        public char GetRowSeperator(char seperator)
        {
            if (seperator != default(char))
            {
                return seperator;
            }

            return _userFeedbackService.GetRowSeparator();
        }

        public async Task<string> GetFileContents(string inputFilePath)
        {
            var fileContents = await _fileService.GetContents(inputFilePath);

            if (string.IsNullOrWhiteSpace(fileContents))
            {
                throw new ArgumentException("Csv file cannot be empty.");
            }

            return fileContents;
        }

        public List<string> GetEnrollmentRows(string fileContents, char rowSeparator)
        {
            var enrollmentRows = _csvService.SeparateIntoRows(fileContents, rowSeparator);

            if (enrollmentRows.Count <= 1)
            {
                throw new ArgumentException("Csv file must have more than 1 row.");
            }

            return enrollmentRows;
        }

        public List<string> GetHeaderRow(List<string> enrollmentRows)
        {
            var headers = _csvService.GetAssumedHeaderRow(enrollmentRows);

            if (!_csvService.IsHeaderRow(headers))
            {
                throw new ArgumentException("Fist row of CSV file must contain vaild headers.");
            }

            var propertyCount = _enrollmentService.GetEnrollmentProperties().Count();

            if (headers.Count != propertyCount)
            {
                _userFeedbackService.HeaderCountNotSameAsPropertyCount(headers.Count, propertyCount);
            }

            return headers;
        }

        public void CleanCsv(List<string> enrollmentRows)
        {
            _csvService.RemoveHeaderRow(enrollmentRows);
            _csvService.RemoveEmptyRows(enrollmentRows);
        }

        public void ValidateRowLengths(List<string> rows, int headerRowCount)
        {
            var errorList = new List<int>();

            for (var i = 0; i < rows.Count; i++)
            {
                var rowLength = rows[i].Split(',').Count();

                if (headerRowCount != rowLength)
                {
                    errorList.Add(i + 1);
                }
            }

            if (errorList.Any())
            {
                throw new ArgumentException("Header row cannot be different length than other rows. Rows which require fixing (with header row being 0): " + string.Join(", ", errorList));
            }
        }

        public IEnumerable<Enrollment> GetEnrollments(Dictionary<PropertyInfo, int> indexes, List<string> rows)
        {
            var enrollments = _csvService.ParseRows(indexes, rows);
            enrollments = _enrollmentService.GetEnrollmentOutput(enrollments);
            return enrollments;
        }

        public async Task CreateResponse(IEnumerable<Enrollment> enrollments, string outputDirectory, CancellationToken cancellationToken)
        {
            foreach(var enrollment in enrollments)
            {
                var ouptutFilePath = _fileService.GetOuputFilePath(outputDirectory, enrollment.InsuranceCompany);
                var companyEnrollments = _enrollmentService.SortEnrollmentsForCompany(enrollments, enrollment.InsuranceCompany);
                var outputEnrollments = _jsonService.SerializeEnrollmentsToJson(companyEnrollments.ToList());
                await _fileService.WriteOutputFile(outputEnrollments, ouptutFilePath, cancellationToken);
            }
        }
    }
}
