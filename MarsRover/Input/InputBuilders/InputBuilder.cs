namespace MarsRover.Input.InputBuilders;

public static class InputBuilder
{
    public static PlateauBuilder Plateau(List<Message> log) => new(log);
}