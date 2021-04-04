using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Models;
using GardnerCsvParser.Extensions;
using System;

namespace GardnerCsvParser.Services
{
    public class CsvService : ICsvService
    {
        public List<string> SeparateIntoRows(string csv, char separator)
        {
            var list = csv.Split(separator).ToList();
            var formattedList = list.Select(item => item.Replace(System.Environment.NewLine, string.Empty));
            return formattedList.ToList();
        }

        public List<string> GetAssumedHeaderRow(List<string> rows)
        {
            var firstRow = rows.FirstOrDefault();

            if (firstRow is null)
            {
                return new List<string>();
            }

            return firstRow.SplitCsvRow();
        }

        public void RemoveHeaderRow(List<string> rows)
        {
            rows.RemoveAt(0);
        }

        public bool IsHeaderRow(IEnumerable<string> rowItems)
        {
            return rowItems.All(item => !int.TryParse(item, out _));
        }

        public void RemoveEmptyRows(List<string> rows)
        {
            rows.RemoveAll(row => string.IsNullOrWhiteSpace(row));
        }

        public IEnumerable<Enrollment> ParseRows(Dictionary<PropertyInfo, int> indexes, List<string> rows)
        {
            var enrollments = new List<Enrollment>();

            foreach (var row in rows)
            {
                var rowItems = row.SplitCsvRow();
                enrollments.Add(ParseRow(indexes, rowItems));
            }

            return enrollments;
        }

        public Enrollment ParseRow(Dictionary<PropertyInfo, int> indexes, List<string> rowItems)
        {
            var enrollment = new Enrollment();

            foreach (var index in indexes)
            {
                if (index.Key.PropertyType == typeof(int))
                {
                    var value = ParseIntProperty(rowItems[index.Value], index.Key.Name);
                    index.Key.SetValue(enrollment, value);
                }
                else
                {
                    index.Key.SetValue(enrollment, rowItems[index.Value]);
                }
            }

            return enrollment;
        }

        public int ParseIntProperty(string str, string propertyName) 
        {
            var success = int.TryParse(str, out var value);

            if (!success)
            {
                throw new ArgumentException($"CSV File invalid. {propertyName} must be an integer");
            }

            return value;
        }
    }
}
