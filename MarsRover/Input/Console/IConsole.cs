namespace MarsRover.Input.Console;

public interface IConsole
{
    string? ReadLine();
    void WriteLine(string value);
    void WriteLine(Message message);
    void WriteLine(Error error);
    void WriteLine();
}