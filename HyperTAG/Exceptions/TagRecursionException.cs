namespace HyperTAG.Exceptions;

public class TagRecursionException : RootException
{
    public TagRecursionException() : base()
    {
        
    }

    public TagRecursionException(string message) : base(message)
    {
        
    }

    public TagRecursionException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}