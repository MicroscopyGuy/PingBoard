namespace PingBoard.TestUtilities;

using Microsoft.AspNetCore.Builder;

public class AppFixture
{
    public WebApplication App;

    public AppFixture()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.AddServices();
        this.App = builder.Build();
    }
}
