namespace NitroRivals.Exceptions;

public class DuplicateAccountException : Exception
{
    public DuplicateAccountException() : base("That username or email is already in use.") { }
    public DuplicateAccountException(string message) : base(message) { }
}
