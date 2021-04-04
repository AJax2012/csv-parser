using System;
using GardnerCsvParser.Contracts;

namespace GardnerCsvParser.Services
{
    public class ConsoleProvider : IConsole
    {
        public void WriteTextForUser(string output)
        {
            Console.WriteLine(output);
        }

        /// <summary>
        /// Gets text from user.
        /// </summary>
        /// <param name="hasFailed">use for testing purposes only</param>
        /// <returns></returns>
        public string GetTextFromUser(bool hasFailed)
        {
            return Console.ReadLine();
        }
    }
}
