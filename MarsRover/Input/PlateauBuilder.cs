namespace MarsRover.Input;

public class PlateauBuilder(List<Message> log)
{
    public BuilderOrErrors<RoverBuilder?> WithDimensions(string input)
    {
        var errors = ParseAndValidateInput(input, out var width, out var height);

        return errors.Count > 0 
            ? new BuilderOrErrors<RoverBuilder?> { Errors = errors }
            : WithDimensions(width, height);
    }

    private RoverBuilder WithDimensions(int width, int height)
    {
        Width = width;
        Height = height;
        return new RoverBuilder(this, log);
    }

    private static List<Error> ParseAndValidateInput(string input, out int width, out int height)
    {
        width = 0; height = 0;
        var parts = input.Split(' ');
        if (parts.Length != 2)
        {
            return ["Plateau input must have two parts."];
        }
        
        List<Error> errors = [];
        if (!int.TryParse(parts[0], out width))
        {
            errors.Add("Plateau width must be an integer.");
        }
        else
        {
            switch (width)
            {
                case < 1:
                    errors.Add("Plateau width must be greater than 0.");
                    break;
                case > 100:
                    errors.Add("Plateau width must be less than or equal to 100.");
                    break;
            }
        }
        
        if (!int.TryParse(parts[1], out height))
        {
            errors.Add("Plateau height must be an integer.");
        }
        else
        {
            switch (height)
            {
                case < 1:
                    errors.Add("Plateau height must be greater than 0.");
                    break;
                case > 100:
                    errors.Add("Plateau height must be less than or equal to 100.");
                    break;
            }
        }
        
        return errors;
    }

    public int Width { get; private set; }
    public int Height { get; private set; }
}