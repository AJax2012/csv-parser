using GardnerCsvParser.Contracts;
using System;

namespace GardnerCsvParser.Services
{
    public class UserFeedbackService : IUserFeedbackService
    {
        private readonly IFileService _fileService;
        private readonly IConsole _consoleRetriever;

        public UserFeedbackService(IFileService fileService, IConsole consoleRetriever)
        {
            _fileService = fileService;
            _consoleRetriever = consoleRetriever;
        }

        public string GetInputFileLocation()
        {
            var response = string.Empty;
            var hasFailed = false;

            while (string.IsNullOrEmpty(response))
            {
                _consoleRetriever.WriteTextForUser("Please type the location of the file you'd like to parse.");
                response = _consoleRetriever.GetTextFromUser(hasFailed);

                if (!_fileService.FileExists(response))
                {
                    _consoleRetriever.WriteTextForUser("Please select a valid file. File does not exist.");
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
                _consoleRetriever.WriteTextForUser("Please type the character that separates the rows (eg. ';' or '\\n'");
                var result = _consoleRetriever.GetTextFromUser(hasFailed);

                success = char.TryParse(result, out response);

                if (!success)
                {
                    _consoleRetriever.WriteTextForUser("Not a valid character. Please try again.");
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
                _consoleRetriever.WriteTextForUser($"The amount of properties that will be mapped are not equal to the amount of properties in the headers line of the CSV file. Properties to be mapped: {propertyCount}; Properties in header: {headerCount}. Would you like to continue? y/N");

                var result = _consoleRetriever.GetTextFromUser(hasFailed);

                response = ParseContinueResponse(result);

                if (response is null)
                {
                    _consoleRetriever.WriteTextForUser("Not a valid response. Please try again.");
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
