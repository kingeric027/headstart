using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IOrderSubmitCommand
    {
        Task<MarketplaceOrder> SubmitOrderAsync(string orderID, OrderDirection direction, OrderCloudIntegrationsCreditCardPayment payment, VerifiedUserContext user);
    }
    public class OrderSubmitCommand : IOrderSubmitCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly IOrderCloudIntegrationsCardConnectCommand _card;

        public OrderSubmitCommand(IOrderCloudClient oc, AppSettings settings, IOrderCloudIntegrationsCardConnectCommand card)
        {
            _oc = oc;
            _settings = settings;
            _card = card;
        }

        public async Task<MarketplaceOrder> SubmitOrderAsync(string orderID, OrderDirection direction, OrderCloudIntegrationsCreditCardPayment payment, VerifiedUserContext user)
        {
            var worksheet = await _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderID);
            await ValidateOrderAsync(worksheet, payment);

            var incrementedOrderID = await IncrementOrderAsync(worksheet);
            if (worksheet.LineItems.Any(li => li.Product.xp.ProductType != ProductType.PurchaseOrder))
            {
                payment.OrderID = incrementedOrderID;
                await _card.AuthorizePayment(payment, user, GetMerchantID(payment)); // authorize AND capture payments (SEB specific, in general use case we'd auth before submit and capture on shipment)
            }
            try
            {
                return await RetryUpToThreeTimes().ExecuteAsync(() => _oc.Orders.SubmitAsync<MarketplaceOrder>(direction, incrementedOrderID, user.AccessToken));
            } catch(Exception e)
            {
                // TODO: 
                // if an error happens here we're in a bad state because user was charged
                // but order didn't submit
                // log this somewhere or send email
                throw e;
            }
        }

        private async Task ValidateOrderAsync(MarketplaceOrderWorksheet worksheet, OrderCloudIntegrationsCreditCardPayment payment)
        {
            Require.That(
                !worksheet.Order.IsSubmitted, 
                new ErrorCode("OrderSubmit.AlreadySubmitted", 400, "Order has already been submitted")
            );

            var shipMethodsWithoutSelections = worksheet.ShipEstimateResponse.ShipEstimates.Where(estimate => estimate.SelectedShipMethodID == null);
            Require.That(
                shipMethodsWithoutSelections.Count() == 0, 
                new ErrorCode("OrderSubmit.MissingShippingSelections", 400, "All shipments on an order must have a selection"), shipMethodsWithoutSelections
                );

            var standardLines = worksheet.LineItems.Where(li => li.Product.xp.ProductType != ProductType.PurchaseOrder);
            Require.That(
                !standardLines.Any() || payment != null,
                new ErrorCode("OrderSubmit.MissingPayment", 400, "Order contains standard line items and must include credit card payment details"),
                standardLines
            );

            try
            {
                // ordercloud validates the same stuff that would be checked on order submit
                await _oc.Orders.ValidateAsync(OrderDirection.Incoming, worksheet.Order.ID);
            } catch(OrderCloudException ex) {
                // this error is expected and will be resolved before oc order submit call happens
                // in a non-seb flow this could be removed because we'd auth the payment which would mark it as accepted
                // before it even hits the submit endpoint
                var errors = ex.Errors.Where(ex => ex.ErrorCode != "Order.CannotSubmitWithUnaccceptedPayments");
                if(errors.Any())
                {
                    throw new OrderCloudIntegrationException(new ApiError
                    {
                        ErrorCode = "OrderSubmit.OrderCloudValidationError",
                        Message = "Failed ordercloud validation, see Data for details",
                        Data = errors
                    });
                }
            }
            
        }

        private async Task<string> IncrementOrderAsync(MarketplaceOrderWorksheet worksheet)
        {
            if (worksheet.Order.xp.IsResubmitting == true)
            {
                // orders marked with IsResubmitting true are orders that were on hold and then declined 
                // so buyer needs to resubmit but we don't want to increment order again
                return worksheet.Order.ID;
            }
            if(worksheet.Order.ID.StartsWith(_settings.OrderCloudSettings.IncrementorPrefix))
            {
                // order has already been incremented, no need to increment again
                return worksheet.Order.ID;
            }
            var order = await _oc.Orders.PatchAsync(OrderDirection.Incoming, worksheet.Order.ID, new PartialOrder
            {
                ID = _settings.OrderCloudSettings.IncrementorPrefix + "{orderIncrementor}"
            });
            return order.ID;
        }

        private string GetMerchantID(OrderCloudIntegrationsCreditCardPayment payment)
        {
            string merchantID;
            if (payment.Currency == "USD")
                merchantID = _settings.CardConnectSettings.UsdMerchantID;
            else if (payment.Currency == "CAD")
                merchantID = _settings.CardConnectSettings.CadMerchantID;
            else
                merchantID = _settings.CardConnectSettings.EurMerchantID;

            return merchantID;
        }

        private AsyncRetryPolicy RetryUpToThreeTimes()
        {
            // retries three times, waits two seconds in-between failures
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(2),
                });
        }
    }
}
