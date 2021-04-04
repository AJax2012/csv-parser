namespace GardnerCsvParser.Contracts
{
    public interface IUserFeedbackService
    {
        string GetInputFileLocation();
        char GetRowSeparator();
        void HeaderCountNotSameAsPropertyCount(int headerCount, int propertyCount);
        bool? ParseContinueResponse(string result);
    }
}
