using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Common.Services;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using ordercloud.integrations.exchangerates;
using Marketplace.Models.Extended;

namespace Marketplace.Common.Commands
{
    public interface ILineItemCommand
    {
        Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem li, VerifiedUserContext verifiedUser);
        Task RequestReturnEmail(string OrderID);
        Task PatchLineItemStatus(string orderID, LineItemStatus lineItemStatus);
        Task<List<MarketplaceLineItem>> UpdateLineItemStatusesWithNotification(OrderDirection orderDirection, string orderID, LineItemStatusChange lineItemStatusChange, VerifiedUserContext verifiedUser);
    }

    public class LineItemCommand : ILineItemCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly ISendgridService _sendgridService;
        private readonly IMeProductCommand _meProductCommand;


        public LineItemCommand(IExchangeRatesCommand exchangeRates, ILocationPermissionCommand locationPermissionCommand, ISendgridService sendgridService, IOrderCloudClient oc, IMeProductCommand meProductCommand)
        {
			_oc = oc;
            _sendgridService = sendgridService;
            _meProductCommand = meProductCommand;
        }

        public async Task<List<MarketplaceLineItem>> UpdateLineItemStatusesWithNotification(OrderDirection orderDirection, string orderID, LineItemStatusChange lineItemStatusChange, VerifiedUserContext verifiedUser = null)
        {
            var userType = verifiedUser?.UsrType ?? "noUser";
            var verifiedUserType = userType.Reserialize<VerifiedUserType>();

            var previousLineItemsStates = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, orderID);
            var newPartialLineItem = new PartialLineItem()
            {
                xp = new
                {
                    lineItemStatusChange.LineItemStatus
                }
            };
            ValidateLineItemStatusChange(previousLineItemsStates.Items.ToList(), lineItemStatusChange, verifiedUserType);
            
            var updatedLineItems = await Throttler.RunAsync(lineItemStatusChange.LineItemIDs, 100, 5, (lineItemID) =>
            {
               // if there is no verified user passed in it has been called from somewhere else in the code base and will be done with the client grant access
               return verifiedUser != null ? _oc.LineItems.PatchAsync<MarketplaceLineItem>(orderDirection, orderID, lineItemID, newPartialLineItem, verifiedUser.AccessToken) : _oc.LineItems.PatchAsync<MarketplaceLineItem>(orderDirection, orderID, lineItemID, newPartialLineItem);
            });

            await HandleLineItemStatusChangeNotification(verifiedUserType, orderID, lineItemStatusChange, previousLineItemsStates.Items.ToList());
            return updatedLineItems.ToList();
        }

        private async Task HandleLineItemStatusChangeNotification(VerifiedUserType setterUserType, string orderID, LineItemStatusChange lineItemStatusChange, List<MarketplaceLineItem> lineItems)
        {
            var buyerOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID.Split('-')[0]);
            var lineItemsChanged = lineItems.Where(li => lineItemStatusChange.LineItemIDs.Contains(li.ID));
            var supplierIDs = lineItems.Select(li => li.SupplierID).Distinct().ToList();
            var suppliers = await Throttler.RunAsync(supplierIDs, 100, 5, supplierID => _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID));

            foreach (KeyValuePair<VerifiedUserType, LineItemEmailDisplayText> entry in StatusChangeEmailText[lineItemStatusChange.LineItemStatus]) {
                var userType = entry.Key;
                var emailText = entry.Value;

                var firstName = "";
                var lastName = "";
                var email = "";

                if (userType == VerifiedUserType.buyer)
                {
                    firstName = buyerOrder.FromUser.FirstName;
                    lastName = buyerOrder.FromUser.LastName;
                    email = buyerOrder.FromUser.Email;
                    email = "bhickey@four51.com";
                    await _sendgridService.SendLineItemStatusChangeEmail(lineItemStatusChange, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                }
                else if (userType == VerifiedUserType.admin) {
                    firstName = "PlaceHolderFirstName";
                    lastName = "PlaceHolderLastName";
                    email = "bhickey@four51.com";
                    var shouldNotify = !(LineItemStatusChangesDontNotifySetter.Contains(lineItemStatusChange.LineItemStatus) && setterUserType == VerifiedUserType.admin);
                    if (shouldNotify)
                    {
                        await _sendgridService.SendLineItemStatusChangeEmail(lineItemStatusChange, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                    }
                } else
                {
                    var shouldNotify = !(LineItemStatusChangesDontNotifySetter.Contains(lineItemStatusChange.LineItemStatus) && setterUserType == VerifiedUserType.supplier);
                    if (shouldNotify)
                    {
                        await Throttler.RunAsync(suppliers, 100, 5, supplier =>
                        {
                            firstName = supplier.xp.SupportContact.Name;
                            lastName = "";
                            //email = supplier.xp.SupportContact.Email;
                            email = "bhickey@four51.com";
                            return _sendgridService.SendLineItemStatusChangeEmail(lineItemStatusChange, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                        });
                    }
                }
            }
        }

        // these statuses can be set by either the supplier or the seller, but when this user modifies the 
        // line item status we do not want to notify themselves
        private static List<LineItemStatus> LineItemStatusChangesDontNotifySetter = new List<LineItemStatus>()
        {
            LineItemStatus.Returned,
            LineItemStatus.Backordered,
            LineItemStatus.Canceled
        };

        // defining seller and supplier together as the current logic is the 
        // seller should be able to do about anything a supplier can do
        private static List<LineItemStatus> ValidSellerOrSupplierLineItemStatuses = new List<LineItemStatus>()
        {
            LineItemStatus.Returned, LineItemStatus.Backordered, LineItemStatus.Canceled
        };

        // definitions of which user contexts can can set which lineItemStatuses
        private static Dictionary<VerifiedUserType, List<LineItemStatus>> ValidLineItemStatusSetByUserType = new Dictionary<VerifiedUserType, List<LineItemStatus>>()
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
        private static Dictionary<LineItemStatus, List<LineItemStatus>> ValidPreviousStateLineItemChangeMap = new Dictionary<LineItemStatus, List<LineItemStatus>>()
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

       
        private static Dictionary<LineItemStatus, Dictionary<VerifiedUserType, LineItemEmailDisplayText>> StatusChangeEmailText = new Dictionary<LineItemStatus, Dictionary<VerifiedUserType, LineItemEmailDisplayText>>()
        {
            { LineItemStatus.Complete, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Items on your order have shipped",
                    StatusChangeDetail = "A supplier has shipped items from your order",
                    StatusChangeDetail2 = "The following items are on there way"
                } }
            } },
            { LineItemStatus.ReturnRequested, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Return request has been submitted on your order",
                    StatusChangeDetail = "You will be updated when this return is processed",
                    StatusChangeDetail2 = "The following items have been request for return"
                } },
                { VerifiedUserType.admin, new LineItemEmailDisplayText()
                {
                    EmailSubject = "A buyer has submitted a return on their order",
                    StatusChangeDetail = "Return request needs to be processed, communication with supplier handled and RMA created.",
                    StatusChangeDetail2 = "The following items have been request for return"
                } },
                { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                {
                    EmailSubject = "A buyer has submitted a return on their order",
                    StatusChangeDetail = "Return request needs to be processed, communication with supplier handled and RMA created.",
                    StatusChangeDetail2 = "The following items have been request for return"
                } }
            } },
              { LineItemStatus.Returned, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Return has been processed for your order",
                    StatusChangeDetail = "You will be refunded for the proper amount",
                    StatusChangeDetail2 = "The following items have had returns processed"
                } },
                { VerifiedUserType.admin, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Return has been process for a buyer order",
                    StatusChangeDetail = "Ensure that the full return process is complete",
                    StatusChangeDetail2 = "The following items have been marked as returned"
                } },
                { VerifiedUserType.supplier , new LineItemEmailDisplayText()
                {
                    EmailSubject = "Return has been process for a buyer order",
                    StatusChangeDetail = "Ensure that the full return process is complete",
                    StatusChangeDetail2 = "The following items have been marked as returned"
                } }
            } },
                { LineItemStatus.Backordered, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Item on your order has been backordered by supplier",
                    StatusChangeDetail = "You will be updated on the status of the order when more information is known",
                    StatusChangeDetail2 = "The following items have been marked as backordered"
                } },
                { VerifiedUserType.admin, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Supplier has marked items on an order as backordered",
                    StatusChangeDetail = "You will be updated on the status of the order when more information is known",
                    StatusChangeDetail2 = "The following items have been marked as backordered"
                } },
                { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Items on order have been marked as backordered",
                    StatusChangeDetail = "Keep the buyer updated on the status of these items when you know more information",
                    StatusChangeDetail2 = "The following items have been marked as backordered"
                } },
               } },
               { LineItemStatus.CancelRequested, new Dictionary<VerifiedUserType, LineItemEmailDisplayText>() {
                { VerifiedUserType.buyer, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Your request for cancelation has been submitted",
                    StatusChangeDetail = "You will be updated on the status of the order when more information is known",
                    StatusChangeDetail2 = "The following items have had cancellation requested"
                } },
                { VerifiedUserType.admin, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Buyer has requested cancelation of lineitems for an order",
                    StatusChangeDetail = "The supplier will look into the feasibility of this cancellation",
                    StatusChangeDetail2 = "The following items have had cancellation requested"
                } },
                { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Buyer has requested cancelation of lineitems for an order",
                    StatusChangeDetail = "Look into the feasibility of this cancellation",
                    StatusChangeDetail2 = "The following items have had cancellation requested"
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
                    EmailSubject = "Items on an order have been cancelled",
                    StatusChangeDetail = "The buyer should be refunded for the proper amount",
                    StatusChangeDetail2 = "The following items have been cancelled"
                } },
                { VerifiedUserType.supplier, new LineItemEmailDisplayText()
                {
                    EmailSubject = "Items on an order have been cancelled",
                    StatusChangeDetail = "The buyer should be refunded for the proper amount",
                    StatusChangeDetail2 = "The following items have been cancelled"
                } },

            } }
        };

        private void ValidateLineItemStatusChange(List<MarketplaceLineItem> previousLineItemStates, LineItemStatusChange lineItemStatusChange, VerifiedUserType userType)
        {
            var allowedLineItemStatuses = ValidLineItemStatusSetByUserType[userType];

            Require.That(allowedLineItemStatuses.Contains(lineItemStatusChange.LineItemStatus), new ErrorCode("Not authorized to set this status on a lineItem", 400, $"Not authorized to set line items to {lineItemStatusChange.LineItemStatus}"));

            var validPreviousStates = ValidPreviousStateLineItemChangeMap[lineItemStatusChange.LineItemStatus];
            var invalidPreviousStates = previousLineItemStates.Where(p => !validPreviousStates.Contains(p.xp.LineItemStatus)).Select(p => p.ID); ;

            Require.That(invalidPreviousStates.Count() == 0, new ErrorCode("Invalid lineItem status change", 400, $"The following lineItems cannot be set to {lineItemStatusChange.LineItemStatus} given their current status: {String.Join(" ,", invalidPreviousStates)}"));
        }

        public async Task RequestReturnEmail(string orderID)
        {
            await _sendgridService.SendReturnRequestedEmail(orderID);
        }
  
        public async Task PatchLineItemStatus(string orderID, LineItemStatus lineItemStatus)
        {
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
            var partialLi = new PartialLineItem { xp = new { LineItemStatus = lineItemStatus } };
            List<Task> lineItemsToPatch = new List<Task>();
            foreach (var li in lineItems.Items)
            {
                lineItemsToPatch.Add(_oc.LineItems.PatchAsync(OrderDirection.Incoming, orderID, li.ID, partialLi));
            }
            await Task.WhenAll(lineItemsToPatch);
        }

        public async Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem liReq, VerifiedUserContext user)
        {
            // get me product with markedup prices correct currency and the existing line items in parellel
            var productRequest = _meProductCommand.Get(liReq.ProductID, user);
            var existingLineItemsRequest = _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, orderID, null, user.AccessToken);

            var existingLineItems = await existingLineItemsRequest;
            var li = new MarketplaceLineItem();

            // If line item exists, update quantity, else create
            var preExistingLi = ((List<MarketplaceLineItem>)existingLineItems.Items).Find(eli => LineItemsMatch(eli, liReq));
            if (preExistingLi != null)
            {
                await _oc.LineItems.DeleteAsync(OrderDirection.Outgoing, orderID, preExistingLi.ID, user.AccessToken);
            }
            
            var product = await productRequest;
            var markedUpPrice = GetLineItemUnitCost(product, liReq);
            liReq.UnitPrice = markedUpPrice;
            liReq.xp.LineItemStatus = LineItemStatus.Open;
            li = await _oc.LineItems
                .CreateAsync<MarketplaceLineItem>
                (OrderDirection.Incoming, orderID, liReq);
            return li;
        }

        private decimal GetLineItemUnitCost(SuperMarketplaceMeProduct product, MarketplaceLineItem li)
        {
            var markedUpBasePrice = product.PriceSchedule.PriceBreaks.Last(priceBreak => priceBreak.Quantity <= li.Quantity).Price;
            var totalSpecMarkup = li.Specs.Aggregate(0M, (accumulator, spec) =>
            {
                var relatedProductSpec = product.Specs.First(productSpec => productSpec.ID == spec.SpecID);
                var relatedSpecMarkup = relatedProductSpec.Options.First(option => option.ID == spec.OptionID).PriceMarkup;
                return accumulator + (relatedSpecMarkup ?? 0M);
            });
            return totalSpecMarkup + markedUpBasePrice;
        }

        private bool LineItemsMatch(LineItem li1, LineItem li2)
        {
            if (li1.ProductID != li2.ProductID) return false;
            foreach (var spec1 in li1.Specs) {
                var spec2 = (li2.Specs as List<LineItemSpec>)?.Find(s => s.SpecID == spec1.SpecID);
                if (spec1?.Value != spec2?.Value) return false;
            }
            return true;
        }
    };
}