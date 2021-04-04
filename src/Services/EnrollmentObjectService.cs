using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Extensions;
using GardnerCsvParser.Models;

namespace GardnerCsvParser.Services
{
    public class EnrollmentObjectService : IEnrollmentObjectService
    {
        public Dictionary<PropertyInfo, int> GetIndexValues(List<string> headers)
        {
            var response = new Dictionary<PropertyInfo, int>();
            var properties = GetEnrollmentProperties();

            foreach (var property in properties)
            {
                var item = GetMatchingHeader(headers, property);

                if (item is null)
                {
                    continue;
                }

                var index = headers.IndexOf(item);

                response.Add(property, index);
            }

            return response;
        }

        public string GetMatchingHeader(List<string> headers, PropertyInfo property)
        {
            var shortPropertyName = property.Name.GetFirstLetters();
            var header = headers.FirstOrDefault(header => header.ToLower().StartsWith(shortPropertyName));
            return header;
        }

        public bool IsRowValidLength(List<string> row, int propertyAmount)
        {
            return row.Count == propertyAmount;
        }

        public IEnumerable<PropertyInfo> GetEnrollmentProperties()
        {
            var type = typeof(Enrollment);
            return type.GetProperties();
        }

        public IEnumerable<string> GetEnrollmentPropertyNames()
        {
            var properties = GetEnrollmentProperties();
            return properties.Select(property => property.Name);
        }

        public IEnumerable<Enrollment> GetEnrollmentOutput(IEnumerable<Enrollment> enrollments)
        {
            return enrollments
                .GroupBy(enrollment => 
                    (enrollment.UserId, enrollment.InsuranceCompany),
                    (key, enrollment) => enrollment
                        .OrderByDescending(e => e.Version)
                        .FirstOrDefault());
        }

        public IEnumerable<Enrollment> SortEnrollmentsForCompany(IEnumerable<Enrollment> enrollments, string companyName)
        {
            return enrollments
                .Where(enrollment => enrollment.InsuranceCompany == companyName)
                .OrderBy(enrollment => enrollment.LastName)
                .ThenBy(enrollment => enrollment.FirstName)
                .ToList();
        }
    }
}
