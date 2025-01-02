using FluentAssertions;
using MarsRover.Model;

namespace MarsRover.Tests.Model;

public class RoverTests
{
    [Fact]
    public void TurnLeft_ShouldRotateRoverCounterClockwise()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);
        var rover = new Rover(plateau,
            new Position(new Coordinates(2, 2), Direction.N),
            [Instruction.Left],
            log);

        // Act
        var result = rover.ExecuteCurrentInstruction();

        // Assert
        result.Should().BeTrue("the rover should successfully turn left");
        rover.Position.Direction.Should().Be(Direction.W, "turning left from North => West");
        rover.HasMovesRemaining.Should().BeFalse("we used up the single instruction");
    }

    [Fact]
    public void TurnRight_ShouldRotateRoverClockwise()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);
        var rover = new Rover(plateau,
            new Position(new Coordinates(2, 2), Direction.N),
            [Instruction.Right],
            log);

        // Act
        var result = rover.ExecuteCurrentInstruction();

        // Assert
        result.Should().BeTrue();
        rover.Position.Direction.Should().Be(Direction.E, "turning right from North => East");
        rover.HasMovesRemaining.Should().BeFalse();
    }

    [Fact]
    public void MoveForward_ShouldIncreaseY_WhenFacingNorth()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);
        var rover = new Rover(plateau,
            new Position(new Coordinates(2, 2), Direction.N),
            [Instruction.Move],
            log);

        // Act
        var result = rover.ExecuteCurrentInstruction();

        // Assert
        result.Should().BeTrue();
        rover.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(2, 3));
        rover.HasMovesRemaining.Should().BeFalse();
    }

    [Theory]
    [InlineData(Direction.N, 2, 5, 2, 5)]
    [InlineData(Direction.S, 2, 0, 2, 0)]
    [InlineData(Direction.W, 0, 2, 0, 2)]
    [InlineData(Direction.E, 5, 2, 5, 2)]
    public void MoveForward_ShouldNotMove_WhenRoverIsAtEdge(Direction facing,
        int startX, int startY,
        int expectedX, int expectedY)
    {
        // Arrange
        var log = new List<Message>();
        var rover = new Plateau(5, 5, log)
            .AddRover(
                new Position(new Coordinates(startX, startY), facing),
                [Instruction.Move]);

        // Act
        var result = rover.ExecuteCurrentInstruction();

        // Assert
        result.Should().BeFalse("the rover is blocked by the plateau boundary");
        rover.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(expectedX, expectedY));
        rover.HasMovesRemaining.Should().BeFalse();
            
        log.Should().ContainSingle(m => 
            m.ToString().Contains("cannot move outside the plateau"));
    }

    [Fact]
    public void ExecuteCurrentInstruction_ShouldHandleMultipleCommands_OneAtATime()
    {
        // Arrange
        // Sequence: Move, Left, Move
        Instruction[] instructions =
        [
            Instruction.Move,
            Instruction.Left,
            Instruction.Move
        ];
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);
        var rover = new Rover(plateau,
            new Position(new Coordinates(1, 2), Direction.N),
            instructions,
            log);

        // Act + Assert
        // 1) Move: from (1,2,N) -> (1,3,N)
        rover.ExecuteCurrentInstruction().Should().BeTrue();
        rover.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(1, 3));
        rover.Position.Direction.Should().Be(Direction.N);

        // 2) Left: now facing West
        rover.ExecuteCurrentInstruction().Should().BeTrue();
        rover.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(1, 3));
        rover.Position.Direction.Should().Be(Direction.W);

        // 3) Move: from (1,3,W) -> (0,3,W)
        rover.ExecuteCurrentInstruction().Should().BeTrue();
        rover.Position.Coordinates.Should().BeEquivalentTo(new Coordinates(0, 3));
        rover.Position.Direction.Should().Be(Direction.W);

        rover.HasMovesRemaining.Should().BeFalse();
    }

    [Fact]
    public void HasMovesRemaining_ShouldReturnFalse_WhenNoInstructions()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);
        var rover = new Rover(plateau,
            new Position(new Coordinates(1, 1), Direction.N),
            [],
            log);

        // Act + Assert
        rover.HasMovesRemaining.Should().BeFalse();
        // No instructions => executing won't do anything
        rover.ExecuteCurrentInstruction().Should().BeFalse();
    }

    [Fact]
    public void Logging_ShouldAddInfo_WhenInstructionIsExecuted()
    {
        // Arrange
        var log = new List<Message>();
        var plateau = new Plateau(5, 5, log);
        var rover = new Rover(plateau,
            new Position(new Coordinates(1, 1), Direction.N),
            [Instruction.Move],
            log);

        // Act
        var moved = rover.ExecuteCurrentInstruction();

        // Assert
        moved.Should().BeTrue();
            
        var stringLog = log.Select(m => m.ToString()).ToArray();
            
        stringLog.Should()
            .NotBeEmpty()
            .And.ContainSingle(m => m.ToString().Contains("Executing current instruction"));
        stringLog.Should().ContainSingle(m => m.ToString().Contains("Moving"));
    }
}