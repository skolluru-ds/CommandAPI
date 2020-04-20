using System;
using Xunit;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandTests : IDisposable
    {
        Command testCommand;

        public CommandTests()
        {
            testCommand = new Command
            {
                HowTo = "Do something awesome",
                Platform = "Some platform",
                CommandLine = "Some commandline"
            };
        }
        public void Dispose()
        {
            testCommand = null;
        }

        [Fact]
        public void CanChangeHowTo()
        {
            //Given or Arrange
                            
            //When or Act
            testCommand.HowTo = "Execute Unit Tests";

            //Then or Assert
            Assert.Equal("Execute Unit Tests", testCommand.HowTo);
        }
        [Fact]
        public void CanChangePlatform()
        {
            //Given or Arrange
                            
            //When or Act
            testCommand.Platform = "xUnit";

            //Then or Assert
            Assert.Equal("xUnit", testCommand.Platform);
        }
        [Fact]
        public void CanChangeCommandLine()
        {
            //Given or Arrange
                            
            //When or Act
            testCommand.CommandLine = "dotnet test";

            //Then or Assert
            Assert.Equal("dotnet test", testCommand.CommandLine);
        }

    }
}