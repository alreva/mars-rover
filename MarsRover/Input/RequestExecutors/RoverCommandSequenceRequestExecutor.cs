using MarsRover.Input.Console;
using MarsRover.Input.InputBuilders;
using MarsRover.Model;

namespace MarsRover.Input.RequestExecutors;

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