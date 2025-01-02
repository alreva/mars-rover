using FluentAssertions;
using MarsRover.Model;

namespace MarsRover.Tests.Model;

public class PlateauTests
{
    [Fact]
    public void ExecuteInstructions_ShouldDoNothing_WhenNoRovers()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);

        // Act
        plateau.ExecuteInstructions();

        // Assert
        // We expect a log message stating no rovers
        log.Should().Contain(m => m.ToString().Contains("No rovers to execute"));
    }

    [Fact]
    public void ExecuteInstructions_ShouldStop_WhenRoversHaveNoMoves()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);

        // Rover with no instructions:
        plateau.AddRover(
            new Position(new Coordinates(2, 2), Direction.N),
            []
        );

        // Act
        plateau.ExecuteInstructions();

        // Assert
        // The plateau should log that rovers have no moves
        log.Should().Contain(m => m.ToString().Contains("All rovers have no moves remaining. Simulation cannot run."));
    }

    [Fact]
    public void ExecuteInstructions_ShouldRunUntilRoversFinishAllInstructions()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);

        // Single rover with 3 instructions: M, L, M
        plateau.AddRover(
            new Position(new Coordinates(1, 2), Direction.N),
            [Instruction.Move, Instruction.Left, Instruction.Move]
        );

        // Act
        plateau.ExecuteInstructions();

        // Assert
        // Final position should be (0,3) facing West
        var rover = plateau.Rovers[0];
        rover.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(0, 3));
        rover.Position.Direction.Should().Be(Direction.W);

        // Also confirm the log mentions ticks
        log.Should().Contain(m => m.ToString().Contains("Starting tick 0"));
        log.Should().Contain(m => m.ToString().Contains("Starting tick 1"));
        log.Should().Contain(m => m.ToString().Contains("Starting tick 2"));
    }

    [Fact]
    public void ExecuteInstructions_ShouldStop_WhenNoRoversMovedInATick()
    {
        // This scenario often happens if rovers are blocked by edges or collisions
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);

        plateau.AddRover(
            new Position(new Coordinates(0, 5), Direction.N),
            [Instruction.Move, Instruction.Move]
        );

        // Act
        plateau.ExecuteInstructions();

        // Assert
        // The rover can't move north from y=5 => each tick tries but fails => eventually no movement => stop
        log.Should().Contain(m => m.ToString().Contains("No rovers moved during this tick. Simulation will stop."));
    }

    [Fact]
    public void ExecuteInstructions_ShouldHandleMultipleRovers()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);

        // Rover A: starts at (1,2,N), instructions: M, L, M
        plateau.AddRover(
            new Position(new Coordinates(1, 2), Direction.N),
            [Instruction.Move, Instruction.Left, Instruction.Move]
        );

        // Rover B: starts at (3,3,E), instructions: Move (x2), Turn Left
        plateau.AddRover(
            new Position(new Coordinates(3, 3), Direction.E),
            [Instruction.Move, Instruction.Move, Instruction.Left]
        );

        // Act
        plateau.ExecuteInstructions();

        // Assert
        // Check final positions after all instructions
        var roverA = plateau.Rovers[0];
        var roverB = plateau.Rovers[1];

        // Rover A path:
        // (1,2,N)->M->(1,3,N)->L->(1,3,W)->M->(0,3,W)
        roverA.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(0, 3));
        roverA.Position.Direction.Should().Be(Direction.W);

        // Rover B path:
        // (3,3,E)->M->(4,3,E)->M->(5,3,E)->Left->(5,3,N)
        roverB.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(5, 3));
        roverB.Position.Direction.Should().Be(Direction.N);
    }

    [Fact]
    public void ExecuteInstructions_ShouldDetectCollision()
    {
        // Setup a scenario where two rovers attempt to move into the same cell in the same tick
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);

        // Rover A: starts at (2,3,E), instructions: M
        // => tries to move to (3,3)
        plateau.AddRover(
            new Position(new Coordinates(2, 3), Direction.E),
            [Instruction.Move]
        );

        // Rover B: starts at (3,2,N), instructions: M
        // => tries to move "forward" from (3,2) => (3,3)
        plateau.AddRover(
            new Position(new Coordinates(3, 2), Direction.N),
            [Instruction.Move]
        );

        // Now both rovers want (3,3) in the same tick => collision
        plateau.ExecuteInstructions();

        // Rover A is booking (3,3)? Actually it's at (3,2) heading East => final is (3,3). 
        // Rover B is at (2,3) heading East => final is (3,3).
        // Let's check that they truly collide at (3,3)...
        // Assert
        log.Should().Contain(m => m.ToString().Contains("Collision detected"));
    }
}