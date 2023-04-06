using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace API.Test.Fox;

public class FoxApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
	public FoxApplicationFactory()
	{
	}

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
    }
}