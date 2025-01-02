using MarsRover.Input.Console;
using MarsRover.Input.InputBuilders;

namespace MarsRover.Input.RequestExecutors;

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