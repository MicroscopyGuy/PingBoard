using System.Text;
using System.Text.Json;

public class PaginationToken<T>
{
    public string Api { get; set; }
    public T Token { get; set; } // whatever the API's page leaves off on, implementation dependent

    /// <summary>
    /// Returns a combined, stringified representation of the token and API name
    /// </summary>
    /// <param name="token">Whatever the API's page leaves off on, it is implementation dependent</param>
    /// <param name="apiName">The name of the API using the token</param>
    /// <returns></returns>
    public static string ToApiFormat(T token, string apiName)
    {
        var pToken = new PaginationToken<T>() { Api = apiName, Token = token };
        var json = JsonSerializer.Serialize(pToken);
        var pTokenBytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(pTokenBytes);
    }

    /// <summary>
    /// Returns a PaginationToken from the stringified PaginationToken, and compares the actual and expected
    /// calling API names for validation. 
    /// </summary>
    /// <param name="apiToken">
    ///     The combined, stringified representation of the token and API name that ToApiFormat returns.
    /// </param>
    /// <param name="expectedApiName">The API expected to have provided the PaginationToken</param>
    /// <returns>A PaginationToken</returns>
    /// <exception cref="ArgumentException">Thrown in the event of a mismatch of expected and actual API callers.</exception>
    public static PaginationToken<T> FromApiFormat(string apiToken, string expectedApiName)
    {
        var pTokenBytes = Convert.FromBase64String(apiToken);
        var jsonStr = Encoding.UTF8.GetString(pTokenBytes);
        var paginationToken = JsonSerializer.Deserialize<PaginationToken<T>>(jsonStr);

        if (paginationToken.Api != expectedApiName)
        {
            string msg = "The provided token doesn't match the expected API. Check you're calling the correct API.";
            throw new ArgumentException(msg);
        }

        return paginationToken;
    }
}