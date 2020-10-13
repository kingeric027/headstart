#define TRACE
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataMigrations.Migrations;
using Marketplace.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace DataMigrations
{
	public class Program
	{
		private static ServiceProvider _provider;

		static async Task Main(string[] args)
		{
			var services = new ServiceCollection();
			var builder = new ConfigurationBuilder();
			var settings = new AppSettings();

			builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION"));
			var config = builder.Build();
			config.Bind(settings);

			var cosmosConfig = new CosmosConfig(settings.CosmosSettings.DatabaseName, settings.CosmosSettings.EndpointUri, settings.CosmosSettings.PrimaryKey);

			//var cosmosConfig = new CosmosConfig("marketplace-database-test",
			//	"https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

			_provider = services
				.AddSingleton(settings)
				.AddSingleton<ICosmosBulkEditor>(new CosmosBulkEditor(cosmosConfig))
				.BuildServiceProvider();

			Trace.AutoFlush = true;
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
			Trace.WriteLine("Entering Main");

			var editor = _provider.GetService<ICosmosBulkEditor>();

			var migration = new new_asset_types_8oct2020(editor);

			await migration.Run();
		}
	}
}
