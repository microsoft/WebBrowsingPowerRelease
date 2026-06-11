namespace EdgeBrowsingPower.Services;

public sealed class EdgeBrowsingPowerException : Exception
{
    public EdgeBrowsingPowerException(string message)
        : base(message)
    {
    }

    public EdgeBrowsingPowerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}