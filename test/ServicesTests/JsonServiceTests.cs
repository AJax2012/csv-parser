using System.Collections.Generic;
using System.Reflection;

using AutoFixture;
using Moq;
using NUnit.Framework;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Models;
using GardnerCsvParser.Services;

namespace GardnerCsvParserTests.ServicesTests
{
    public class JsonServiceTests
    {
        private JsonService _sut;
        private Mock<IEnrollmentObjectService> _enrollmentServiceMock;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _enrollmentServiceMock = new Mock<IEnrollmentObjectService>();
            _sut = new JsonService(_enrollmentServiceMock.Object);
            fixture = new Fixture();
        }

        [Test]
        public void SerializeEnrollmentsToJson_ReturnsReadableJsonObject()
        {
            // Arrange
            var enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    UserId = "Guid2",
                    FirstName = "AExpectedFirst",
                    LastName = "AExpectedLast",
                    Version = 4,
                    InsuranceCompany = "company2"
                },
                new Enrollment
                {
                    UserId = "Guid",
                    FirstName = "ExpectedFirst",
                    LastName = "ExpectedLast",
                    Version = 4,
                    InsuranceCompany = "company"
                }
            };

            var expectedOutput = "{\r\n\"data\": [\r\n{\r\n\"UserId\": \"Guid2\",\n\"FirstName\": \"AExpectedFirst\",\n\"LastName\": \"AExpectedLast\",\n\"Version\": 4,\n\"InsuranceCompany\": \"company2\"\n},\r\n{\r\n\"UserId\": \"Guid\",\n\"FirstName\": \"ExpectedFirst\",\n\"LastName\": \"ExpectedLast\",\n\"Version\": 4,\n\"InsuranceCompany\": \"company\"\n}\r\n]\r\n}\r\n";

            var properties = GetEnrollmentProperties();

            _enrollmentServiceMock.Setup(s => s.GetEnrollmentProperties()).Returns(properties).Verifiable();

            // Act
            var result = _sut.SerializeEnrollmentsToJson(enrollments);

            // Assert
            _enrollmentServiceMock.VerifyAll();
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(true)]
        public void GetPropertyValueLine_ReturnsOnlyValueWhenValueTypeIsIntOrBool(object value)
        {
            // Act
            var result = _sut.GetPropertyValueLine(value);

            // Assert
            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetPropertyValueLine_ReturnsValueInQuotesWhenNotIntOrBool()
        {
            // Arrange
            object value = fixture.Create<string>();
            object expected = $"\"{value}\"";

            // Act
            var result = _sut.GetPropertyValueLine(value);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(1, ",\n")]
        [TestCase(2, "\n")]
        public void GetJsonLineEnd_ReturnsCommaWehNotLastLine(int iteration, string expectedResult)
        {
            // Arrange
            var propertyCount = 3;

            // Act
            var result = _sut.GetJsonLineEnd(propertyCount, iteration);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(1, "},")]
        [TestCase(2, "}")]
        public void GetClosingBracketLine_ReturnsCommaWhenNotLastLine(int iteration, string expectedResult)
        {
            // Arrange
            var enrollmentCount = 3;

            // Act
            var result = _sut.GetClosingBracketLine(enrollmentCount, iteration);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        private IEnumerable<PropertyInfo> GetEnrollmentProperties()
        {
            var type = typeof(Enrollment);
            return type.GetProperties();
        }
    }
}
