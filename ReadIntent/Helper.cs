using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace ReadIntent
{
  public static class Helper
  {
    public static string GetConfig(string key, string tag, object value)
    {
      var builder = new ConfigurationBuilder();
      builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));

      var config = builder.Build();
      return config["TestApp:Settings:Message"] ?? "Hello world!";

    }
  }
}