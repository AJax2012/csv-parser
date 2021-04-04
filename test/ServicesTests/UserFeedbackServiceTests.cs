using AutoFixture;
using Moq;
using NUnit.Framework;

using GardnerCsvParser.Contracts;
using GardnerCsvParser.Services;
using System;

namespace GardnerCsvParserTests.ServicesTests
{
    public class UserFeedbackServiceTests
    {
        private UserFeedbackService _sut;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IConsole> _consoleMock;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _fileServiceMock = new Mock<IFileService>();
            _consoleMock = new Mock<IConsole>();
            _sut = new UserFeedbackService(_fileServiceMock.Object, _consoleMock.Object);
            fixture = new Fixture();
        }

        [Test]
        public void GetInputFileLocation_CallsWriteTextForUserOnce_WhenFileFromUserIntputExists()
        {
            // Arrange
            var consoleOutput = "Please type the location of the file you'd like to parse.";
            var userInput = fixture.Create<string>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns(userInput);
            _fileServiceMock.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            _sut.GetInputFileLocation();

            // Assert
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(o => o == false)), Times.Once);
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput)), Times.Once);
            _fileServiceMock.Verify(f => f.FileExists(It.Is<string>(p => p == userInput)), Times.Once);
        }

        [Test]
        public void GetInputFileLocation_CallsWriteTextForUserTwice_WhenFileFromUserIntputNotExists()
        {
            // Arrange
            var consoleOutput1 = "Please type the location of the file you'd like to parse.";
            var consoleOutput2 = "Please select a valid file. File does not exist.";
            var userInput1 = fixture.Create<string>();
            var userInput2 = fixture.Create<string>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == false))).Returns(userInput1);
            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == true))).Returns(userInput2);
            _fileServiceMock.Setup(f => f.FileExists(It.Is<string>(p => p == userInput1))).Returns(false);
            _fileServiceMock.Setup(f => f.FileExists(It.Is<string>(p => p == userInput2))).Returns(true);

            // Act
            _sut.GetInputFileLocation();

            // Assert
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(o => o == false)), Times.Once);
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput1)), Times.Exactly(2));
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput2)), Times.Once);
            _fileServiceMock.Verify(f => f.FileExists(It.Is<string>(p => p == userInput1)), Times.Once);
            _fileServiceMock.Verify(f => f.FileExists(It.Is<string>(p => p == userInput2)), Times.Once);
        }

        [Test]
        public void GetInputFileLocation_ReturnsUserInput()
        {
            // Arrange
            var userInput = fixture.Create<string>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns(userInput);
            _fileServiceMock.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            var result = _sut.GetInputFileLocation();

            // Assert
            Assert.AreEqual(userInput, result);
        }

        [Test]
        public void GetRowSeparator_CallsWriteTextForUserOnce_WhenFileFromUserIntputExists()
        {
            // Arrange
            var consoleOutput = "Please type the character that separates the rows (eg. ';' or '\\n'";
            var userInput = fixture.Create<char>().ToString();

            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns(userInput);

            // Act
            _sut.GetRowSeparator();

            // Assert
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(o => o == false)), Times.Once);
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput)), Times.Once);
        }

        [Test]
        public void GetRowSeparator_CallsWriteTextForUserTwice_WhenFileFromUserIntputNotExists()
        {
            // Arrange
            var consoleOutput1 = "Please type the character that separates the rows (eg. ';' or '\\n'";
            var consoleOutput2 = "Not a valid character. Please try again.";
            var userInput1 = fixture.Create<string>();
            var userInput2 = fixture.Create<char>().ToString();

            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == false))).Returns(userInput1);
            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == true))).Returns(userInput2);

            // Act
            _sut.GetRowSeparator();

            // Assert
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(o => o == false)), Times.Once);
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput1)), Times.Exactly(2));
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput2)), Times.Once);
        }

        [Test]
        public void GetRowSeparator_ReturnsUserInput()
        {
            // Arrange
            var userInput = fixture.Create<char>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns(userInput.ToString());

            // Act
            var result = _sut.GetRowSeparator();

            // Assert
            Assert.AreEqual(userInput, result);
        }

        [Test]
        public void HeaderCountNotSameAsPropertyCount_ContinuesAfterYesUserInput()
        {
            // Arrange
            var headerCount = 1;
            var propertyCount = 2;
            var expectedOutput = $"The amount of properties that will be mapped are not equal to the amount of properties in the headers line of the CSV file. Properties to be mapped: {propertyCount}; Properties in header: {headerCount}. Would you like to continue? y/N";


            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns("y");

            // Act
            _sut.HeaderCountNotSameAsPropertyCount(headerCount, propertyCount);

            // Assert
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(x => x == expectedOutput)), Times.Once);
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(x => x == false)), Times.Once);
        }

        [Test]
        public void HeaderCountNotSameAsPropertyCount_ThrowsExceptionIfUserResponse()
        {
            // Arrange
            var headerCount = 1;
            var propertyCount = 2;
            var expectedOutput = "You have chosen to exit the program and fix the CSV File.";


            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns("n");

            // Act
            // Assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.HeaderCountNotSameAsPropertyCount(headerCount, propertyCount));

            Assert.AreEqual(expectedOutput, exception.Message);
        }

        [Test]
        public void HeaderCountNotSameAsPropertyCount_RepeatsWhenInvalidEntry()
        {
            // Arrange
            var headerCount = 1;
            var propertyCount = 2;
            var expectedOutput1 = $"The amount of properties that will be mapped are not equal to the amount of properties in the headers line of the CSV file. Properties to be mapped: {propertyCount}; Properties in header: {headerCount}. Would you like to continue? y/N";
            var expectedOutput2 = "Not a valid response. Please try again.";


            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == false))).Returns("k");
            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == true))).Returns("y");

            // Act
            _sut.HeaderCountNotSameAsPropertyCount(headerCount, propertyCount);

            // Assert
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(x => x == expectedOutput1)), Times.Exactly(2));
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(x => x == expectedOutput2)), Times.Once);
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(x => x == false)), Times.Once);
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(x => x == true)), Times.Once);
        }

        [Test]
        [TestCase("y", true)]
        [TestCase("n", false)]
        [TestCase("k", null)]
        public void ParseContinueResponse_ReturnsExpectedValues(string result, bool? expected)
        {
            // Act
            var response = _sut.ParseContinueResponse(result);

            // Assert
            Assert.AreEqual(expected, response);
        }
    }
}
