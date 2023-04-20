using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
  public class Startup : FunctionsStartup
  {
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
      builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
            {
              options.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
              .ConfigureRefresh(refresh =>
                {
                  refresh.Register("TestApp:Settings:Message")
                      .SetCacheExpiration(TimeSpan.FromSeconds(10));
                })
              .Select(KeyFilter.Any, LabelFilter.Null)
              // Override with any configuration values specific to current hosting env
              .Select(KeyFilter.Any, Environment.GetEnvironmentVariable("Environment"));
            });
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
      builder.Services.AddAzureAppConfiguration();
    }
  }
}