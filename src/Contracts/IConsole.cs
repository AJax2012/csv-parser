namespace GardnerCsvParser.Contracts
{
    public interface IConsole
    {
        string GetTextFromUser(bool hasFailed);
        void WriteTextForUser(string output);
    }
}
