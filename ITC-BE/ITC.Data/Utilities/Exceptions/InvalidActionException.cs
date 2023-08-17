namespace ITC.Data.Utilities.Exceptions;

public class InvalidActionException : HandledException
{
    public InvalidActionException(string? message = null) : base(400, message ?? "Action could not be performed")
    {
    }
}