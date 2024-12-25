namespace LibHac.NSZ;

public class NczFormatException : Exception
{
    public NczFormatException(string? message = null, Exception? ex = null) : base(message, ex)
    {
    }
}