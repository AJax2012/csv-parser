using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GardnerCsvParser.Models;

namespace GardnerCsvParserTests
{
    public static class PropertyInfoTestHelpers
    {
        /// <summary>
        /// Returns 2 properties to allow for testing common dictionary parameter
        /// </summary>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, int> GetTestIndexs()
        {
            var testProperties = GetTestProperties();
            var indexes = new Dictionary<PropertyInfo, int>();
            indexes.Add(testProperties.First(x => x.Name == "UserId"), 0);
            indexes.Add(testProperties.First(x => x.Name == "Version"), 1);
            return indexes;
        }

        public static IEnumerable<PropertyInfo> GetTestProperties()
        {
            var testClassType = typeof(Enrollment);
            return testClassType.GetProperties();
        }
    }
}
