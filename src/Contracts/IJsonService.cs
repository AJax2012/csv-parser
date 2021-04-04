using System.Collections.Generic;

using GardnerCsvParser.Models;

namespace GardnerCsvParser.Contracts
{
    public interface IJsonService
    {
        string GetClosingBracketLine(int enrollmentCount, int iteration);
        string GetJsonLineEnd(int propertyCount, int iteration);
        object GetPropertyValueLine(object value);
        string SerializeEnrollmentsToJson(List<Enrollment> enrollments);
    }
}
