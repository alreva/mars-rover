namespace MarsRover.Input;

public interface IConsole
{
    string? ReadLine();
    void WriteLine(string value);
    void WriteLine(Message message);
    void WriteLine(Error error);
    void WriteLine();
}

public class ConsoleAdapter : IConsole
{
    public string? ReadLine() => Console.ReadLine();
    public void WriteLine(string value) => Console.WriteLine(value);
    public void WriteLine(Message message) => Console.WriteLine(message);
    public void WriteLine(Error error) => Console.WriteLine(error);
    public void WriteLine() => Console.WriteLine();
}

