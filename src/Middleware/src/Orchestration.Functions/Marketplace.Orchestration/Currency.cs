using System;
using System.IO;
using System.Threading.Tasks;
using Headstart.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using Flurl.Http.Configuration;
using LazyCache;

namespace Headstart.Orchestration
{
    public class Currency
    {
        private readonly IExchangeRatesCommand _command;
        public Currency(AppSettings settings, IFlurlClientFactory flurlFactory, IAppCache cache)
        {
            _command = new ExchangeRatesCommand(new BlobServiceConfig()
            {
                ConnectionString = settings.ExchangeRatesSettings.ConnectionString,
                Container = settings.ExchangeRatesSettings.Container
            }, flurlFactory, cache);
        }

        [FunctionName("Currency")]
        public async Task<IActionResult> UpdateCurrencyHttp(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "currency")] HttpRequest req, ILogger log)
        {
            log.LogInformation("HTTP Currency Updated Trigger");
            await _command.Update();

            return new OkObjectResult("OK");
        }
    }
}
