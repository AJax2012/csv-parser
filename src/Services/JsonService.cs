using System.Collections.Generic;
using System.Linq;
using System.Text;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Models;

namespace GardnerCsvParser.Services
{
    public class JsonService : IJsonService
    {
        private readonly IEnrollmentObjectService _enrollmentService;

        public JsonService(IEnrollmentObjectService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public string SerializeEnrollmentsToJson(List<Enrollment> enrollments)
        {
            var properties = _enrollmentService.GetEnrollmentProperties().ToList();

            var builder = new StringBuilder();
            builder.AppendLine("{");
            builder.AppendLine("\"data\": [");

            for (var i = 0; i < enrollments.Count; i++)
            {
                builder.AppendLine("{");

                for (var j = 0; j < properties.Count; j++)
                {
                    var property = properties[j];
                    builder.Append($"\"{property.Name}\": ");
                    builder.Append(GetPropertyValueLine(property.GetValue(enrollments[i])));
                    builder.Append(GetJsonLineEnd(properties.Count, j));
                }

                builder.AppendLine(GetClosingBracketLine(enrollments.Count, i));
            }


            builder.AppendLine("]");
            builder.AppendLine("}");
            return builder.ToString();
        }

        public object GetPropertyValueLine(object value)
        {
            if (value.GetType() == typeof(int) || value.GetType() == typeof(bool))
            {
                return value;
            }

            return $"\"{value}\"";
        }

        public string GetJsonLineEnd(int propertyCount, int iteration)
        {
            return iteration < propertyCount - 1 ? ",\n" : "\n";
        }

        public string GetClosingBracketLine(int enrollmentCount, int iteration)
        {
            return iteration < enrollmentCount - 1 ? "}," : "}";
        }
    }
}
