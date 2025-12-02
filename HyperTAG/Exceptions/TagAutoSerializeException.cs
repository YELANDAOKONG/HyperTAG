namespace HyperTAG.Exceptions;

public class TagAutoSerializeException : RootException
{
    public TagAutoSerializeException() : base()
    {
    }

    public TagAutoSerializeException(string message) : base(message)
    {
    }

    public TagAutoSerializeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}