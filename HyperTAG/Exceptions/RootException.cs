namespace HyperTAG.Exceptions;

public class RootException : Exception
{
    public RootException() : base()
    {
        
    }
    
    public RootException(string message) : base(message)
    {
        
    }
    
    public RootException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
    
}