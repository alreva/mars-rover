namespace MarsRover;

public class Error(string message) : Message(message)
{
    // cast from string to Error
    public static implicit operator Error(string message)
    {
        return new Error(message);
    }
}