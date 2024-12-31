namespace MarsRover;

public class Rover
{
    public Rover(Plateau plateau,
        Position position,
        Instruction[] instructions,
        List<Message> log)
    {
        Plateau = plateau;
        Position = position;
        Instructions = instructions;
        CurrentInstructionIndex = 0;
        Log = log;
    }

    private int CurrentInstructionIndex { get; set; }

    public bool HasMovesRemaining => CurrentInstructionIndex < Instructions.Length;
    public Plateau Plateau { get; }
    public Position Position { get; set; }
    public Instruction[] Instructions { get; }
    
    public List<Message> Log { get; }

    public bool ExecuteCurrentInstruction()
    {
        LogInfo("Executing current instruction");
        if (!HasMovesRemaining)
        {
            LogInfo("No moves remaining, keeping current position and direction");
            return false;
        }

        var instruction = Instructions[CurrentInstructionIndex];
        switch (instruction)
        {
            case Instruction.Left:
                LogInfo("Turning left");
                TurnLeft();
                return true;
            case Instruction.Right:
                LogInfo("Turning right");
                TurnRight();
                return true;
            case Instruction.Move:
                LogInfo("Moving");
                return Move();
            default:
                throw new InvalidOperationException($"Wrong instruction: {instruction}");
        }
    }

    private void TurnLeft()
    {
        Position = Position with
        {
            Direction = Position.Direction switch
            {
                Direction.N => Direction.W,
                Direction.W => Direction.S,
                Direction.S => Direction.E,
                Direction.E => Direction.N,
                _ => throw new InvalidOperationException()
            }
        };

        CurrentInstructionIndex++;
    }

    private void TurnRight()
    {
        Position = Position with
        {
            Direction = Position.Direction switch
            {
                Direction.N => Direction.E,
                Direction.E => Direction.S,
                Direction.S => Direction.W,
                Direction.W => Direction.N,
                _ => throw new InvalidOperationException()
            }
        };

        CurrentInstructionIndex++;
    }

    public (Position, Error[], bool) GetNextPosition()
    {
        if (CurrentInstructionIndex >= Instructions.Length)
        {
            return (Position, [], false);
        }

        var instruction = Instructions[CurrentInstructionIndex];

        if (instruction != Instruction.Move)
        {
            return (Position, [], false);
        }
        
        var nextPositionCandidate = Position with
        {
            Coordinates = new Coordinates(
                X: Position.Direction switch
                {
                    Direction.E => Position.Coordinates.X + 1,
                    Direction.W => Position.Coordinates.X - 1,
                    _ => Position.Coordinates.X
                }, Y: Position.Direction switch
                {
                    Direction.N => Position.Coordinates.Y + 1,
                    Direction.S => Position.Coordinates.Y - 1,
                    _ => Position.Coordinates.Y
                })
        };

        List<Error> errors = [];
        
        if (Plateau.Width < nextPositionCandidate.Coordinates.X)
        {
            errors.Add("Rover cannot move outside the plateau's width.");
        }
        
        if (Plateau.Height < nextPositionCandidate.Coordinates.Y)
        {
            errors.Add("Rover cannot move outside the plateau's height.");
        }

        return errors.Count > 0
            ? (Position, errors.ToArray(), false)
            : (nextPositionCandidate, [], true);
    }
    
    private bool Move()
    {
        var (position, errors, _) = GetNextPosition();
        
        if (errors.Length > 0)
        {
            foreach (var error in errors)
            {
                LogError(error);
            }
            return false;
        }

        Position = position;
        CurrentInstructionIndex++;
        return true;
    }

    private void LogInfo(Message message)
    {
        Log.Add(message);
    }

    public void LogError(Error error)
    {
        Log.Add(error);
    }

    public void Deconstruct(out Plateau Plateau, out Position Position, out Instruction[] Instructions)
    {
        Plateau = this.Plateau;
        Position = this.Position;
        Instructions = this.Instructions;
    }
}