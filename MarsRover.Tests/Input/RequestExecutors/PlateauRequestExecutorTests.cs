using FluentAssertions;
using MarsRover.Input;
using MarsRover.Input.InputBuilders;
using MarsRover.Input.RequestExecutors;
using MarsRover.Tests.TestArtifacts;

namespace MarsRover.Tests.Input.RequestExecutors;

public static class PlateauRequestExecutorTests
{
    public static class Run
    {
        public class ShouldExit
        {
            [Fact]
            public void Run_ShouldReturnExit_WhenUserTypesExit()
            {
                // Arrange
                var fakeConsole = new FakeConsole(["exit"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);
            
                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.Exit);
                roverBuilder.Should().BeNull();
            }
        }
        
        public class ShouldRequestAgain
        {
            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenInputFailsValidation()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["abc def"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("must be an integer"));
            }

            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenInputWidthLessThanOne()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["0 100"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("width must be greater than 0"));
            }
            
            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenInputHeightIsLessThanOne()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["100 0"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("height must be greater than 0"));
            }
            
            [Fact]
            public void Run_ShouldHaveTwoErrors_WhenInputBothWidthAndHeightAreTooLow()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["0 -1"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("width must be greater than 0"));
                fakeConsole.Output.Should().Contain(line => line.Contains("height must be greater than 0"));
            }

            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenInputWidthIsTooHigh()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["101 100"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("width must be less than or equal to 100"));
            }
            
            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenInputHeightIsTooHigh()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["100 101"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("height must be less than or equal to 100"));
            }
            
            [Fact]
            public void Run_ShouldHaveTwoErrors_WhenInputBothWidthAndHeightAreTooHigh()
            {
                // e.g., user input "abc def"
                var fakeConsole = new FakeConsole(["101 101"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverBuilder.Should().BeNull();
                fakeConsole.Output.Should().Contain(line => line.Contains("width must be less than or equal to 100"));
                fakeConsole.Output.Should().Contain(line => line.Contains("height must be less than or equal to 100"));
            }
        }
        
        public class ShouldContinue
        {
            [Fact]
            public void Run_ShouldUseDefault_WhenUserInputIsEmpty()
            {
                // Arrange
                var fakeConsole = new FakeConsole([""]); // empty => use default "5 5"
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);
            
                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                // The default input "5 5" is valid => no errors
                result.Should().Be(RunOneCycleResult.Continue);
                roverBuilder.Should().NotBeNull();
                builder.Width.Should().Be(5);
                builder.Height.Should().Be(5);
            }
            
            [Fact]
            public void Run_ShouldReturnContinue_AndValidBuilder_OnValidInput()
            {
                // e.g., "10 11"
                var fakeConsole = new FakeConsole(["10 11"]);
                var builder = new PlateauBuilder([]);
                var executor = new PlateauRequestExecutor(builder, fakeConsole);

                // Act
                var (result, roverBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.Continue);
                roverBuilder.Should().NotBeNull();
                builder.Width.Should().Be(10);
                builder.Height.Should().Be(11);
            }
        }
    }
}