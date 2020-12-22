using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{

    public interface IPaymentCommand
    {
        Task<IList<Payment>> UpdatePayments(string orderID, List<Payment> requestedPayments, string userToken);
    }

    public class PaymentCommand : IPaymentCommand
    {
        private readonly IOrderCloudClient _oc;

        public PaymentCommand(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        // updates or creates payments as needed
        public async Task<IList<Payment>> UpdatePayments(string orderID, List<Payment> requestedPayments, string userToken)
        {
            var existingPayments = (await _oc.Payments.ListAsync(OrderDirection.Incoming, orderID)).Items;

            foreach(var requestedPayment in requestedPayments)
            {
                if (requestedPayment.Type == PaymentType.CreditCard)
                {
                    var existingCCpayment = existingPayments.FirstOrDefault(p => p.Type == PaymentType.CreditCard);
                    if (existingCCpayment == null)
                    {
                        await _oc.Payments.CreateAsync(OrderDirection.Outgoing, orderID, requestedPayment, userToken); // need user token because admins cant see personal credit cards
                    }
                    else if (existingCCpayment.CreditCardID == requestedPayment.CreditCardID)
                    {
                        await _oc.Payments.PatchAsync(OrderDirection.Incoming, orderID, existingCCpayment.ID, new PartialPayment
                        {
                            Amount = requestedPayment.Amount,
                            xp = requestedPayment.xp
                        });
                    }
                    else
                    {
                        // we need to delete payment because you can't have payments totaling more than order total and you can't set payments to $0
                        await _oc.Payments.DeleteAsync(OrderDirection.Incoming, orderID, existingCCpayment.ID);
                        await _oc.Payments.CreateAsync(OrderDirection.Outgoing, orderID, requestedPayment, userToken); // need user token because admins cant see personal credit cards
                    }
                }

                if (requestedPayment.Type == PaymentType.PurchaseOrder)
                {
                    var existingPoPayment = existingPayments.FirstOrDefault(p => p.Type == PaymentType.PurchaseOrder);
                    if (existingPoPayment == null)
                    {
                        await _oc.Payments.CreateAsync(OrderDirection.Incoming, orderID, requestedPayment);
                        
                    }
                    else
                    {
                        await _oc.Payments.PatchAsync(OrderDirection.Incoming, orderID, existingPoPayment.ID, new PartialPayment
                        {
                            Amount = requestedPayment.Amount
                        });
                    }
                }
            }

            return (await _oc.Payments.ListAsync(OrderDirection.Incoming, orderID)).Items;
        }
    };
}