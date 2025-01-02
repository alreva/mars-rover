using FluentAssertions;
using MarsRover.Input;
using MarsRover.Tests.TestArtifacts;

namespace MarsRover.Tests;

public class RoverConsoleUiTests
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
            "5 5",      // plateau OK
            "exit"      // rover = exit
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
            "5 5",      // plateau OK
            "1 2 N",    // rover OK
            "exit"      // instructions = exit
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
}