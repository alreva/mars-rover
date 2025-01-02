namespace MarsRover.Input.Console;

public class ConsoleAdapter : IConsole
{
    public string? ReadLine() => System.Console.ReadLine();
    public void WriteLine(string value) => System.Console.WriteLine(value);
    public void WriteLine() => System.Console.WriteLine();
}

