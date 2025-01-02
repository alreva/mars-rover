using MarsRover.Input;

namespace MarsRover.Tests.TestArtifacts;

public class FakeConsole : IConsole
{
    private readonly Queue<string> _inputs;

    public FakeConsole(IEnumerable<string> inputs)
    {
        _inputs = new Queue<string>(inputs);
    }
    
    public List<string> Output { get; } = [];
        
    public string? ReadLine()
    {
        return _inputs.Count > 0 ? _inputs.Dequeue() : null;
    }

    public void WriteLine(string value)
    {
        Output.Add(value);
    }

    public void WriteLine(Message message)
    {
        Output.Add(message.ToString());
    }

    public void WriteLine(Error error)
    {
        Output.Add(error.ToString());
    }

    public void WriteLine()
    {
        Output.Add("");
    }
}