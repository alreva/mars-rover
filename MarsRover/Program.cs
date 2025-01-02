using MarsRover.Input;

var console = new ConsoleAdapter();
RoverConsoleUi roverConsoleUi = new(console);

console.WriteLine("Mars Rover");
while (true)
{
    if (roverConsoleUi.RunOneCycle() == RunOneCycleResult.Exit)
    {
        console.WriteLine("Goodbye!");
        break;
    }
}