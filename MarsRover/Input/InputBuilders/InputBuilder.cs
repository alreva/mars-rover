namespace MarsRover.Input.InputBuilders;

public class InputBuilder
{
    public static PlateauBuilder Plateau(List<Message> log) => new PlateauBuilder(log);
}