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
using Marketplace.Common.Constants;

namespace Marketplace.Common.Commands
{
    public interface ILineItemCommand
    {
        Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem li, VerifiedUserContext verifiedUser);
        Task<List<MarketplaceLineItem>> UpdateLineItemStatusesWithNotification(OrderDirection orderDirection, string orderID, LineItemStatusChanges lineItemStatusChanges, VerifiedUserContext verifiedUser);
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

        /// <summary>
        /// Validates LineItemStatus Change, Updates Line Item Statuses, Updates Order Statuses, Sends Necessary Emails
        /// </summary>
  
        // all line item status changes should go through here
        public async Task<List<MarketplaceLineItem>> UpdateLineItemStatusesWithNotification(OrderDirection orderDirection, string orderID, LineItemStatusChanges lineItemStatusChanges, VerifiedUserContext verifiedUser = null)
        {
            var userType = verifiedUser?.UsrType ?? "noUser";
            var verifiedUserType = userType.Reserialize<VerifiedUserType>();

            var previousLineItemsStates = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, orderID);

            ValidateLineItemStatusChange(previousLineItemsStates.Items.ToList(), lineItemStatusChanges, verifiedUserType);
            var updatedLineItems = await Throttler.RunAsync(lineItemStatusChanges.LineItemChanges, 100, 5, (lineItemStatusChange) =>
            {
                var newPartialLineItem = BuildNewPartialLineItem(lineItemStatusChange, previousLineItemsStates.Items.ToList(), lineItemStatusChanges.LineItemStatus);
               // if there is no verified user passed in it has been called from somewhere else in the code base and will be done with the client grant access
               return verifiedUser != null ? _oc.LineItems.PatchAsync<MarketplaceLineItem>(orderDirection, orderID, lineItemStatusChange.LineItemID, newPartialLineItem, verifiedUser.AccessToken) : _oc.LineItems.PatchAsync<MarketplaceLineItem>(orderDirection, orderID, lineItemStatusChange.LineItemID, newPartialLineItem);
            });

            var statusSync = SyncOrderStatuses(orderDirection, orderID);
            var notifictionSender = HandleLineItemStatusChangeNotification(verifiedUserType, orderID, lineItemStatusChanges, previousLineItemsStates.Items.ToList());
            
            await statusSync;
            await notifictionSender;

            return updatedLineItems.ToList();
        }

        private async Task SyncOrderStatuses(OrderDirection orderDirection, string orderID)
        {
            var buyerOrderID = orderID.Split('-')[0];
            var buyerOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, buyerOrderID);
            var relatedSupplierOrderIDs = buyerOrder.xp.SupplierIDs.Select(supplierID => $"{buyerOrderID}-{supplierID}");

            await SyncOrderStatus(OrderDirection.Incoming, buyerOrderID);

            foreach(var supplierOrderID in relatedSupplierOrderIDs) {
                await SyncOrderStatus(OrderDirection.Outgoing, supplierOrderID);
            }
        }

        private async Task SyncOrderStatus(OrderDirection orderDirection, string orderID)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(orderDirection, orderID);
            var (SubmittedOrderStatus, ShippingStatus, ClaimStatus) = LineItemStatusConstants.GetOrderStatuses(lineItems.Items.ToList());
            var partialOrder = new PartialOrder()
            {
                xp = new
                {
                    SubmittedOrderStatus,
                    ShippingStatus,
                    ClaimStatus
                }
            };
            await _oc.Orders.PatchAsync(orderDirection, orderID, partialOrder);
        }

        private PartialLineItem BuildNewPartialLineItem(LineItemStatusChange lineItemStatusChange, List<MarketplaceLineItem> previousLineItemStates, LineItemStatus newLineItemStatus)
        {
            var existingLineItem = previousLineItemStates.First(li => li.ID == lineItemStatusChange.LineItemID);
            var quantitySetting = GetQuantityBeingChanged(lineItemStatusChange.PreviousQuantities);
            var LineItemStatusByQuantity = BuildNewLineItemStatusByQuantity(lineItemStatusChange, existingLineItem, newLineItemStatus);
            if (newLineItemStatus == LineItemStatus.ReturnRequested || newLineItemStatus == LineItemStatus.Returned)
            {
                var returnRequests = existingLineItem.xp.Returns ?? new List<LineItemClaim>();
                return new PartialLineItem()
                {
                    xp = new
                    {
                        ReturnRequests = GetUpdatedChangeRequests(returnRequests, lineItemStatusChange, quantitySetting, newLineItemStatus, LineItemStatusByQuantity),
                        LineItemStatusByQuantity 
                    }
                };
            } else if(newLineItemStatus == LineItemStatus.CancelRequested || newLineItemStatus == LineItemStatus.Canceled)
            {
                var cancelRequests = existingLineItem.xp.Cancelations ?? new List<LineItemClaim>();
                return new PartialLineItem()
                {
                    xp = new
                    {
                        CancelationRequests = GetUpdatedChangeRequests(cancelRequests, lineItemStatusChange, quantitySetting, newLineItemStatus, LineItemStatusByQuantity),
                        LineItemStatusByQuantity
                    }
                };
            } else
            {
                return new PartialLineItem()
                {
                    xp = new
                    {
                        LineItemStatusByQuantity
                    }
                };
            }
        }

        private List<LineItemClaim> GetUpdatedChangeRequests(List<LineItemClaim> existinglineItemStatusChangeRequests, LineItemStatusChange lineItemStatusChange, int QuantitySetting, LineItemStatus newLineItemStatus, Dictionary<LineItemStatus, int> lineItemStatuses)
        {
            if(newLineItemStatus == LineItemStatus.Returned || newLineItemStatus == LineItemStatus.Canceled) 
            {
                // go through the return requests and resolve each request until there aren't enough returned or canceled items 
                // to resolve an additional request
                var numberReturnedOrCanceled = lineItemStatuses[newLineItemStatus];
                var currentClaimIndex = 0;
                while (numberReturnedOrCanceled > 0 && currentClaimIndex < existinglineItemStatusChangeRequests.Count()) { 
                    if(existinglineItemStatusChangeRequests[currentClaimIndex].Quantity <= numberReturnedOrCanceled)
                    {
                        existinglineItemStatusChangeRequests[currentClaimIndex].IsResolved = true;
                        currentClaimIndex++;
                        numberReturnedOrCanceled -= existinglineItemStatusChangeRequests[currentClaimIndex].Quantity;
                    } else
                    {
                        currentClaimIndex++;
                    }
                }
            } else
            {
                existinglineItemStatusChangeRequests.Add(new LineItemClaim()
                {
                    Comment = lineItemStatusChange.Comment,
                    Reason = lineItemStatusChange.Reason,
                    IsResolved = false,
                    Quantity = QuantitySetting
                });

            }
            
            return existinglineItemStatusChangeRequests;
        }

        private int GetQuantityBeingChanged(Dictionary<LineItemStatus, int> previousQuantities)
        {
            return previousQuantities.Aggregate(0, (currentCount, previousQuantity) =>
            {
                var value = previousQuantity.Value;
                return currentCount + value;
            });
        }

        private Dictionary<LineItemStatus, int> BuildNewLineItemStatusByQuantity(LineItemStatusChange lineItemStatusChange, MarketplaceLineItem existingLineItem, LineItemStatus newLineItemStatus)
        {
            var quantitySetting = GetQuantityBeingChanged(lineItemStatusChange.PreviousQuantities);

            var newStatusDictionary = new Dictionary<LineItemStatus, int>();
            foreach (KeyValuePair<LineItemStatus, int> entry in lineItemStatusChange.PreviousQuantities)
            {
                // decrement the quantity by the quantity changed
                newStatusDictionary[entry.Key] = existingLineItem.xp.StatusByQuantity[entry.Key] - entry.Value;
            }

            newStatusDictionary.Add(newLineItemStatus, quantitySetting);
            return newStatusDictionary;
        }

        private async Task HandleLineItemStatusChangeNotification(VerifiedUserType setterUserType, string orderID, LineItemStatusChanges lineItemStatusChanges, List<MarketplaceLineItem> lineItems)
        {
            var buyerOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID.Split('-')[0]);
            var lineItemsChanged = lineItems.Where(li => lineItemStatusChanges.LineItemChanges.Select(li => li.LineItemID).Contains(li.ID));
            var supplierIDs = lineItems.Select(li => li.SupplierID).Distinct().ToList();
            var suppliers = await Throttler.RunAsync(supplierIDs, 100, 5, supplierID => _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID));

            // currently the only place supplier name is used is when there should be lineitems from only one supplier included on the change, so we can just take the first supplier
            var statusChangeTextDictionary = LineItemStatusConstants.GetStatusChangeEmailText(suppliers.First().Name);

            foreach (KeyValuePair<VerifiedUserType, LineItemEmailDisplayText> entry in statusChangeTextDictionary[lineItemStatusChanges.LineItemStatus]) {
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
                    await _sendgridService.SendLineItemStatusChangeEmail(lineItemStatusChanges, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                }
                else if (userType == VerifiedUserType.admin) {
                    firstName = "PlaceHolderFirstName";
                    lastName = "PlaceHolderLastName";
                    email = "bhickey@four51.com";
                    var shouldNotify = !(LineItemStatusConstants.LineItemStatusChangesDontNotifySetter.Contains(lineItemStatusChanges.LineItemStatus) && setterUserType == VerifiedUserType.admin);
                    if (shouldNotify)
                    {
                        await _sendgridService.SendLineItemStatusChangeEmail(lineItemStatusChanges, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                    }
                } else
                {
                    var shouldNotify = !(LineItemStatusConstants.LineItemStatusChangesDontNotifySetter.Contains(lineItemStatusChanges.LineItemStatus) && setterUserType == VerifiedUserType.supplier);
                    if (shouldNotify)
                    {
                        await Throttler.RunAsync(suppliers, 100, 5, supplier =>
                        {
                            firstName = supplier.xp.SupportContact.Name;
                            lastName = "";
                            email = supplier.xp.SupportContact.Email;
                            return _sendgridService.SendLineItemStatusChangeEmail(lineItemStatusChanges, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                        });
                    }
                }
            }
        }

        private void ValidateLineItemStatusChange(List<MarketplaceLineItem> previousLineItemStates, LineItemStatusChanges lineItemStatusChanges, VerifiedUserType userType)
        {
            /* need to validate 3 things on a lineitem status change
             * 
             * 1) user making the request has the ability to make that line item change based on usertype
             * 2) there are sufficient amount of the previous quantities for each lineitem
             * 3) the previous values are valid for the new values being set
             */

            // 1) 
            var allowedLineItemStatuses = LineItemStatusConstants.ValidLineItemStatusSetByUserType[userType];
            Require.That(allowedLineItemStatuses.Contains(lineItemStatusChanges.LineItemStatus), new ErrorCode("Not authorized to set this status on a lineItem", 400, $"Not authorized to set line items to {lineItemStatusChanges.LineItemStatus}"));

            // 2)
            var areCurrentQuantitiesToSupportChange = lineItemStatusChanges.LineItemChanges.All(lineItemChange =>
            {
                var relatedLineItems = previousLineItemStates.Where(previousState => previousState.ID == lineItemChange.LineItemID);
                if (relatedLineItems.Count() != 1)
                {
                    // if the lineitem is not found on the order, invalid change
                    return false;
                }
                if (lineItemChange.PreviousQuantities == null)
                {
                    return false;
                }

                var existingLineItem = relatedLineItems.First();
                foreach (KeyValuePair<LineItemStatus, int> entry in lineItemChange.PreviousQuantities)
                {
                    var lineItemChangeStatus = entry.Key;
                    var lineItemChangeQuantity = entry.Value;

                    if (existingLineItem.xp.StatusByQuantity == null || !existingLineItem.xp.StatusByQuantity.ContainsKey(lineItemChangeStatus))
                    {
                        return false;
                    }

                    var existingQuantity = existingLineItem.xp.StatusByQuantity[lineItemChangeStatus];
                    if(existingQuantity < lineItemChangeQuantity)
                    {
                        return false;
                    }
                }
                return true;
            });
            Require.That(areCurrentQuantitiesToSupportChange, new ErrorCode("Invalid lineItem status change", 400, $"Current lineitem quantity status on the order are not sufficient to support the requested change"));

            // 3)
            var areValidPreviousStates = lineItemStatusChanges.LineItemChanges.All(lineItemChange =>
            {
                var relatedLineItems = previousLineItemStates.Where(previousState => previousState.ID == lineItemChange.LineItemID);
                var validPreviousStates = LineItemStatusConstants.ValidPreviousStateLineItemChangeMap[lineItemStatusChanges.LineItemStatus];
                var existingLineItem = relatedLineItems.First();
                foreach (KeyValuePair<LineItemStatus, int> entry in lineItemChange.PreviousQuantities)
                {
                    var lineItemChangeStatus = entry.Key;
                    var lineItemChangeQuantity = entry.Value;

                    if (!validPreviousStates.Contains(lineItemChangeStatus))
                    {
                        return false;
                    }
                }
                return true;
            });
            Require.That(areValidPreviousStates, new ErrorCode("Invalid lineItem status change", 400, $"The previous line item statuses you are attempting to change cannot all be changed to the new Line Item Status"));
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

            // should be among the only line item status changes not handled by the updatelineitemstatuswithnotification function
            liReq.xp.StatusByQuantity.Add(LineItemStatus.Open, liReq.Quantity);
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