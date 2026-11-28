namespace NitroRivals.Exceptions;

public class InvalidOtpException : Exception
{
    public InvalidOtpException() : base("The verification code is invalid.") { }
    public InvalidOtpException(string message) : base(message) { }
}
