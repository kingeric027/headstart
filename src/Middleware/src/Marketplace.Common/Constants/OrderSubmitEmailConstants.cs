using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Constants
{
    class OrderSubmitEmailConstants
    {
        public static Dictionary<VerifiedUserType, EmailDisplayText> GetOrderSubmitDictionary(string orderID, string firstName, string lastName)
        {
            return new Dictionary<VerifiedUserType, EmailDisplayText>()
            {
                {VerifiedUserType.buyer, new EmailDisplayText()
                {
                    EmailSubject = "Your order has been submitted",
                    DynamicText = "Thank you for your order.",
                    DynamicText2 = "We are getting your order ready to be shiped. You will be notified when it has been sent. Your order contains the folowing items."
                } },
                {VerifiedUserType.admin, new EmailDisplayText()
                {
                    EmailSubject = $"An order has been submitted {orderID}",
                    DynamicText = $"{firstName} {lastName} has placed an order.",
                    DynamicText2 = "The order contains the following items:"
                } },
                {VerifiedUserType.supplier, new EmailDisplayText()
                {
                    EmailSubject = $"An order has been submitted {orderID}",
                    DynamicText = $"{firstName} {lastName} has placed an order.",
                    DynamicText2 = "The order contains the following items:"
                } },
            };
        }
        public static Dictionary<VerifiedUserType, EmailDisplayText> GetQuoteOrderSubmitDictionary(string orderID, string firstName, string lastName)
        {
            return new Dictionary<VerifiedUserType, EmailDisplayText>()
            {
                {VerifiedUserType.buyer, new EmailDisplayText()
                {
                    EmailSubject = "Your quote has been submitted",
                    DynamicText = "Your quote has been submitted.",
                    DynamicText2 = "The vendor for this product will contact your with more information on your quote request."
                } },
                {VerifiedUserType.supplier, new EmailDisplayText()
                {
                    EmailSubject = "A quote has been requested",
                    DynamicText = "A quote has been requested for one of your products",
                    DynamicText2 = "Please reach out to the customer direclty to give them more information about their quote."
                } },
            };
        }
        public static EmailDisplayText GetOrderRequiresApprovalText()
        {
            return new EmailDisplayText()
            {
                EmailSubject = "An order requires your approval",
                DynamicText = "An order requires your approval. Please review the order and approve or decline the order.",
                DynamicText2 = "The order for approval contains the following items"
            };
        }
        public static EmailDisplayText GetRequestedApprovalText()
        {
            return new EmailDisplayText()
            {
                EmailSubject = "Your order was sent for approval",
                DynamicText = "Your order was sent for approval. You will receive an email when the order is approved.",
                DynamicText2 = "Your order awaiting approval contains the following items"
            };
        }
        public static EmailDisplayText GetOrderApprovedText()
        {
            return new EmailDisplayText()
            {
                EmailSubject = "Your order was approved",
                DynamicText = "Your order was approved and submitted.",
                DynamicText2 = "Your order contains the following items"
            };
        }
        public static EmailDisplayText GetOrderDeclinedText()
        {
            return new EmailDisplayText()
            {
                EmailSubject = "Your order was declined",
                DynamicText = "Your order was declined. Please review the order and re-submit the order again.",
                DynamicText2 = "Your declined order contains the following items"
            };
        }
    }
}
