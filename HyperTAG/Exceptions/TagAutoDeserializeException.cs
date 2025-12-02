namespace HyperTAG.Exceptions;

public class TagAutoDeserializeException : RootException
{
    public TagAutoDeserializeException() : base()
    {
    }

    public TagAutoDeserializeException(string message) : base(message)
    {
    }

    public TagAutoDeserializeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}