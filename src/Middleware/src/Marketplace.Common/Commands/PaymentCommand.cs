using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderCloud.SDK;
using ordercloud.integrations.cardconnect;
using Marketplace.Common.Services;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;

namespace Marketplace.Common.Commands
{

    public interface IPaymentCommand
    {
        Task<IList<MarketplacePayment>> SavePayments(string orderID, List<MarketplacePayment> requestedPayments, string userToken);
    }

    public class PaymentCommand : IPaymentCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IOrderCalcService _orderCalc;
        private readonly ICreditCardCommand _ccCommand;
        public PaymentCommand(
            IOrderCloudClient oc,
            IOrderCalcService orderCalc,
            ICreditCardCommand ccCommand
        )
        {
            _oc = oc;
            _orderCalc = orderCalc;
            _ccCommand = ccCommand;
        }

        public async Task<IList<MarketplacePayment>> SavePayments(string orderID, List<MarketplacePayment> requestedPayments, string userToken)
        {
            var worksheet = await _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderID);
            var existingPayments = (await _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, orderID)).Items;
            await DeletePaymentsNoLongerNeededAsync(requestedPayments, existingPayments, worksheet.Order, userToken);

            foreach(var requestedPayment in requestedPayments)
            {
                var existingPayment = existingPayments.FirstOrDefault(p => p.Type == requestedPayment.Type);
                if(requestedPayment.Type == PaymentType.CreditCard) { await UpdateCreditCardPaymentAsync(requestedPayment, existingPayment, worksheet, userToken); }
                if(requestedPayment.Type == PaymentType.PurchaseOrder) { await UpdatePoPaymentAsync(requestedPayment, existingPayment, worksheet); }
            }

            return (await _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, orderID)).Items;
        }

        private async Task UpdateCreditCardPaymentAsync(MarketplacePayment requestedPayment, MarketplacePayment existingPayment, MarketplaceOrderWorksheet worksheet, string userToken)
        {
            var paymentAmount = _orderCalc.GetCreditCardTotal(worksheet);
            if (existingPayment == null)
            {
                requestedPayment.Amount = paymentAmount;
                await _oc.Payments.CreateAsync(OrderDirection.Outgoing, worksheet.Order.ID, requestedPayment, userToken); // need user token because admins cant see personal credit cards
            }
            else if (existingPayment.CreditCardID == requestedPayment.CreditCardID)
            {
                await _ccCommand.VoidPaymentAsync(existingPayment, worksheet.Order, userToken);
                await _oc.Payments.PatchAsync(OrderDirection.Incoming, worksheet.Order.ID, existingPayment.ID, new PartialPayment
                {
                    Amount = paymentAmount,
                    xp = requestedPayment.xp
                });
            }
            else
            {
                // we need to delete payment because you can't have payments totaling more than order total and you can't set payments to $0
                await DeleteCreditCardPaymentAsync(existingPayment, worksheet.Order, userToken);
                requestedPayment.Amount = paymentAmount;
                await _oc.Payments.CreateAsync(OrderDirection.Outgoing, worksheet.Order.ID, requestedPayment, userToken); // need user token because admins cant see personal credit cards
            }
        }

        private async Task UpdatePoPaymentAsync(MarketplacePayment requestedPayment, MarketplacePayment existingPayment, MarketplaceOrderWorksheet worksheet)
        {
            var paymentAmount = _orderCalc.GetPurchaseOrderTotal(worksheet);
            if (existingPayment == null)
            {
                requestedPayment.Amount = paymentAmount;
                await _oc.Payments.CreateAsync(OrderDirection.Incoming, worksheet.Order.ID, requestedPayment);
            }
            else
            {
                await _oc.Payments.PatchAsync(OrderDirection.Incoming, worksheet.Order.ID, existingPayment.ID, new PartialPayment
                {
                    Amount = paymentAmount
                });
            }
        }

        private async Task DeleteCreditCardPaymentAsync(MarketplacePayment payment, MarketplaceOrder order, string userToken)
        {
            await _ccCommand.VoidPaymentAsync(payment, order, userToken);
            await _oc.Payments.DeleteAsync(OrderDirection.Incoming, order.ID, payment.ID);
        }


        private async Task DeletePaymentsNoLongerNeededAsync(IList<MarketplacePayment> requestedPayments, IList<MarketplacePayment> existingPayments, MarketplaceOrder order, string userToken)
        {
            // requestedPayments represents the payments that should be on the order
            // if there are any existing payments not reflected in requestedPayments then they should be deleted
            foreach (var existingPayment in existingPayments)
            {
                if (!requestedPayments.Any(p => p.Type == existingPayment.Type))
                {

                    if (existingPayment.Type == PaymentType.PurchaseOrder)
                    {
                        await _oc.Payments.DeleteAsync(OrderDirection.Incoming, order.ID, existingPayment.ID);
                    }
                    if (existingPayment.Type == PaymentType.CreditCard)
                    {
                        await DeleteCreditCardPaymentAsync(existingPayment, order, userToken);
                    }
                }
            }
        }
    };
}