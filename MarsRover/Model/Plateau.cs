namespace MarsRover.Model;

public class Plateau(int width, int height, List<Message> log)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    
    List<Message> Log { get; } = log;

    public Rover AddRover(Position position, Instruction[] instructions)
    {
        var rover = new Rover(this, position, instructions, Log);
        Rovers.Add(new Rover(this, position, instructions, Log));
        return rover;
    }

    public List<Rover> Rovers { get; set; } = [];

    public void ExecuteInstructions()
    {
        LogInfo("Starting execution");
        if (!Rovers.Any())
        {
            LogInfo("No rovers to execute. Simulation will stop.");
            return;
        }
        
        if (!Rovers.Any(r => r.HasMovesRemaining))
        {
            LogInfo("All rovers have no moves remaining. Simulation cannot run.");
            return;
        }

        var currentTick = 0;
        while (Rovers.Any(r => r.HasMovesRemaining))
        {
            LogInfo($"Starting tick {currentTick}");
            var roversWithStateChanges = ExecuteNextTick();
            if (roversWithStateChanges.Count != 0)
            {
                currentTick++;
                continue;
            }
            
            LogInfo("No rovers moved during this tick. Simulation will stop.");
            break;
        }
    }

    private IList<int> ExecuteNextTick()
    {
        // book positions
        Dictionary<Coordinates, int> bookings = [];
        HashSet<int> roversToChange = [];

        var newRoverPositions = Rovers
            .Select((r, i) => (i, r.GetNextPosition()))
            .ToArray();

        LogInfo("Booking positions for rovers turning left or right");
        bool anythingBooked = false;
        foreach (var (roverIndex, newPosition) in newRoverPositions)
        {
            var (position, errors, willMove) = newPosition;
            var (coordinates, _) = position;
            if (willMove)
            {
                continue;
            }
            
            LogInfo($"Booking present position {coordinates} for rover {roverIndex}");
            bookings[coordinates] = roverIndex;
            if (errors.Length == 0)
            {
                anythingBooked = true;
                roversToChange.Add(roverIndex);
            }
            else foreach (var error in errors)
            {
                Rovers[roverIndex].LogError(error);
            }
        }

        if (!anythingBooked)
        {
            LogInfo("No rovers turning left or right in this iteration");
        }
        
        LogInfo("Booking positions for rovers moving forward");
        bool anythingBooked2 = false;
        foreach (var (roverIndex, newPosition) in newRoverPositions)
        {
            var (position, _, willMove) = newPosition;
            var (coordinates, _) = position;
            
            if (!willMove)
            {
                continue;
            }
            
            var rover = Rovers[roverIndex];
            if (bookings.TryGetValue(coordinates, out var booking))
            {
                var message = $"Collision detected at {coordinates} booked by rover {booking}" +
                              $" when rover {roverIndex} attempted to move to {coordinates};" +
                              $" rover {roverIndex} will not move";
                LogInfo(message);
                rover.LogError(message);
                continue;
            }

            LogInfo($"Booking target intended position {coordinates} for rover {roverIndex}");
            bookings[coordinates] = roverIndex;
            anythingBooked2 = true;
            roversToChange.Add(roverIndex);
        }
        if (!anythingBooked2)
        {
            LogInfo("No rovers moving forward in this iteration");
        }
        
        LogInfo("Executing instructions for rovers");
        HashSet<int> roversWithStateChanges = [];
        foreach (var roverIndex in roversToChange)
        {
            LogInfo($"Executing instruction for rover {roverIndex}");
            var stateChanged = Rovers[roverIndex].ExecuteCurrentInstruction();
            if (stateChanged)
            {
                roversWithStateChanges.Add(roverIndex);
            }
        }

        return roversWithStateChanges.ToArray();
    }
    
    private void LogInfo(Message message)
    {
        Log.Add(message);
    }
}