namespace MarsRover.Input.Console;

public class ConsoleAdapter : IConsole
{
    public string? ReadLine() => System.Console.ReadLine();
    public void WriteLine(string value) => System.Console.WriteLine(value);
    public void WriteLine(Message message) => System.Console.WriteLine(message);
    public void WriteLine(Error error) => System.Console.WriteLine(error);
    public void WriteLine() => System.Console.WriteLine();
}

