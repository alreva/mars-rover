namespace MarsRover.Input;

public class InputBuilder
{
    public static PlateauBuilder Plateau(List<Message> log) => new PlateauBuilder(log);
}