namespace HyperTAG.Exceptions;

public class TagSerializeException : RootException
{
    public TagSerializeException() : base()
    {
        
    }

    public TagSerializeException(string message) : base(message)
    {
        
    }

    public TagSerializeException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}