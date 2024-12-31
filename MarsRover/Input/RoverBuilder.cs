namespace MarsRover.Input;

public class RoverBuilder(PlateauBuilder plateauBuilder, List<Message> log)
{
    public PlateauBuilder PlateauBuilder { get; } = plateauBuilder;
    
    public BuilderOrErrors<RoverCommandSequenceBuilder?> WithRover(string input)
    {
        var errors = ParseAndValidateInput(input, out var x, out var y, out var direction);

        return errors.Count > 0 
            ? new BuilderOrErrors<RoverCommandSequenceBuilder?> { Errors = errors }
            : WithRover(x, y, direction);
    }

    private List<Error> ParseAndValidateInput(string input, out int x, out int y, out Direction direction)
    {
        x = 0; y = 0; direction = Direction.N;
        var parts = input.Split(' ');
        if (parts.Length != 3)
        {
            return ["Rover input must have three parts."];
        }
        
        List<Error> errors = [];
        if (!int.TryParse(parts[0], out x))
        {
            errors.Add("Rover X coordinate must be an integer.");
        }
        else
        {
            if (x < 0)
            {
                errors.Add("Rover X coordinate must be greater than or equal to 0.");
            }
            else if (x > PlateauBuilder.Width)
            {
                errors.Add("Rover X coordinate must be less than or equal to the plateau width.");
            }
        }
        
        if (!int.TryParse(parts[1], out y))
        {
            errors.Add("Rover Y coordinate must be an integer.");
        }
        else
        {
            if (y < 0)
            {
                errors.Add("Rover Y coordinate must be greater than or equal to 0.");
            }
            else if (y > PlateauBuilder.Height)
            {
                errors.Add("Rover Y coordinate must be less than or equal to the plateau height.");
            }
        }
        
        if (!Enum.TryParse(parts[2], true, out direction))
        {
            errors.Add("Rover direction must be a valid direction.");
        }
        
        return errors;
    }

    private RoverCommandSequenceBuilder WithRover(int x, int y, Direction direction)
    {
        X = x;
        Y = y;
        Direction = direction;
        return new RoverCommandSequenceBuilder(this, log);
    }

    public Direction Direction { get; set; }

    public int Y { get; set; }

    public int X { get; set; }
}