using System.Collections.Generic;
using System.Reflection;

using GardnerCsvParser.Models;

namespace GardnerCsvParser.Contracts
{
    public interface IEnrollmentObjectService
    {
        IEnumerable<PropertyInfo> GetEnrollmentProperties();
        IEnumerable<string> GetEnrollmentPropertyNames();
        Dictionary<PropertyInfo, int> GetIndexValues(List<string> headers);
        string GetMatchingHeader(List<string> headers, PropertyInfo property);
        IEnumerable<Enrollment> GetEnrollmentOutput(IEnumerable<Enrollment> enrollments);
        bool IsRowValidLength(List<string> row, int propertyAmount);
        IEnumerable<Enrollment> SortEnrollmentsForCompany(IEnumerable<Enrollment> enrollments, string companyName);
    }
}
