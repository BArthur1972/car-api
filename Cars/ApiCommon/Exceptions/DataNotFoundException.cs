namespace Cars.ApiCommon.Exceptions;

[Serializable]
public class DataNotFoundException : Exception
{
    public DataNotFoundException()
        : base(null, null)
    {
    }

    public DataNotFoundException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}