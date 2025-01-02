using FluentAssertions;
using MarsRover.Input;
using MarsRover.Tests.TestArtifacts;

namespace MarsRover.Tests.Input;

public static class RoverConsoleUiTests
{
    public static class RunOneCycle
    {
        public class ShouldExit
        {
            [Fact]
            public void RunOneCycle_ShouldExit_WhenPlateauPhaseIsExit()
            {
                // Arrange
                var console = new FakeConsole(["exit"]);
                var ui = new RoverConsoleUi(console);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.Exit);
                console.Output.Should().BeEquivalentTo(
                    "Enter the plateau dimensions (default '5 5'):"
                );
            }

            [Fact]
            public void RunOneCycle_ShouldExit_WhenRoverPhaseIsExit()
            {
                // Arrange
                var fakeConsole = new FakeConsole(new[]
                {
                    "5 5", // plateau OK
                    "exit" // rover = exit
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.Exit);
                fakeConsole.Output.Should().BeEquivalentTo(
                    "Enter the plateau dimensions (default '5 5'):",
                    "Enter the rover's starting position (default '1 2 N'):"
                );
            }

            [Fact]
            public void RunOneCycle_ShouldExit_WhenInstructionsPhaseIsExit()
            {
                // Arrange
                var fakeConsole = new FakeConsole(new[]
                {
                    "5 5", // plateau OK
                    "1 2 N", // rover OK
                    "exit" // instructions = exit
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.Exit);
                fakeConsole.Output.Should().BeEquivalentTo(
                    "Enter the plateau dimensions (default '5 5'):",
                    "Enter the rover's starting position (default '1 2 N'):",
                    "Enter the rover's instructions (default 'LMLMLMLMM'):"
                );
            }
            
            [Fact]
            public void RunOneCycle_ShouldExit_WhenWeProvideValidInputs_AndThenRefuseNextRound()
            {
                // Arrange
                var fakeConsole = new FakeConsole(new[]
                {
                    "5 5",        // plateau
                    "1 2 N",      // rover
                    "LMLMLMLMM",  // instructions
                    // after simulation, we get "Do you want to continue? (y/n)"
                    "n"           // say "no" => exit
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                // The cycle ends by user choosing 'n' at the end
                result.Should().Be(RunOneCycleResult.Exit);
                // We can also confirm the output has certain lines
                fakeConsole.Output.Should().Contain(line => line.Contains("Simulation complete"));
            }
        }
        
        public class ShouldRequestAgain
        {
            [Fact]
            public void RunOneCycle_ShouldRequestAgain_WhenPlateauLessThanTwoParameters()
            {
                // In real usage, you'd loop in Program.cs. For a single cycle:
                var fakeConsole = new FakeConsole(new[]
                {
                    "single",  // invalid => should yield InvalidInput, then method returns
                    // The cycle ends, no further input needed
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                fakeConsole.Output.Should().Contain(line => line.Contains("input must have two parts"));
            }

            [Fact]
            public void RunOneCycle_ShouldRequestAgain_WhenPlateauMoreThanTwoParameters()
            {
                // In real usage, you'd loop in Program.cs. For a single cycle:
                var fakeConsole = new FakeConsole(new[]
                {
                    "fst snd thd",  // invalid => should yield InvalidInput, then method returns
                    // The cycle ends, no further input needed
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                fakeConsole.Output.Should().Contain(line => line.Contains("input must have two parts"));
            }

            [Fact]
            public void RunOneCycle_ShouldRequestAgain_WhenPlateauWidthNotInteger()
            {
                // In real usage, you'd loop in Program.cs. For a single cycle:
                var fakeConsole = new FakeConsole(new[]
                {
                    "abc 1",  // invalid => should yield InvalidInput, then method returns
                    // The cycle ends, no further input needed
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                fakeConsole.Output.Should().Contain(line => line.Contains("width must be an integer"));
            }
            
            [Fact]
            public void RunOneCycle_ShouldRequestAgain_WhenPlateauHeightNotInteger()
            {
                // In real usage, you'd loop in Program.cs. For a single cycle:
                var fakeConsole = new FakeConsole(new[]
                {
                    "1 def",  // invalid => should yield InvalidInput, then method returns
                    // The cycle ends, no further input needed
                });
                var ui = new RoverConsoleUi(fakeConsole);

                // Act
                var result = ui.RunOneCycle();

                // Assert
                result.Should().Be(RunOneCycleResult.InvalidInput);
                fakeConsole.Output.Should().Contain(line => line.Contains("height must be an integer"));
            }
        }
    }
}