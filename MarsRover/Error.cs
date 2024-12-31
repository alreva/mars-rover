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

public class Error(string message) : Message(message)
{
    // cast from string to Error
    public static implicit operator Error(string message)
    {
        return new Error(message);
    }
}