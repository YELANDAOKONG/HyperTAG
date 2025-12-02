namespace HyperTAG.Exceptions;

public class TagDeserializeException : RootException
{
    public TagDeserializeException() : base()
    {
        
    }

    public TagDeserializeException(string message) : base(message)
    {
        
    }

    public TagDeserializeException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}