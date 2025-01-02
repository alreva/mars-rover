using MarsRover.Input.Console;
using MarsRover.Input.InputBuilders;
using MarsRover.Input.RequestExecutors;

namespace MarsRover.Input;

public class RoverConsoleUi
{
    private readonly IConsole _console;
    private readonly List<Message> _log = [];
    private readonly PlateauBuilder _plateauBuilder;
    private RoverBuilder? _roverBuilder;
    private RoverCommandSequenceBuilder? _roverCommandSequenceBuilder;
    private Plateau? _plateau;
    
    public RoverConsoleUi(IConsole console)
    {
        _console = console;
        _plateauBuilder = InputBuilder.Plateau(_log);
    }
    
    public RunOneCycleResult RunOneCycle()
    {
        Func<RunOneCycleResult>[] inputs =
        [
            RequestPlateau,
            RequestRover,
            RequestInstructions
        ];
        
        foreach (var input in inputs)
        {
            var inputResult = input();
            switch (inputResult)
            {
                case RunOneCycleResult.Exit:
                case RunOneCycleResult.InvalidInput:
                    return inputResult;
            }
        }

        ExecuteSimulation();
        
        _console.WriteLine("Simulation complete. Execution log:");
        foreach (var message in _log)
        {
            _console.WriteLine(message);
        }

        _console.WriteLine();
        for (var index = 0; index < _plateau!.Rovers.Count; index++)
        {
            var plateauRover = _plateau.Rovers[index];
            _console.WriteLine($"Rover {index + 1} final position: {plateauRover.Position}");
        }

        return AskUserToContinue();
    }
    
    private RunOneCycleResult RequestPlateau()
    {
        if (_roverBuilder is not null)
        {
            return RunOneCycleResult.Continue;
        }
        var (result, builder) = new PlateauRequestExecutor(_plateauBuilder, _console).Run();
        _roverBuilder = builder;
        return result;
    }
    
    private RunOneCycleResult RequestRover()
    {
        if (_roverCommandSequenceBuilder is not null)
        {
            return RunOneCycleResult.Continue;
        }
        var (result, builder) = new RoverRequestExecutor(_roverBuilder!, _console).Run();
        _roverCommandSequenceBuilder = builder;
        return result;
    }
    
    private RunOneCycleResult RequestInstructions()
    {
        if (_plateau is not null)
        {
            return RunOneCycleResult.Continue;
        }
        var (result, plateau) = new RoverCommandSequenceRequestExecutor(_roverCommandSequenceBuilder!, _console).Run();
        _plateau = plateau;
        return result;
    }
    
    private void ExecuteSimulation()
    {
        _plateau!.ExecuteInstructions();
    }
    
    private RunOneCycleResult AskUserToContinue()
    {
        _console.WriteLine();
        _console.WriteLine("Do you want to continue? (y/n, default 'n')");
        var input = _console.ReadLine();
        var continueSimulation = string.Equals(input, "y", StringComparison.OrdinalIgnoreCase);
        if (continueSimulation)
        {
            ResetState();
        }
        return continueSimulation ? RunOneCycleResult.Continue : RunOneCycleResult.Exit;
    }
    
    private void ResetState()
    {
        _log.Clear();
        _roverBuilder = null;
        _roverCommandSequenceBuilder = null;
        _plateau = null;
    }
}