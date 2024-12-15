namespace PingBoard.Probes.Services;

using System.Text.RegularExpressions;

public record class Hostname
{
    public string Value { get; private set; }
    private const string _validHostnameFormat =
        @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";

    public Hostname(string hostname)
    {
        if (!Regex.IsMatch(hostname, _validHostnameFormat))
        {
            throw new ArgumentException($"hostname was not valid.");
        }

        Value = hostname;
    }

    private Hostname() { }

    public static bool TryParse(string maybeHostname, out Hostname? hostname)
    {
        hostname = null;
        if (!Regex.IsMatch(maybeHostname, _validHostnameFormat))
        {
            return false;
        }

        hostname = new Hostname();
        hostname.Value = maybeHostname;
        return true;
    }

    public override string ToString()
    {
        return Value;
    }
}
