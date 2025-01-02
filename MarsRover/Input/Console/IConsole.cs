namespace MarsRover.Input.Console;

public interface IConsole
{
    string? ReadLine();
    void WriteLine(string value);
    void WriteLine();
}