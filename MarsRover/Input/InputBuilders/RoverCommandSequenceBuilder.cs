using MarsRover.Model;

namespace MarsRover.Input.InputBuilders;

public class RoverCommandSequenceBuilder(RoverBuilder roverBuilder, List<Message> log)
{
    public RoverBuilder RoverBuilder { get; } = roverBuilder;
    
    public BuilderOrErrors<Plateau?> WithInstructions(string input)
    {
        var errors = ParseAndValidateInput(input, out var instructions);

        return errors.Count > 0 
            ? new BuilderOrErrors<Plateau?> { Errors = errors }
            : WithInstructions(instructions);
    }

    private Plateau WithInstructions(params IEnumerable<Instruction> commands)
    {
        Commands = commands.ToArray();
        return new Plateau(
                RoverBuilder.PlateauBuilder.Width,
                RoverBuilder.PlateauBuilder.Height,
                log)
            .AddRover(
                new Position(new Coordinates(RoverBuilder.X, RoverBuilder.Y), RoverBuilder.Direction),
                Commands!
            );
    }

    private static List<Error> ParseAndValidateInput(string input, out List<Instruction> instructions)
    {
        List<Error> errors = [];
        instructions = [];
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (c)
            {
                case 'L':
                    instructions.Add(Instruction.Left);
                    break;
                case 'R':
                    instructions.Add(Instruction.Right);
                    break;
                case 'M':
                    instructions.Add(Instruction.Move);
                    break;
                default:
                    errors.Add($"Invalid instruction '{c}' at position {i}.");
                    break;
            }
        }

        return errors;
    }
    
    private Instruction[]? Commands { get; set; }
}