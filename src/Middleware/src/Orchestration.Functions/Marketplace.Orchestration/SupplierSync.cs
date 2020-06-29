using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common;
using Marketplace.Common.Commands.SupplierSync;
using Marketplace.Common.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Orchestration
{
    public class SupplierSync
    {
        private readonly IOrderCloudIntegrationsFunctionToken _token;
        private readonly ISupplierSyncCommand _supplier;
        private readonly AppSettings _settings;

        public SupplierSync(AppSettings settings, IOrderCloudIntegrationsFunctionToken token, ISupplierSyncCommand supplier)
        {
            _settings = settings;
            _token = token;
            _supplier = supplier;
        }

        [FunctionName("GetSupplierOrder")]
        public async Task<IActionResult> GetSupplierOrder([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "{supplierId}/{orderId}")]
            HttpRequest req, string supplierId, string orderId, ILogger log)
        {
            log.LogInformation($"Supplier Order GET Request: {supplierId} {orderId}");
            try
            {
                var user = await _token.Authorize(req, new[] { ApiRole.OrderAdmin, ApiRole.OrderReader });
                Require.That(user.SupplierID == supplierId, new ErrorCode("Authorization.InvalidToken", 401, "Authorization.InvalidToken: Access token is invalid or expired."));
                var order = await _supplier.GetOrderAsync(orderId, user);
                log.LogInformation($"Supplier Order GET Request Success: {supplierId} {orderId}");
                return new OkObjectResult(order);
            }
            catch (OrderCloudIntegrationException oex)
            {
                log.LogError($"Error retrieving order for supplier: {supplierId} {orderId}. { oex.ApiError }");
                return new BadRequestObjectResult(oex.ApiError);
            }
            catch (OrderCloudException ocex)
            {
                log.LogError($"Error retrieving order for supplier: {supplierId} {orderId}. { ocex.Errors }");
                return new BadRequestObjectResult(ocex.Errors);
            }
            catch (Exception ex)
            {
                log.LogError($"Error retrieving order for supplier: {supplierId} {orderId}. {ex.Message}");
                return new BadRequestObjectResult(new ApiError()
                {
                    ErrorCode = "500",
                    Message = ex.Message
                });
            }
        }
    }
}