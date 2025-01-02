using MarsRover.Input.Console;

namespace MarsRover.Input.RequestExecutors;

public abstract class InputRequestExecutor<TBuilder>
{
    protected InputRequestExecutor(IConsole console)
    {
        this.Console = console;
    }

    protected abstract string Prompt { get; }
    
    protected abstract string DefaultValue { get; }

    protected abstract BuilderOrErrors<TBuilder?> Parse(string input);
    
    public (RunOneCycleResult, TBuilder?) Run()
    {
        Console.WriteLine(Prompt);
        var input = Console.ReadLine();
        
        if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
        {
            return (RunOneCycleResult.Exit, default);
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            input = DefaultValue;
        }
        
        var result = Parse(input);

        if (result.HasErrors)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error);
            }

            return (RunOneCycleResult.InvalidInput, default);
        }

        return (RunOneCycleResult.Continue, result.Builder);
    }

    public IConsole Console { get; }
}