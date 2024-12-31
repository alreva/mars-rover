using MarsRover;
using MarsRover.Input;

List<Message> log = [];
PlateauBuilder plateauBuilder = InputBuilder.Plateau(log);
RoverBuilder? roverBuilder = null;
RoverCommandSequenceBuilder? roverCommandSequenceBuilder = null;
Plateau? plateau = null;

while (true)
{
    if (roverBuilder == null)
    {
        Console.WriteLine($"Enter the plateau's width and height (default is '{DefaultInputs.Plateau}'):");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            input = DefaultInputs.Plateau;
        }
        if (input == "exit")
        {
            break;
        }
        var roverBuilderCandidate = plateauBuilder.WithDimensions(input);
        if (roverBuilderCandidate.HasErrors)
        {
            foreach (var error in roverBuilderCandidate.Errors)
            {
                Console.WriteLine(error);
            }
            continue;
        }
        roverBuilder = roverBuilderCandidate.Builder;
        continue;
    }

    if (roverCommandSequenceBuilder == null)
    {
        Console.WriteLine($"Enter the rover's initial position (default is '{DefaultInputs.Rover}'):");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            input = DefaultInputs.Rover;
        }
        if (input == "exit")
        {
            break;
        }
        var roverCommandSequenceBuilderCandidate = roverBuilder.WithRover(input);
        if (roverCommandSequenceBuilderCandidate.HasErrors)
        {
            foreach (var error in roverCommandSequenceBuilderCandidate.Errors)
            {
                Console.WriteLine(error);
            }
            continue;
        }
        roverCommandSequenceBuilder = roverCommandSequenceBuilderCandidate.Builder;
        continue;
    }

    if (plateau == null)
    {
        Console.WriteLine($"Enter the rover's command sequence (default is '{DefaultInputs.RoverCommands}'):");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            input = DefaultInputs.RoverCommands;
        }
        if (input == "exit")
        {
            break;
        }
        var roverCandidate = roverCommandSequenceBuilder.WithInstructions(input);
        if (roverCandidate.HasErrors)
        {
            foreach (var error in roverCandidate.Errors)
            {
                Console.WriteLine(error);
            }
            continue;
        }
        plateau = roverCandidate.Builder;
        continue;
    }
    
    plateau.ExecuteInstructions();

    Console.WriteLine("Execution Log:");
    foreach (var message in log)
    {
        if (message is Error error)
        {
            Console.WriteLine($"[E] {error}");
        }
        else
        {
            Console.WriteLine(message);
        }
    }
    Console.WriteLine();

    for (var roverIndex = 0; roverIndex < plateau.Rovers.Count; roverIndex++)
    {
        var rover = plateau.Rovers[roverIndex];
        Console.WriteLine($"Rover {roverIndex} final position:");
        Console.WriteLine(rover.Position);
        Console.WriteLine();
    }
    
    Console.WriteLine("Do you want to continue? (y/n)");
    if (Console.ReadLine() != "y")
    {
        break;
    }
    
    log.Clear();
    roverBuilder = null;
    roverCommandSequenceBuilder = null;
    plateau = null;
}