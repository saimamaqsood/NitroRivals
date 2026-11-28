namespace NitroRivals.Exceptions;

public class OtpExpiredException : Exception
{
    public OtpExpiredException() : base("The verification code has expired.") { }
    public OtpExpiredException(string message) : base(message) { }
}
