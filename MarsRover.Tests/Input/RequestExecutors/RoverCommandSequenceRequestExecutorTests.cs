using FluentAssertions;
using MarsRover.Input;
using MarsRover.Input.InputBuilders;
using MarsRover.Input.RequestExecutors;
using MarsRover.Model;
using MarsRover.Tests.TestArtifacts;

namespace MarsRover.Tests.Input.RequestExecutors
{
    public class RoverCommandSequenceRequestExecutorTests
    {
        /// <summary>
        /// Helper method to build a basic RoverCommandSequenceBuilder 
        /// with a 5x5 plateau, a rover at (1,2,N).
        /// You can adjust as needed if you want different defaults.
        /// </summary>
        private static RoverCommandSequenceBuilder CreateDefaultRoverCommandSequenceBuilder(
            List<Message> log)
        {
            return new PlateauBuilder(log)
                .WithDimensions(5, 5)
                .WithRover(1, 2, Direction.N);
        }

        [Fact]
        public void Run_ShouldReturnExit_WhenUserTypesExit()
        {
            // Arrange
            var fakeConsole = new FakeConsole(["exit"]);
            var log = new List<Message>();
            var rcsBuilder = CreateDefaultRoverCommandSequenceBuilder(log);
            
            var executor = new RoverCommandSequenceRequestExecutor(rcsBuilder, fakeConsole);

            // Act
            var (result, plateau) = executor.Run();

            // Assert
            result.Should().Be(RunOneCycleResult.Exit);
            plateau.Should().BeNull();
            // We don't expect any error messages, just an exit path
        }

        [Fact]
        public void Run_ShouldUseDefault_WhenInputIsEmpty()
        {
            // The default is presumably "LMLMLMLMM" from DefaultInputs.RoverCommands
            var fakeConsole = new FakeConsole([""]); 
            var log = new List<Message>();
            var rcsBuilder = CreateDefaultRoverCommandSequenceBuilder(log);
            var executor = new RoverCommandSequenceRequestExecutor(rcsBuilder, fakeConsole);

            // Act
            var (result, plateau) = executor.Run();

            // Assert
            result.Should().Be(RunOneCycleResult.Continue);
            plateau.Should().NotBeNull("because the default instructions are valid");

            // (Optional) check the plateau has exactly 1 rover, 
            // and the rover has the instructions you expect:
            plateau!.Rovers.Count.Should().Be(1);
            var rover = plateau.Rovers[0];
            rover.Instructions.Should().BeEquivalentTo(
                new[] { 
                    Instruction.Left, Instruction.Move, Instruction.Left, 
                    Instruction.Move, Instruction.Left, Instruction.Move, 
                    Instruction.Left, Instruction.Move, Instruction.Move
                },
                because: "LMLMLMLMM => L, M, L, M, L, M, L, M, M"
            );
        }

        [Fact]
        public void Run_ShouldReturnInvalidInput_WhenInstructionHasInvalidCharacters()
        {
            // e.g. "LMBR" (B is invalid)
            var fakeConsole = new FakeConsole(["LMBR"]);
            var log = new List<Message>();
            var rcsBuilder = CreateDefaultRoverCommandSequenceBuilder(log);
            var executor = new RoverCommandSequenceRequestExecutor(rcsBuilder, fakeConsole);

            // Act
            var (result, plateau) = executor.Run();

            // Assert
            result.Should().Be(RunOneCycleResult.InvalidInput);
            plateau.Should().BeNull();
            fakeConsole.Output.Should().Contain(line =>
                line.Contains("Invalid instruction 'B'")
            );
        }

        [Fact]
        public void Run_ShouldReturnContinue_AndPlateau_WhenInstructionsAreValid()
        {
            // e.g., "LRM" => Left, Right, Move
            var fakeConsole = new FakeConsole(["LRM"]);
            var log = new List<Message>();
            var rcsBuilder = CreateDefaultRoverCommandSequenceBuilder(log);
            var executor = new RoverCommandSequenceRequestExecutor(rcsBuilder, fakeConsole);

            // Act
            var (result, plateau) = executor.Run();

            // Assert
            result.Should().Be(RunOneCycleResult.Continue);
            plateau.Should().NotBeNull();

            // Check the plateau has the rover with the instructions
            plateau!.Rovers.Count.Should().Be(1);
            var rover = plateau.Rovers[0];
            rover.Instructions.Should().BeEquivalentTo(
                new[] { Instruction.Left, Instruction.Right, Instruction.Move });
        }

        [Fact]
        public void Run_ShouldReturnContinue_AndPlateau_WhenUserInputsMultipleMoves()
        {
            // e.g. "MMMM" => 4 moves
            var fakeConsole = new FakeConsole(["MMMM"]);
            var log = new List<Message>();
            var rcsBuilder = CreateDefaultRoverCommandSequenceBuilder(log);
            var executor = new RoverCommandSequenceRequestExecutor(rcsBuilder, fakeConsole);

            // Act
            var (result, plateau) = executor.Run();

            // Assert
            result.Should().Be(RunOneCycleResult.Continue);
            plateau.Should().NotBeNull();

            plateau!.Rovers.Count.Should().Be(1);
            var rover = plateau.Rovers[0];
            rover.Instructions.Should().BeEquivalentTo(
                new[] { Instruction.Move, Instruction.Move, Instruction.Move, Instruction.Move });
        }
    }
}
