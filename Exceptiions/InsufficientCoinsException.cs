namespace NitroRivals.Exceptions;

// Used later by sp_BuyCar / sp_UpgradeCar / sp_EnterCup.
public class InsufficientCoinsException : Exception
{
    public InsufficientCoinsException() : base("You do not have enough coins.") { }
    public InsufficientCoinsException(string message) : base(message) { }
}
