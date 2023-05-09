using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

[assembly: FunctionsStartup(typeof(ReadIntent.Startup))]

namespace ReadIntent
{
  public class Startup : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      builder.Services.AddDbContext<DataAccess.BloggingContext, DataAccess.SqliteBloggingContext>();
    }
  }
}