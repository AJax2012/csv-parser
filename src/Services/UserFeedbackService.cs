using GardnerCsvParser.Contracts;
using System;

namespace GardnerCsvParser.Services
{
    public class UserFeedbackService : IUserFeedbackService
    {
        private readonly IFileService _fileService;
        private readonly IConsole _console;

        public UserFeedbackService(IFileService fileService, IConsole console)
        {
            _fileService = fileService;
            _console = console;
        }

        public string GetInputFileLocation()
        {
            var response = string.Empty;
            var hasFailed = false;

            while (string.IsNullOrEmpty(response))
            {
                _console.WriteTextForUser("Please type the location of the file you'd like to parse.");
                response = _console.GetTextFromUser(hasFailed);

                if (!_fileService.FileExists(response))
                {
                    _console.WriteTextForUser("Please select a valid file. File does not exist.");
                    response = string.Empty;
                    hasFailed = true;
                }
            }

            return response;
        }

        public char GetRowSeparator()
        {
            var success = false;
            var response = new char();
            var hasFailed = false;

            while (!success)
            {
                _console.WriteTextForUser("Please type the character that separates the rows (eg. ';' or '\\n'");
                var result = _console.GetTextFromUser(hasFailed);

                success = char.TryParse(result, out response);

                if (!success)
                {
                    _console.WriteTextForUser("Not a valid character. Please try again.");
                    hasFailed = true;
                }
            }

            return response;
        }

        public void HeaderCountNotSameAsPropertyCount(int headerCount, int propertyCount)
        {
            var success = false;
            bool? response = null;
            var hasFailed = false;

            while (!success)
            {
                _console.WriteTextForUser($"The amount of properties that will be mapped are not equal to the amount of properties in the headers line of the CSV file. Properties to be mapped: {propertyCount}; Properties in header: {headerCount}. Would you like to continue? y/N");

                var result = _console.GetTextFromUser(hasFailed);

                response = ParseContinueResponse(result);

                if (response is null)
                {
                    _console.WriteTextForUser("Not a valid response. Please try again.");
                    hasFailed = true;
                    continue;
                }

                if (!response.Value)
                {
                    throw new ArgumentException("You have chosen to exit the program and fix the CSV File.");
                }

                success = true;
            }
        }

        public bool? ParseContinueResponse(string result)
        {
            switch (result.ToLower())
            {
                case string a when a.StartsWith("y"): return true;
                case string b when b.StartsWith("n"): return false;
                default: return null;
            }
        }
    }
}
