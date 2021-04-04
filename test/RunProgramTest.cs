using AutoFixture;
using GardnerCsvParser;
using GardnerCsvParser.Contracts;
using GardnerCsvParser.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GardnerCsvParserTests
{
    public class RunProgramTest
    {
        private RunProgram _sut;
        private Mock<IRunService> _runServiceMock;
        private Mock<IEnrollmentObjectService> _enrollmentSerivceMock;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _runServiceMock = new Mock<IRunService>();
            _enrollmentSerivceMock = new Mock<IEnrollmentObjectService>();
            _sut = new RunProgram(_runServiceMock.Object, _enrollmentSerivceMock.Object);
            fixture = new Fixture();
        }

        [Test]
        public async Task RunAsync_CallsAppropriateMethods()
        {
            // Arrange
            var userInputDto = fixture.Create<UserInputDto>();
            var fileContents = fixture.Create<string>();
            var enrollmentRows = fixture.CreateMany<string>(3);
            var headers = fixture.CreateMany<string>(3);
            var indexes = PropertyInfoTestHelpers.GetTestIndexs();
            var enrollments = fixture.CreateMany<Enrollment>(3);

            _runServiceMock.Setup(s => s.GetUserInputValues(It.IsAny<string>(), It.IsAny<char>())).Returns(userInputDto);
            _runServiceMock.Setup(s => s.GetFileContents(It.IsAny<string>())).ReturnsAsync(fileContents);
            _runServiceMock.Setup(s => s.GetEnrollmentRows(It.IsAny<string>(), It.IsAny<char>())).Returns(enrollmentRows.ToList());
            _runServiceMock.Setup(s => s.GetHeaderRow(It.IsAny<List<string>>())).Returns(headers.ToList());
            _enrollmentSerivceMock.Setup(s => s.GetIndexValues(It.IsAny<List<string>>())).Returns(indexes);
            _runServiceMock.Setup(s => s.GetEnrollments(It.IsAny<Dictionary<PropertyInfo, int>>(), It.IsAny<List<string>>())).Returns(enrollments);

            // Act
            await _sut.RunAsync(new string[] { }, new CancellationToken());

            // Assert
            _runServiceMock.Verify(s => s.GetUserInputValues(It.IsAny<string>(), It.IsAny<char>()), Times.Once);
            _runServiceMock.Verify(s => s.GetFileContents(It.Is<string>(x => x == userInputDto.InputFilePath)), Times.Once);
            _runServiceMock.Verify(s => s.GetEnrollmentRows(It.Is<string>(x => x == fileContents), It.Is<char>(x => x == userInputDto.RowSeparator)), Times.Once);
            _runServiceMock.Verify(s => s.GetHeaderRow(It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList()))), Times.Once);
            _runServiceMock.Verify(s => s.CleanCsv(It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList()))), Times.Once);
            _runServiceMock.Verify(s => s.ValidateRowLengths(It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList())), It.Is<int>(x => x == 3)), Times.Once);
            _enrollmentSerivceMock.Setup(s => s.GetIndexValues(It.Is<List<string>>(x => x == headers.ToList())));
            _runServiceMock.Verify(s => s.GetEnrollments(It.IsAny<Dictionary<PropertyInfo, int>>(), It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList()))), Times.Once);
            _runServiceMock.Verify(s => s.CreateResponse(It.IsAny<IEnumerable<Enrollment>>(), It.Is<string>(x => x == userInputDto.OutputDirectory), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RunAsync_ParsesArgsCorrectly()
        {
            // Arrange
            var fileInput = fixture.Create<string>();
            var seperator = fixture.Create<char>();
            var userInputDto = fixture.Create<UserInputDto>();
            var fileContents = fixture.Create<string>();
            var enrollmentRows = fixture.CreateMany<string>(3);
            var headers = fixture.CreateMany<string>(3);
            var indexes = PropertyInfoTestHelpers.GetTestIndexs();
            var enrollments = fixture.CreateMany<Enrollment>(3);

            var args = new string[]
            {
                fileInput, seperator.ToString()
            };

            _runServiceMock.Setup(s => s.GetUserInputValues(It.IsAny<string>(), It.IsAny<char>())).Returns(userInputDto);
            _runServiceMock.Setup(s => s.GetFileContents(It.IsAny<string>())).ReturnsAsync(fileContents);
            _runServiceMock.Setup(s => s.GetEnrollmentRows(It.IsAny<string>(), It.IsAny<char>())).Returns(enrollmentRows.ToList());
            _runServiceMock.Setup(s => s.GetHeaderRow(It.IsAny<List<string>>())).Returns(headers.ToList());
            _enrollmentSerivceMock.Setup(s => s.GetIndexValues(It.IsAny<List<string>>())).Returns(indexes);
            _runServiceMock.Setup(s => s.GetEnrollments(It.IsAny<Dictionary<PropertyInfo, int>>(), It.IsAny<List<string>>())).Returns(enrollments);

            // Act
            await _sut.RunAsync(args, new CancellationToken());

            // Assert
            _runServiceMock.Verify(s => s.GetUserInputValues(It.Is<string>(x => x == fileInput), It.Is<char>(x => x == seperator)), Times.Once);
            _runServiceMock.Verify(s => s.GetFileContents(It.Is<string>(x => x == userInputDto.InputFilePath)), Times.Once);
            _runServiceMock.Verify(s => s.GetEnrollmentRows(It.Is<string>(x => x == fileContents), It.Is<char>(x => x == userInputDto.RowSeparator)), Times.Once);
            _runServiceMock.Verify(s => s.GetHeaderRow(It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList()))), Times.Once);
            _runServiceMock.Verify(s => s.CleanCsv(It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList()))), Times.Once);
            _runServiceMock.Verify(s => s.ValidateRowLengths(It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList())), It.Is<int>(x => x == 3)), Times.Once);
            _enrollmentSerivceMock.Setup(s => s.GetIndexValues(It.Is<List<string>>(x => x == headers.ToList())));
            _runServiceMock.Verify(s => s.GetEnrollments(It.IsAny<Dictionary<PropertyInfo, int>>(), It.Is<List<string>>(x => x.SequenceEqual(enrollmentRows.ToList()))), Times.Once);
            _runServiceMock.Verify(s => s.CreateResponse(It.IsAny<IEnumerable<Enrollment>>(), It.Is<string>(x => x == userInputDto.OutputDirectory), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
