namespace MarsRover;

public class Message(string message)
{
    public static implicit operator Message(string message)
    {
        return new Message(message);
    }

    public override string ToString()
    {
        return message;
    }
}