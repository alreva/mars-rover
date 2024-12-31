namespace MarsRover;

public record Position(Coordinates Coordinates, Direction Direction);

public record Coordinates(int X, int Y);