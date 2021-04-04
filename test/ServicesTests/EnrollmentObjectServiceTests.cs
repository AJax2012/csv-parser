using System.Collections.Generic;
using System.Linq;

using AutoFixture;
using NUnit.Framework;

using GardnerCsvParser.Models;
using GardnerCsvParser.Services;

namespace GardnerCsvParserTests.ServicesTests
{
    public class EnrollmentObjectServiceTests
    {
        private EnrollmentObjectService _sut;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _sut = new EnrollmentObjectService();
            fixture = new Fixture();
        }

        [Test]
        public void GetIndexValues_ReturnsPropertiesWithLocationOfPropertyNameInIndex()
        {
            // Arrange
            var headers = new List<string>
            {
                "user_id", "first_name", "last_name", "version", "insurance_company"
            };

            // Act
            var result = _sut.GetIndexValues(headers);

            var userIdActualIndex = result.First(r => r.Key.Name == "UserId").Value;
            var firstNameActualIndex = result.First(r => r.Key.Name == "FirstName").Value;
            var lastNameActualIndex = result.First(r => r.Key.Name == "LastName").Value;
            var versionActualIndex = result.First(r => r.Key.Name == "Version").Value;
            var insuranceCompanyActualIndex = result.First(r => r.Key.Name == "InsuranceCompany").Value;

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(0, userIdActualIndex);
            Assert.AreEqual(1, firstNameActualIndex);
            Assert.AreEqual(2, lastNameActualIndex);
            Assert.AreEqual(3, versionActualIndex);
            Assert.AreEqual(4, insuranceCompanyActualIndex);
        }

        [Test]
        public void GetIndexValues_ReturnsFourProperties_WhenCannotMatchPropertyName()
        {
            // Arrange
            var headers = new List<string>
            {
                "id", "first_name", "last_name", "version", "insurance_company"
            };

            // Act
            var result = _sut.GetIndexValues(headers);

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [Test]
        public void GetMatchingHeader_ReturnsExpectedHeader_WhenFound()
        {
            // Arrange
            var userId = "user_id";

            var headers = new List<string>
            {
                userId, "first_name", "last_name", "version", "insurance_company"
            };

            var property = PropertyInfoTestHelpers.GetTestProperties().First(prop => prop.Name == "UserId");

            // Act
            var result = _sut.GetMatchingHeader(headers, property);

            // Assert
            Assert.AreEqual(userId, result);
        }

        [Test]
        public void GetMatchingHeader_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var headers = new List<string>
            {
                "id", "first_name", "last_name", "version", "insurance_company"
            };

            var property = PropertyInfoTestHelpers.GetTestProperties().First(prop => prop.Name == "UserId");

            // Act
            var result = _sut.GetMatchingHeader(headers, property);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetEnrollmentProperties_ReturnsFiveProperties()
        {
            // Act
            var result = _sut.GetEnrollmentProperties();

            // Assert
            Assert.AreEqual(5, result.Count());
        }

        [Test]
        public void GetEnrollmentPropertyNames_ReturnsFivePropertyName()
        {
            // Act
            var result = _sut.GetEnrollmentPropertyNames();

            // Assert
            Assert.AreEqual(5, result.Count());
            Assert.IsTrue(result.Contains("UserId"));
            Assert.IsTrue(result.Contains("FirstName"));
            Assert.IsTrue(result.Contains("LastName"));
            Assert.IsTrue(result.Contains("Version"));
            Assert.IsTrue(result.Contains("InsuranceCompany"));
        }

        [Test]
        public void GetEnrollmentOutput_GroupsEnrollmentsByUserIdAndInsuranceCompany()
        {
            // Arrange
            var userId = fixture.Create<string>();
            var insuranceCompany = fixture.Create<string>();
            var version = fixture.Create<int>();

            var enrollment1 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.UserId, userId)
                .With(enrollment => enrollment.InsuranceCompany, insuranceCompany)
                .With(enrollment => enrollment.Version, version)
                .Create();

            var enrollment2 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.UserId, userId)
                .With(enrollment => enrollment.InsuranceCompany, insuranceCompany)
                .With(enrollment => enrollment.Version, version + 1)
                .Create();

            var enrollment3 = fixture.Create<Enrollment>();

            var enrollments = new List<Enrollment> { enrollment1, enrollment2, enrollment3 };

            // Act
            var result = _sut.GetEnrollmentOutput(enrollments);
            var actualEnrollmentUnderTest = result.Where(x => x.UserId == userId);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, actualEnrollmentUnderTest.Count());
            Assert.AreEqual(version + 1, actualEnrollmentUnderTest.First().Version);
        }

        [Test]
        public void SortEnrollmentsForCompany_FiltersCompanyName()
        {
            // Arrange
            var a = "a";
            var b = "b";
            var companyName = fixture.Create<string>();

            var enrollment1 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.LastName, a)
                .With(enrollment => enrollment.FirstName, a)
                .With(enrollment => enrollment.InsuranceCompany, companyName)
                .Create();

            var enrollment2 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.LastName, b)
                .With(enrollment => enrollment.FirstName, a)
                .With(enrollment => enrollment.InsuranceCompany, companyName)
                .Create();

            var enrollment3 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.LastName, b)
                .With(enrollment => enrollment.FirstName, b)
                .Create();

            // insert out of order
            var enrollments = new List<Enrollment> { enrollment3, enrollment2, enrollment1 };

            // Act
            var result = _sut.SortEnrollmentsForCompany(enrollments, companyName);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void SortEnrollmentsForCompany_SortsByLastNameThenFirstName()
        {
            // Arrange
            var a = "a";
            var b = "b";
            var companyName = fixture.Create<string>();

            var enrollment1 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.LastName, a)
                .With(enrollment => enrollment.FirstName, a)
                .With(enrollment => enrollment.InsuranceCompany, companyName)
                .Create();

            var enrollment2 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.LastName, b)
                .With(enrollment => enrollment.FirstName, a)
                .With(enrollment => enrollment.InsuranceCompany, companyName)
                .Create();

            var enrollment3 = fixture.Build<Enrollment>()
                .With(enrollment => enrollment.LastName, b)
                .With(enrollment => enrollment.FirstName, b)
                .With(enrollment => enrollment.InsuranceCompany, companyName)
                .Create();

            // insert out of order
            var enrollments = new List<Enrollment> { enrollment3, enrollment2, enrollment1 };

            // Act
            var result = _sut.SortEnrollmentsForCompany(enrollments, companyName);

            // Assert
            Assert.AreEqual(a, result.First().LastName);
            Assert.AreEqual(a, result.First().FirstName);
            Assert.AreEqual(b, result.Skip(1).First().LastName);
            Assert.AreEqual(a, result.Skip(1).First().FirstName);
            Assert.AreEqual(b, result.Skip(2).First().LastName);
            Assert.AreEqual(b, result.Skip(2).First().FirstName);
        }
    }
}
