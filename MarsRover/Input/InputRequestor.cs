namespace MarsRover.Input;

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

public class PlateauRequestExecutor(
    PlateauBuilder plateauBuilder,
    IConsole console)
    : InputRequestExecutor<RoverBuilder>(console)
{
    protected override string Prompt => $"Enter the plateau dimensions (default '{DefaultInputs.Plateau}'):";
    
    protected override string DefaultValue => DefaultInputs.Plateau;

    protected override BuilderOrErrors<RoverBuilder?> Parse(string input)
    {
        return plateauBuilder.WithDimensions(input);
    }
}

public class RoverRequestExecutor(
    RoverBuilder roverBuilder,
    IConsole console)
    : InputRequestExecutor<RoverCommandSequenceBuilder?>(console)
{
    protected override string Prompt => $"Enter the rover's starting position (default '{DefaultInputs.Rover}'):";
    
    protected override string DefaultValue => DefaultInputs.Rover;

    protected override BuilderOrErrors<RoverCommandSequenceBuilder?> Parse(string input)
    {
        return roverBuilder.WithRover(input);
    }
}

public class RoverCommandSequenceRequestExecutor(
    RoverCommandSequenceBuilder roverCommandSequenceBuilder,
    IConsole console)
    : InputRequestExecutor<Plateau?>(console)
{
    protected override string Prompt => $"Enter the rover's instructions (default '{DefaultInputs.RoverCommands}'):";
    
    protected override string DefaultValue => DefaultInputs.RoverCommands;

    protected override BuilderOrErrors<Plateau?> Parse(string input)
    {
        return roverCommandSequenceBuilder.WithInstructions(input);
    }
}