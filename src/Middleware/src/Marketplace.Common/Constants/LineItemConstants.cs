using System.Collections.Generic;
using Marketplace.Models.Extended;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models;
using System.Linq;

namespace Marketplace.Common.Constants
{
    public static class LineItemStatusConstants
    {
        public static (SubmittedOrderStatus, ShippingStatus, ClaimStatus) GetOrderStatuses(List<MarketplaceLineItem> lineItems)
        {
            var orderStatusOccurances = new HashSet<SubmittedOrderStatus>();
            var shippingStatusOccurances = new HashSet<ShippingStatus>();
            var claimStatusOccurances = new HashSet<ClaimStatus>();

            foreach(var lineItem in lineItems)
            {
                foreach(var status in lineItem.xp.StatusByQuantity)
                {
                    if(status.Value > 0)
                    {
                        orderStatusOccurances.Add(RelatedOrderStatus[status.Key]);
                        shippingStatusOccurances.Add(RelatedShippingStatus[status.Key]);
                        claimStatusOccurances.Add(RelatedClaimStatus[status.Key]);
                    }
                }
            }

            var orderStatus = GetOrderStatus(orderStatusOccurances);
            var shippingStatus = GetOrderShippingStatus(shippingStatusOccurances);
            var claimStatus = GetOrderClaimStatus(claimStatusOccurances);

            return (orderStatus, shippingStatus, claimStatus);
        }

        private static SubmittedOrderStatus GetOrderStatus(HashSet<SubmittedOrderStatus> orderStatusOccurances)
        {
            if(orderStatusOccurances.Count == 1)
            {
                return orderStatusOccurances.First();
            }

            if(orderStatusOccurances.Contains(SubmittedOrderStatus.Open))
            {
                return SubmittedOrderStatus.Open;
            }

            if(orderStatusOccurances.Contains(SubmittedOrderStatus.Completed))
            {
                return SubmittedOrderStatus.Completed;
            }

            // otherwise all lineitem statuses are canceled
            return SubmittedOrderStatus.Canceled;
        }

        private static ShippingStatus GetOrderShippingStatus(HashSet<ShippingStatus> shippingStatusOccurances)
        {
            if (shippingStatusOccurances.Count == 1)
            {
                return shippingStatusOccurances.First();
            }

            if (shippingStatusOccurances.Contains(ShippingStatus.Processing) && shippingStatusOccurances.Contains(ShippingStatus.Processing))
            {
                return ShippingStatus.PartiallyShipped;
            }

            if (shippingStatusOccurances.Contains(ShippingStatus.Shipped))
            {
                return ShippingStatus.Shipped;
            }

            if (shippingStatusOccurances.Contains(ShippingStatus.Processing))
            {
                return ShippingStatus.Processing;
            }

            // otherwise all lineitem statuses are canceled
            return ShippingStatus.Canceled;
        }

        private static ClaimStatus GetOrderClaimStatus(HashSet<ClaimStatus> claimStatusOccurances)
        {
            if (claimStatusOccurances.Count == 1)
            {
                return claimStatusOccurances.First();
            }

            if (claimStatusOccurances.Contains(ClaimStatus.Pending))
            {
                return ClaimStatus.Pending;
            }

            if (claimStatusOccurances.Contains(ClaimStatus.Complete))
            {
                return ClaimStatus.Complete;
            }

            // otherwise there are no claims
            return ClaimStatus.NoClaim;
        }

        private static Dictionary<LineItemStatus, SubmittedOrderStatus> RelatedOrderStatus = new Dictionary<LineItemStatus, SubmittedOrderStatus>()
        {
            { LineItemStatus.Submitted, SubmittedOrderStatus.Open },
            { LineItemStatus.Backordered, SubmittedOrderStatus.Open },
            { LineItemStatus.CancelRequested, SubmittedOrderStatus.Open },
            { LineItemStatus.Complete, SubmittedOrderStatus.Completed },
            { LineItemStatus.ReturnRequested, SubmittedOrderStatus.Completed },
            { LineItemStatus.Returned, SubmittedOrderStatus.Completed },
            { LineItemStatus.Canceled, SubmittedOrderStatus.Canceled },
        };
        private static Dictionary<LineItemStatus, ShippingStatus> RelatedShippingStatus = new Dictionary<LineItemStatus, ShippingStatus>()
        {
            { LineItemStatus.Submitted, ShippingStatus.Processing },
            { LineItemStatus.Backordered, ShippingStatus.Processing },
            { LineItemStatus.CancelRequested, ShippingStatus.Processing },
            { LineItemStatus.Complete, ShippingStatus.Shipped },
            { LineItemStatus.ReturnRequested, ShippingStatus.Shipped },
            { LineItemStatus.Returned, ShippingStatus.Shipped },
            { LineItemStatus.Canceled, ShippingStatus.Canceled },
        };
        private static Dictionary<LineItemStatus, ClaimStatus> RelatedClaimStatus = new Dictionary<LineItemStatus, ClaimStatus>()
        {
            { LineItemStatus.Submitted, ClaimStatus.NoClaim },
            { LineItemStatus.Backordered, ClaimStatus.Pending },
            { LineItemStatus.CancelRequested, ClaimStatus.Pending },
            { LineItemStatus.Complete, ClaimStatus.NoClaim },
            { LineItemStatus.ReturnRequested, ClaimStatus.Pending },
            { LineItemStatus.Returned, ClaimStatus.Complete },
            { LineItemStatus.Canceled, ClaimStatus.Complete },
        };


        // these statuses can be set by either the supplier or the seller, but when this user modifies the 
        // line item status we do not want to notify themselves
        public static List<LineItemStatus> LineItemStatusChangesDontNotifySetter = new List<LineItemStatus>()
        {
            LineItemStatus.Returned,
            LineItemStatus.Backordered,
            LineItemStatus.Canceled
        };

        // defining seller and supplier together as the current logic is the 
        // seller should be able to do about anything a supplier can do
        public static List<LineItemStatus> ValidSellerOrSupplierLineItemStatuses = new List<LineItemStatus>()
        {
            LineItemStatus.Returned, LineItemStatus.Backordered, LineItemStatus.Canceled
        };

        // definitions of which user contexts can can set which lineItemStatuses
        public static Dictionary<VerifiedUserType, List<LineItemStatus>> ValidLineItemStatusSetByUserType = new Dictionary<VerifiedUserType, List<LineItemStatus>>()
        {
            { VerifiedUserType.admin, ValidSellerOrSupplierLineItemStatuses },
            { VerifiedUserType.supplier, ValidSellerOrSupplierLineItemStatuses },
            { VerifiedUserType.buyer, new List<LineItemStatus>{ LineItemStatus.ReturnRequested, LineItemStatus.CancelRequested} },

            // requests that are not directly made to modify lineItem status, derivatives of order submit or shipping,
            // these should not be set without those trigger actions (order submit or shipping)
            { VerifiedUserType.noUser, new List<LineItemStatus>{ LineItemStatus.Submitted, LineItemStatus.Complete } }
        };

        // definitions to control which line item status changes are allowed
        // for example cannot change a completed line item to anything but returned or return requested
        public static Dictionary<LineItemStatus, List<LineItemStatus>> ValidPreviousStateLineItemChangeMap = new Dictionary<LineItemStatus, List<LineItemStatus>>()
        {
            // no previous states for submitted
            { LineItemStatus.Submitted, new List<LineItemStatus>() { } },

            { LineItemStatus.Complete, new List<LineItemStatus>() { LineItemStatus.Submitted, LineItemStatus.Backordered } },
            { LineItemStatus.ReturnRequested, new List<LineItemStatus>() { LineItemStatus.Complete } },
            { LineItemStatus.Returned, new List<LineItemStatus>() { LineItemStatus.ReturnRequested, LineItemStatus.Complete } },
            { LineItemStatus.Backordered, new List<LineItemStatus>() { LineItemStatus.Submitted } },
            { LineItemStatus.CancelRequested, new List<LineItemStatus>() { LineItemStatus.Submitted, LineItemStatus.Backordered } },
            { LineItemStatus.Canceled, new List<LineItemStatus>() { LineItemStatus.CancelRequested, LineItemStatus.Submitted, LineItemStatus.Backordered } },
        };

        public static Dictionary<LineItemStatus, Dictionary<VerifiedUserType, LineItemEmailDisplayText>> GetStatusChangeEmailText(string supplierName)
        {
            return new Dictionary<LineItemStatus, Dictionary<VerifiedUserType, LineItemEmailDisplayText>>()
            {
                { LineItemStatus.Complete, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                    { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Items on your order have shipped",
                        StatusChangeDetail = $"{supplierName} has shipped items from your order",
                        StatusChangeDetail2 = "The following items are on there way"
                    } }
                } },
                { LineItemStatus.ReturnRequested, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                    { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "A return request has been submitted on your order",
                        StatusChangeDetail = "You will be updated when this return is processed",
                        StatusChangeDetail2 = "The following items have been requested for return"
                    } },
                    { VerifiedUserType.admin, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "A buyer has submitted a return on their order",
                        StatusChangeDetail = "Contact the Supplier to process the return request.",
                        StatusChangeDetail2 = "The following items have been requested for return"
                    } },
                    { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "A buyer has submitted a return on their order",
                        StatusChangeDetail = "The seller will contact you to process the return request",
                        StatusChangeDetail2 = "The following items have been requested for return"
                    } }
                } },
                  { LineItemStatus.Returned, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                    { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "A return has been processed for your order",
                        StatusChangeDetail = "You will be refunded for the proper amount",
                        StatusChangeDetail2 = "The following items have had returns processed"
                    } },
                    { VerifiedUserType.admin, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "The supplier has processed a return",
                        StatusChangeDetail = "Ensure that the full return process is complete, and the customer was refunded",
                        StatusChangeDetail2 = "The following items have been marked as returned"
                    } },
                    { VerifiedUserType.supplier , new LineItemEmailDisplayText()
                    {
                        EmailSubject = "The seller has processed a return",
                        StatusChangeDetail = "Ensure that the full return process is complete",
                        StatusChangeDetail2 = "The following items have been marked as returned"
                    } }
                } },
                    { LineItemStatus.Backordered, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                    { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Item(s) on your order have been backordered by supplier",
                        StatusChangeDetail = "You will be updated on the status of the order when more information is known",
                        StatusChangeDetail2 = "The following items have been marked as backordered"
                    } },
                    { VerifiedUserType.admin, new LineItemEmailDisplayText()
                    {
                        EmailSubject = $"{supplierName} has marked items on an order as backordered",
                        StatusChangeDetail = "You will be updated on the status of the order when more information is known",
                        StatusChangeDetail2 = "The following items have been marked as backordered"
                    } },
                    { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Item(s) on order have been marked as backordered",
                        StatusChangeDetail = "Keep the buyer updated on the status of these items when you know more information",
                        StatusChangeDetail2 = "The following items have been marked as backordered"
                    } },
                   } },
                   { LineItemStatus.CancelRequested, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                    { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Your request for cancellation has been submitted",
                        StatusChangeDetail = "You will be updated on the status of the cancellation when more information is known",
                        StatusChangeDetail2 = "The following items have had cancellation requested"
                    } },
                    { VerifiedUserType.admin, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "A buyer has requested cancellation of line items on an order",
                        StatusChangeDetail = "The supplier will look into the feasibility of this cancellation",
                        StatusChangeDetail2 = "The following items have been requested for cancellation"
                    } },
                    { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "A buyer has requested cancelation of line items on an order",
                        StatusChangeDetail = "Review the items below to see if any can be cancelled before they ship",
                        StatusChangeDetail2 = "The following items have have been requested for cancellation"
                    } },

                } },
                 { LineItemStatus.Canceled, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                    { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Items on your order have been cancelled",
                        StatusChangeDetail = "You will be refunded for the cost of these items",
                        StatusChangeDetail2 = "The following items have been cancelled"
                    } },
                    { VerifiedUserType.admin, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Item(s) on an order have been cancelled",
                        StatusChangeDetail = "Ensure the buyer is refunded for the proper amount",
                        StatusChangeDetail2 = "The following items have been cancelled"
                    } },
                    { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                    {
                        EmailSubject = "Item(s) on an order have been cancelled",
                        StatusChangeDetail = "The seller will refund the buyer for the proper amount",
                        StatusChangeDetail2 = "The following items have been cancelled"
                    } },

                } }
            };
        }
    }
}
