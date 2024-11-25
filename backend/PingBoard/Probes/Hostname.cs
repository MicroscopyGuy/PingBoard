namespace PingBoard.Probes.Services;
using System.Text.RegularExpressions;

public record class Hostname
{
    public string Value { get;  }
    
    public Hostname(string hostname)
    {
        string validHostnameFormat =
            @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        if (!Regex.IsMatch(hostname, validHostnameFormat))
        {
            throw new ArgumentException($"hostname was not valid.");
        }

        Value = hostname;
    }
}