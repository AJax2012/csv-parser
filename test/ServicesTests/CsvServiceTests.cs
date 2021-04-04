using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;
using NUnit.Framework;

using GardnerCsvParser.Services;

namespace GardnerCsvParserTests.ServicesTests
{
    public class CsvServiceTests
    {
        private CsvService _sut;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _sut = new CsvService();
            fixture = new Fixture();
        }

        [Test]
        [TestCase(";")]
        [TestCase("\n")]
        public void SeparateIntoRows_SplitsRowsAccordingToSeparatorInput(char separator)
        {
            // Arrange
            var csv = $"test1.1,test1.2{separator}test2.1,test2.2{separator}test3.1,test3.2";

            // Act
            var result = _sut.SeparateIntoRows(csv, separator);

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void SeparateIntoRows_RemovesNewLineCharacters()
        {
            // Arrange
            var strProp1 = fixture.Create<string>();
            var strProp2 = fixture.Create<string>();

            var csv = @$"
{strProp1}.1,{strProp1}.2;
{strProp2}.1,{strProp2}.2;";

            // Act
            var result = _sut.SeparateIntoRows(csv, ';');

            // Assert
            Assert.IsFalse(result.Any(row => row.Contains("\n")));
            Assert.IsFalse(result.Any(row => row.Contains("\r")));
        }

        [Test]
        public void GetAssumedHeaderRow_GetsFirstRowOnly_AndSplitsRow()
        {
            // Arrange
            var expectedStrProp = fixture.Create<string>();
            var notExpectedStrProp = fixture.Create<string>();

            var rows = new List<string> 
            {
                $"{expectedStrProp}1,{expectedStrProp}2",
                $"{notExpectedStrProp}1.1,{notExpectedStrProp}1.2",
                $"{notExpectedStrProp}1.1,{notExpectedStrProp}2.2"
            };

            // Act
            var result = _sut.GetAssumedHeaderRow(rows);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(item => item.StartsWith(expectedStrProp)));
        }

        [Test]
        public void GetAssumedHeaderRow_ReturnsEmptyList_WhenRowsEmpty()
        {
            // Arrange
            var rows = new List<string>();

            // Act
            var result = _sut.GetAssumedHeaderRow(rows);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase("Expected1", "Expected2", true)]
        [TestCase("1", "2", false)]
        public void IsHeaderRow_VerifiesThatRowIsNotInt(string firstValue, string secondValue, bool expectedResult)
        {
            // Arrange
            var rows = new List<string>
            {
                firstValue,
                secondValue
            };

            // Act
            var result = _sut.IsHeaderRow(rows);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void RemoveEmptyRows_RemovesEmptyRows()
        {
            // Arrange
            var strProp = fixture.Create<string>();

            var rows = new List<string>
            {
                strProp,
                $"{strProp}   ",
                string.Empty,
                "   "
            };

            // Act
            _sut.RemoveEmptyRows(rows);

            // Assert
            Assert.AreEqual(2, rows.Count);
        }

        [Test]
        public void ParseRows_ReturnsTwoEnrollments_WhenGivenTwoLines()
        {
            // Arrange
            var strProp = fixture.Create<string>();
            var intProp = fixture.Create<int>();

            var indexes = PropertyInfoTestHelpers.GetTestIndexs();
            var rows = new List<string>
            {
                $"{strProp},{intProp}",
                $"{strProp}.1,{intProp + 1}"
            };

            // Act
            var response = _sut.ParseRows(indexes, rows);

            // Assert
            Assert.AreEqual(2, response.Count());
        }

        [Test]
        public void ParseRow_ReturnsIntPropertyAndStringProperty()
        {
            // Arrange
            var strProp = fixture.Create<string>();
            var intProp = fixture.Create<int>();

            var indexes = PropertyInfoTestHelpers.GetTestIndexs();
            var rows = new List<string>
            {
                strProp,
                intProp.ToString()
            };

            // Act
            var response = _sut.ParseRow(indexes, rows);

            // Assert
            Assert.AreEqual(strProp, response.UserId);
            Assert.AreEqual(intProp, response.Version);
        }

        [Test]
        public void ParseIntProperty_ThrowsArgumentExceptionWhenPropertyIsNotInt()
        {
            // Arrange
            var str = fixture.Create<string>();
            var propertyName = fixture.Create<string>();
            var expectedMessage = $"CSV File invalid. {propertyName} must be an integer";

            // Act
            // Assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ParseIntProperty(str, propertyName));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ParseIntProperty_ReturnsIntWhenPropertyIsInt()
        {
            // Arrange
            var expected = fixture.Create<int>();
            var str = expected.ToString();
            var propertyName = fixture.Create<string>();

            // Act
            var result = _sut.ParseIntProperty(str, propertyName);

            // Assert
            Assert.AreEqual(expected, result);
        }        
    }
}
