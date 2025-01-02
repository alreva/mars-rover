using MarsRover.Input.Console;
using MarsRover.Input.InputBuilders;

namespace MarsRover.Input.RequestExecutors;

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