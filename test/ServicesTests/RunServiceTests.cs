using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;
using Moq;
using NUnit.Framework;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Models;
using GardnerCsvParser.Services;

namespace GardnerCsvParserTests.ServicesTests
{
    public class RunServiceTests
    {
        private RunService _sut;
        private Mock<ICsvService> _csvServiceMock;
        private Mock<IEnrollmentObjectService> _enrollmentServiceMock;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IUserFeedbackService> _userFeedbackServiceMock;
        private Mock<IJsonService> _jsonServiceMock;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _csvServiceMock = new Mock<ICsvService>();
            _enrollmentServiceMock = new Mock<IEnrollmentObjectService>();
            _fileServiceMock = new Mock<IFileService>();
            _userFeedbackServiceMock = new Mock<IUserFeedbackService>();
            _jsonServiceMock = new Mock<IJsonService>();

            _sut = new RunService(
                _csvServiceMock.Object,
                _enrollmentServiceMock.Object,
                _fileServiceMock.Object,
                _userFeedbackServiceMock.Object,
                _jsonServiceMock.Object);

            fixture = new Fixture();
        }

        [Test]
        public void GetUserInputValues_CallsCorrectMethodsAndReturnsUserInputDto()
        {
            // Arrange
            var inputFilePath = fixture.Create<string>();
            var ouputFilePath = fixture.Create<string>();
            var rowSeparator = fixture.Create<char>();

            _userFeedbackServiceMock.Setup(s => s.GetInputFileLocation()).Returns(inputFilePath);
            _fileServiceMock.Setup(s => s.GetOutputDirectory(It.IsAny<string>())).Returns(ouputFilePath);
            _userFeedbackServiceMock.Setup(s => s.GetRowSeparator()).Returns(rowSeparator);

            // Act
            var response = _sut.GetUserInputValues(string.Empty, default(char));

            // Assert
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Once);
            _fileServiceMock.Verify(s => s.GetOutputDirectory(It.Is<string>(y => y == inputFilePath)), Times.Once);
            _userFeedbackServiceMock.Verify(s => s.GetRowSeparator(), Times.Once);

            Assert.AreEqual(inputFilePath, response.InputFilePath);
            Assert.AreEqual(ouputFilePath, response.OutputDirectory);
            Assert.AreEqual(rowSeparator, response.RowSeparator);
        }

        [Test]
        public void GetInputFilePath_ReturnsInput_WhenNotNullOrEmtpyAndFileExists()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            _fileServiceMock.Setup(s => s.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            var response = _sut.GetInputFilePath(filePath);

            // Assert
            Assert.AreEqual(filePath, response);
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Never);
        }

        [Test]
        public void GetInputFilePath_ReturnsUserInput_WhenFileNotExists()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            var userInput = fixture.Create<string>();
            _fileServiceMock.Setup(s => s.FileExists(It.IsAny<string>())).Returns(false);
            _userFeedbackServiceMock.Setup(s => s.GetInputFileLocation()).Returns(userInput);

            // Act
            var response = _sut.GetInputFilePath(filePath);

            // Assert
            Assert.AreEqual(userInput, response);
            _fileServiceMock.Verify(s => s.FileExists(It.Is<string>(y => y == filePath)), Times.Once);
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Once);
        }

        [Test]
        public void GetInputFilePath_ReturnsUserInput_WhenNullOrEmtpyAndFileExists()
        {
            // Arrange
            var userInput = fixture.Create<string>();
            _userFeedbackServiceMock.Setup(s => s.GetInputFileLocation()).Returns(userInput);

            // Act
            var response = _sut.GetInputFilePath(string.Empty);

            //
            Assert.AreEqual(userInput, response);
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Once);
        }

        [Test]
        public void GetRowSeperator_ReturnsInput_WhenNotNullOrEmtpy()
        {
            // Arrange
            var filePath = fixture.Create<char>();

            // Act
            var response = _sut.GetRowSeperator(filePath);

            //
            Assert.AreEqual(filePath, response);
            _userFeedbackServiceMock.Verify(s => s.GetRowSeparator(), Times.Never);
        }

        [Test]
        public void GetRowSeperator_ReturnsUserInput_WhenDefault()
        {
            // Arrange
            var userInput = fixture.Create<char>();
            _userFeedbackServiceMock.Setup(s => s.GetRowSeparator()).Returns(userInput);

            // Act
            var response = _sut.GetRowSeperator(default(char));

            //
            Assert.AreEqual(userInput, response);
            _userFeedbackServiceMock.Verify(s => s.GetRowSeparator(), Times.Once);
        }

        [Test]
        public void GetFileContents_ThrowsExcption_WhenFileContentsEmpty()
        {
            // Arrange
            var inputFilePath = fixture.Create<string>();
            var fileContents = string.Empty;
            var expectedMessage = "Csv file cannot be empty.";

            _fileServiceMock.Setup(s => s.GetContents(It.IsAny<string>())).ReturnsAsync(fileContents);

            // Act
            // Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _sut.GetFileContents(inputFilePath));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public async Task GetFileContents_ReturnsFileContents_WhenNotEmpty()
        {
            // Arrange
            var inputFilePath = fixture.Create<string>();
            var fileContents = fixture.Create<string>();

            _fileServiceMock.Setup(s => s.GetContents(It.IsAny<string>())).ReturnsAsync(fileContents);

            // Act
            var result = await _sut.GetFileContents(inputFilePath);

            // Assert
            _fileServiceMock.Verify(s => s.GetContents(It.Is<string>(x => x == inputFilePath)), Times.Once);
            Assert.AreEqual(fileContents, result);
        }

        [Test]
        public void GetEnrollmentRows_ThrowsExcption_WhenNotHeaderRow()
        {
            // Arrange
            var fileContents = fixture.Create<string>();
            var rowSeparator = fixture.Create<char>();
            var enrollmentRows = fixture.CreateMany<string>(1).ToList();
            var expectedMessage = "Csv file must have more than 1 row.";

            _csvServiceMock.Setup(s => s.SeparateIntoRows(It.IsAny<string>(), It.IsAny<char>())).Returns(enrollmentRows);

            // Act
            // Assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetEnrollmentRows(fileContents, rowSeparator));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetEnrollmentRows_ReturnsHeaderRow_WhenIsHeaderRow()
        {
            // Arrange
            var fileContents = fixture.Create<string>();
            var rowSeparator = fixture.Create<char>();
            var enrollmentRows = fixture.CreateMany<string>(3).ToList();

            _csvServiceMock.Setup(s => s.SeparateIntoRows(It.IsAny<string>(), It.IsAny<char>())).Returns(enrollmentRows);

            // Act
            var result = _sut.GetEnrollmentRows(fileContents, rowSeparator);

            // Assert
            _csvServiceMock.Verify(s => s.SeparateIntoRows(It.Is<string>(x => x == fileContents), It.Is<char>(x => x == rowSeparator)), Times.Once);
            Assert.AreEqual(enrollmentRows, result);
        }

        [Test]
        public void GetHeaderRow_ThrowsExcption_WhenNotHeaderRow()
        {
            // Arrange
            var enrollmentRows = fixture.Create<List<string>>();
            var headers = new List<string>();
            var expectedMessage = "Fist row of CSV file must contain vaild headers.";

            _csvServiceMock.Setup(s => s.GetAssumedHeaderRow(It.IsAny<List<string>>())).Returns(headers);
            _csvServiceMock.Setup(s => s.IsHeaderRow(It.IsAny<List<string>>())).Returns(false);

            // Act
            // Assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetHeaderRow(enrollmentRows));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetHeaderRow_ReturnsHeaderRow_WhenIsHeaderRow()
        {
            // Arrange
            var enrollmentRows = fixture.CreateMany<string>(3).ToList();
            var headers = fixture.CreateMany<string>(3).ToList();
            var properties = PropertyInfoTestHelpers.GetTestProperties();

            _csvServiceMock.Setup(s => s.GetAssumedHeaderRow(It.IsAny<List<string>>())).Returns(headers);
            _csvServiceMock.Setup(s => s.IsHeaderRow(It.IsAny<List<string>>())).Returns(true);
            _enrollmentServiceMock.Setup(s => s.GetEnrollmentProperties()).Returns(properties);

            // Act
            var result = _sut.GetHeaderRow(enrollmentRows);

            // Assert
            _csvServiceMock.Verify(s => s.GetAssumedHeaderRow(It.Is<List<string>>(x => x == enrollmentRows)), Times.Once);
            _csvServiceMock.Verify(s => s.IsHeaderRow(It.Is<List<string>>(x => x == headers)), Times.Once);
            _enrollmentServiceMock.Verify(s => s.GetEnrollmentProperties(), Times.Once);

            _userFeedbackServiceMock.Verify(s => s.HeaderCountNotSameAsPropertyCount(It.Is<int>(x => x == 3), It.Is<int>(x => x == 5)));
            Assert.AreEqual(headers, result);
        }

        [Test]
        public void CleanCsv_CallsTwoMethodsInCsvService()
        {
            // Arrange
            var enrollmentRows = fixture.Create<List<string>>();// Act

            // Act
            _sut.CleanCsv(enrollmentRows);

            // Assert
            _csvServiceMock.Verify(s => s.RemoveHeaderRow(It.Is<List<string>>(x => x == enrollmentRows)), Times.Once);
            _csvServiceMock.Verify(s => s.RemoveEmptyRows(It.Is<List<string>>(x => x == enrollmentRows)), Times.Once);
        }

        [Test]
        public void ValidateRowLengths_ThrowsException_WhenRowNotSameLengthAsHeaders()
        {
            // Arrange
            var headerRowCount = 3;
            var rows = new List<string> 
            {
                "1,2,3",
                "1,2"
            };

            var expectedErrorMessage = "Header row cannot be different length than other rows. Rows which require fixing (with header row being 0): 2";

            // Act
            // Assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ValidateRowLengths(rows, headerRowCount));
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        public void GetEnrollments_CallsCorrectServicesAndReturnsEnrollments()
        {
            // Arrange
            var indexes = PropertyInfoTestHelpers.GetTestIndexs();
            var rows = fixture.CreateMany<string>(3).ToList();
            var enrollments = fixture.CreateMany<Enrollment>(3);

            _csvServiceMock.Setup(s => s.ParseRows(It.IsAny<Dictionary<PropertyInfo, int>>(), It.IsAny<List<string>>())).Returns(enrollments);

            _enrollmentServiceMock.Setup(x => x.GetEnrollmentOutput(It.IsAny<IEnumerable<Enrollment>>())).Returns(enrollments);

            // Act
            var result = _sut.GetEnrollments(indexes, rows);

            // Assert
            _csvServiceMock.Verify(s => s.ParseRows(It.Is<Dictionary<PropertyInfo, int>>(x => x == indexes), It.Is<List<string>>(x => x == rows)), Times.Once);
            _enrollmentServiceMock.Verify(s => s.GetEnrollmentOutput(It.IsAny<IEnumerable<Enrollment>>()), Times.Once);

            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task CreateResponse_CallsAppropriateServiceMethods()
        {
            // Arrange
            var enrollments = fixture.CreateMany<Enrollment>(3);
            var ouputDirectory = fixture.Create<string>();
            var outputEnrollments = fixture.Create<string>();
            var ouputFilePath = fixture.Create<string>();

            _fileServiceMock.Setup(s => s.GetOuputFilePath(It.IsAny<string>(), It.IsAny<string>())).Returns(ouputFilePath);
            _enrollmentServiceMock.Setup(s => s.SortEnrollmentsForCompany(It.IsAny<IEnumerable<Enrollment>>(), It.IsAny<string>()));
            _jsonServiceMock.Setup(s => s.SerializeEnrollmentsToJson(It.IsAny<List<Enrollment>>())).Returns(outputEnrollments);

            // Act
            await _sut.CreateResponse(enrollments, ouputDirectory, new CancellationToken());

            // Assert
            _fileServiceMock.Verify(s => s.GetOuputFilePath(It.Is<string>(x => x == ouputDirectory), It.IsAny<string>()), Times.Exactly(3));
            _enrollmentServiceMock.Verify(s => s.SortEnrollmentsForCompany(It.IsAny<IEnumerable<Enrollment>>(), It.IsAny<string>()), Times.Exactly(3));
            _jsonServiceMock.Verify(s => s.SerializeEnrollmentsToJson(It.IsAny<List<Enrollment>>()), Times.Exactly(3));
            _fileServiceMock.Verify(s => s.WriteOutputFile(It.Is<string>(x => x == outputEnrollments), It.Is<string>(x => x == ouputFilePath), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }
    }
}
