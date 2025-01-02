using FluentAssertions;
using MarsRover.Input;
using MarsRover.Input.InputBuilders;
using MarsRover.Input.RequestExecutors;
using MarsRover.Model;
using MarsRover.Tests.TestArtifacts;

namespace MarsRover.Tests.Input.RequestExecutors
{
    public static class RoverRequestExecutorTests
    {
        public class Run
        {
            private static RoverRequestExecutor Arrange(string[] consoleInput)
            {
                var fakeConsole = new FakeConsole(consoleInput);
                List<Message> log = [];
                var roverBuilder = new PlateauBuilder(log)
                    .WithDimensions(5, 5);
                return new RoverRequestExecutor(roverBuilder, fakeConsole);
            }
            
            private static (RoverRequestExecutor, FakeConsole) Arrange2(IEnumerable<string> consoleInput)
            {
                var fakeConsole = new FakeConsole(consoleInput);
                List<Message> log = [];
                var roverBuilder = new PlateauBuilder(log)
                    .WithDimensions(5, 5);
                return (new RoverRequestExecutor(roverBuilder, fakeConsole), fakeConsole);
            }
            
            [Fact]
            public void Run_ShouldReturnExit_WhenUserTypesExit()
            {
                // Arrange
                var executor = Arrange(["exit"]);

                // Act
                var (result, builder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.Exit);
                builder.Should().BeNull();
                // Verify the console didn't produce errors, but we might not strictly need that
            }

            [Fact]
            public void Run_ShouldUseDefault_WhenInputIsEmpty()
            {
                // Arrange
                var (executor, fakeConsole) = Arrange2([""]);

                // Act
                var (result, roverCommandSeqBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.Continue);
                roverCommandSeqBuilder.Should().NotBeNull();

                // Because the default is "1 2 N", it should be valid within a 5x5 plateau
                // No error messages should appear
                fakeConsole.Output.Should()
                    .NotContain(x => x.Contains("error", StringComparison.OrdinalIgnoreCase));
            }

            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenInputIsMissingParts()
            {
                // e.g., "1 2" but missing direction
                // Arrange
                var (executor, fakeConsole) = Arrange2(["1 2"]);

                // Act
                var (result, roverCommandSeqBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                roverCommandSeqBuilder.Should().BeNull();

                // Check we printed an error message
                fakeConsole.Output.Should().Contain(line =>
                    line.Contains("Rover input must have three parts."));
            }

            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenXIsNonInteger()
            {
                // Arrange
                var (executor, fakeConsole) = Arrange2(["abc 2 N"]);

                // Act
                var (result, builder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                builder.Should().BeNull();
                fakeConsole.Output.Should()
                    .Contain(line => line.Contains("Rover X coordinate must be an integer."));
            }

            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenXIsOutOfRange()
            {
                // e.g. 10 is outside plateau width=5
                // Arrange
                var (executor, fakeConsole) = Arrange2(["10 2 N"]);

                // Act
                var (result, builder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                builder.Should().BeNull();
                fakeConsole.Output.Should()
                    .Contain(line =>
                        line.Contains("Rover X coordinate must be less than or equal to the plateau width."));
            }

            [Fact]
            public void Run_ShouldReturnInvalidInput_WhenDirectionIsInvalid()
            {
                // Arrange
                var (executor, fakeConsole) = Arrange2(["2 2 XYZ"]);

                // Act
                var (result, builder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                builder.Should().BeNull();
                fakeConsole.Output.Should()
                    .Contain(line => line.Contains("Rover direction must be a valid direction."));
            }

            [Fact]
            public void Run_ShouldReturnContinue_AndBuilder_WhenValidInput()
            {
                // Arrange
                var log = new List<Message>();
                var roverBuilder = new PlateauBuilder(log)
                    .WithDimensions(5, 5);
                var fakeConsole = new FakeConsole(["3 4 E"]);
                var executor = new RoverRequestExecutor(roverBuilder, fakeConsole);

                // Act
                var (result, cmdSeqBuilder) = executor.Run();

                // Assert
                result.Should().Be(RunOneCycleResult.Continue);
                cmdSeqBuilder.Should().NotBeNull();

                // We can also check the roverBuilder X, Y, Direction were set
                // Because the "WithRover" method sets the builder's X, Y, Direction
                roverBuilder.X.Should().Be(3);
                roverBuilder.Y.Should().Be(4);
                roverBuilder.Direction.Should().Be(Direction.E);
            }
        }
    }
}
