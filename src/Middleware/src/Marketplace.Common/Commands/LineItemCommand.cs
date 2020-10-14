using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Common.Services;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.library;
using Marketplace.Models.Extended;
using Marketplace.Common.Constants;
using SendGrid.Helpers.Mail;

namespace Marketplace.Common.Commands
{
    public interface ILineItemCommand
    {
        Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem li, VerifiedUserContext verifiedUser);
        Task<List<MarketplaceLineItem>> UpdateLineItemStatusesAndNotifyIfApplicable(OrderDirection orderDirection, string orderID, LineItemStatusChanges lineItemStatusChanges, VerifiedUserContext verifiedUser = null);
        Task<List<MarketplaceLineItem>> SetInitialSubmittedLineItemStatuses(string buyerOrderID);
        Task DeleteLineItem(string orderID, string lineItemID);
    }

    public class LineItemCommand : ILineItemCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly ISendgridService _sendgridService;
        private readonly IMeProductCommand _meProductCommand;
        private readonly IPromotionCommand _promotionCommand;


        public LineItemCommand(ISendgridService sendgridService, IOrderCloudClient oc, IMeProductCommand meProductCommand, IPromotionCommand promotionCommand)
        {
			_oc = oc;
            _sendgridService = sendgridService;
            _meProductCommand = meProductCommand;
            _promotionCommand = promotionCommand;
        }

        // used on post order submit
        public async Task<List<MarketplaceLineItem>> SetInitialSubmittedLineItemStatuses(string buyerOrderID)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, buyerOrderID);
            var updatedLineItems = await Throttler.RunAsync(lineItems.Items, 100, 5, li =>
            {
                var partial = new PartialLineItem()
                {
                    xp = new
                    {
                        StatusByQuantity = new Dictionary<LineItemStatus, int>() {
                            { LineItemStatus.Submitted, li.Quantity },
                            { LineItemStatus.Open, 0 },
                            { LineItemStatus.Backordered, 0 },
                            { LineItemStatus.Canceled, 0 },
                            { LineItemStatus.CancelRequested, 0 },
                            { LineItemStatus.Returned, 0 },
                            { LineItemStatus.ReturnRequested, 0 },
                            { LineItemStatus.Complete, 0 }
                        },
                        Returns = new List<LineItemClaim>(),
                        Cancelations = new List<LineItemClaim>()
                    }
                };
                return _oc.LineItems.PatchAsync<MarketplaceLineItem>(OrderDirection.Incoming, buyerOrderID, li.ID, partial);
            });
            return updatedLineItems.ToList();
        }

        /// <summary>
        /// Validates LineItemStatus Change, Updates Line Item Statuses, Updates Order Statuses, Sends Necessary Emails
        /// </summary>

        // all line item status changes should go through here
        public async Task<List<MarketplaceLineItem>> UpdateLineItemStatusesAndNotifyIfApplicable(OrderDirection orderDirection, string orderID, LineItemStatusChanges lineItemStatusChanges, VerifiedUserContext verifiedUser = null)
        {
            var userType = verifiedUser?.UsrType ?? "noUser";
            var verifiedUserType = userType.Reserialize<VerifiedUserType>();
            
            var buyerOrderID = orderID.Split('-')[0];
            var previousLineItemsStates = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, buyerOrderID);

            ValidateLineItemStatusChange(previousLineItemsStates.Items.ToList(), lineItemStatusChanges, verifiedUserType);
            var updatedLineItems = await Throttler.RunAsync(lineItemStatusChanges.Changes, 100, 5, (lineItemStatusChange) =>
            {
                var newPartialLineItem = BuildNewPartialLineItem(lineItemStatusChange, previousLineItemsStates.Items.ToList(), lineItemStatusChanges.Status);
               // if there is no verified user passed in it has been called from somewhere else in the code base and will be done with the client grant access
               return verifiedUser != null ? _oc.LineItems.PatchAsync<MarketplaceLineItem>(orderDirection, orderID, lineItemStatusChange.ID, newPartialLineItem, verifiedUser.AccessToken) : _oc.LineItems.PatchAsync<MarketplaceLineItem>(orderDirection, orderID, lineItemStatusChange.ID, newPartialLineItem);
            });

            var buyerOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, buyerOrderID);
            var allLineItemsForOrder = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, buyerOrderID);
            var lineItemsChanged = allLineItemsForOrder.Items.Where(li => lineItemStatusChanges.Changes.Select(li => li.ID).Contains(li.ID)).ToList();
            var supplierIDsRelatingToChange = lineItemsChanged.Select(li => li.SupplierID).Distinct().ToList();
            var relatedSupplierOrderIDs = supplierIDsRelatingToChange.Select(supplierID => $"{buyerOrderID}-{supplierID}").ToList();

            var statusSync = SyncOrderStatuses(buyerOrder, relatedSupplierOrderIDs, allLineItemsForOrder.Items.ToList());
            var notifictionSender = HandleLineItemStatusChangeNotification(verifiedUserType, buyerOrder, supplierIDsRelatingToChange, lineItemsChanged, lineItemStatusChanges);
            
            await statusSync;
            await notifictionSender;

            return updatedLineItems.ToList();
        }

        private async Task SyncOrderStatuses(MarketplaceOrder buyerOrder, List<string> relatedSupplierOrderIDs, List<MarketplaceLineItem> allOrderLineItems)
        {
            await SyncOrderStatus(OrderDirection.Incoming, buyerOrder.ID, allOrderLineItems);

            foreach(var supplierOrderID in relatedSupplierOrderIDs) {
                var supplierID = supplierOrderID.Split('-')[1];
                var allOrderLineItemsForSupplierOrder = allOrderLineItems.Where(li => li.SupplierID == supplierID).ToList();
                await SyncOrderStatus(OrderDirection.Outgoing, supplierOrderID, allOrderLineItemsForSupplierOrder);
            }
        }

        private async Task SyncOrderStatus(OrderDirection orderDirection, string orderID, List<MarketplaceLineItem> changedLineItems)
        {
            var (SubmittedOrderStatus, ShippingStatus, ClaimStatus) = LineItemStatusConstants.GetOrderStatuses(changedLineItems);
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
            var existingLineItem = previousLineItemStates.First(li => li.ID == lineItemStatusChange.ID);
            var StatusByQuantity = BuildNewLineItemStatusByQuantity(lineItemStatusChange, existingLineItem, newLineItemStatus);
            if (newLineItemStatus == LineItemStatus.ReturnRequested || newLineItemStatus == LineItemStatus.Returned)
            {
                var returnRequests = existingLineItem.xp.Returns ?? new List<LineItemClaim>();
                return new PartialLineItem()
                {
                    xp = new
                    {
                        Returns = GetUpdatedChangeRequests(returnRequests, lineItemStatusChange, lineItemStatusChange.Quantity, newLineItemStatus, StatusByQuantity),
                        StatusByQuantity
                    }
                };
            } else if(newLineItemStatus == LineItemStatus.CancelRequested || newLineItemStatus == LineItemStatus.Canceled)
            {
                var cancelRequests = existingLineItem.xp.Cancelations ?? new List<LineItemClaim>();
                return new PartialLineItem()
                {
                    xp = new
                    {
                        Cancelations = GetUpdatedChangeRequests(cancelRequests, lineItemStatusChange, lineItemStatusChange.Quantity, newLineItemStatus, StatusByQuantity),
                        StatusByQuantity
                    }
                };
            } else
            {
                return new PartialLineItem()
                {
                    xp = new
                    {
                        StatusByQuantity
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
                        numberReturnedOrCanceled -= existinglineItemStatusChangeRequests[currentClaimIndex].Quantity;
                        currentClaimIndex++;
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

        private Dictionary<LineItemStatus, int> BuildNewLineItemStatusByQuantity(LineItemStatusChange lineItemStatusChange, MarketplaceLineItem existingLineItem, LineItemStatus newLineItemStatus)
        {
            Dictionary<LineItemStatus, int> statusDictionary = existingLineItem.xp.StatusByQuantity;
            var quantitySetting = lineItemStatusChange.Quantity;

            // increment
            statusDictionary[newLineItemStatus] += quantitySetting;


            var validPreviousStates = LineItemStatusConstants.ValidPreviousStateLineItemChangeMap[newLineItemStatus];

            // decrement
            foreach(LineItemStatus status in validPreviousStates)
            {
                if(statusDictionary[status] != 0)
                {
                    if(statusDictionary[status] <= quantitySetting)
                    {
                        quantitySetting -= statusDictionary[status];
                        statusDictionary[status] = 0;
                    } else
                    {
                        statusDictionary[status] -= quantitySetting;
                        quantitySetting = 0;
                    }
                }
            }


            return statusDictionary;
        }

        private async Task HandleLineItemStatusChangeNotification(VerifiedUserType setterUserType, MarketplaceOrder buyerOrder, List<string> supplierIDsRelatedToChange, List<MarketplaceLineItem> lineItemsChanged, LineItemStatusChanges lineItemStatusChanges)
        {
            var suppliers = await Throttler.RunAsync(supplierIDsRelatedToChange, 100, 5, supplierID => _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID));

            // currently the only place supplier name is used is when there should be lineitems from only one supplier included on the change, so we can just take the first supplier
            var statusChangeTextDictionary = LineItemStatusConstants.GetStatusChangeEmailText(suppliers.First().Name);

            foreach (KeyValuePair<VerifiedUserType, LineItemEmailDisplayText> entry in statusChangeTextDictionary[lineItemStatusChanges.Status]) {
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
                    await _sendgridService.SendLineItemStatusChangeEmail(buyerOrder, lineItemStatusChanges, lineItemsChanged.ToList(), firstName, lastName, email, emailText);
                }
                else if (userType == VerifiedUserType.admin) {
                    // Loop over seller users, pull out THEIR boolean, as well as the List<string> of AddtlRcpts
                    var sellerUsers = await _oc.AdminUsers.ListAsync<MarketplaceSellerUser>();
                    var tos = new List<EmailAddress>();
                    foreach (var seller in sellerUsers.Items)
                    {
                        if (seller?.xp?.OrderEmails ?? false)
                        {
                            tos.Add(new EmailAddress(seller.Email));
                        };
                        if (seller?.xp?.AddtlRcpts?.Any() ?? false)
                        {
                            foreach (var rcpt in seller.xp.AddtlRcpts) {
                                tos.Add(new EmailAddress(rcpt));
                            };
                        };
                    };
                    var shouldNotify = !(LineItemStatusConstants.LineItemStatusChangesDontNotifySetter.Contains(lineItemStatusChanges.Status) && setterUserType == VerifiedUserType.admin);
                    if (shouldNotify)
                    {
                        await _sendgridService.SendLineItemStatusChangeEmailMultipleRcpts(buyerOrder, lineItemStatusChanges, lineItemsChanged.ToList(), tos, emailText);
                    }
                } else
                {
                    var shouldNotify = !(LineItemStatusConstants.LineItemStatusChangesDontNotifySetter.Contains(lineItemStatusChanges.Status) && setterUserType == VerifiedUserType.supplier);
                    if (shouldNotify)
                    {
                        await Throttler.RunAsync(suppliers, 100, 5, async supplier =>
                        {
                            if (supplier?.xp?.NotificationRcpts?.Any() ?? false)
                            {
                                var tos = new List<EmailAddress>();
                                foreach (var rcpt in supplier.xp.NotificationRcpts)
                                {
                                    tos.Add(new EmailAddress(rcpt));
                                };
                                await _sendgridService.SendLineItemStatusChangeEmailMultipleRcpts(buyerOrder, lineItemStatusChanges, lineItemsChanged.ToList(), tos, emailText);
                            }
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
             */

            // 1) 
            var allowedLineItemStatuses = LineItemStatusConstants.ValidLineItemStatusSetByUserType[userType];
            Require.That(allowedLineItemStatuses.Contains(lineItemStatusChanges.Status), new ErrorCode("Not authorized to set this status on a lineItem", 400, $"Not authorized to set line items to {lineItemStatusChanges.Status}"));

            // 2)
            var areCurrentQuantitiesToSupportChange = lineItemStatusChanges.Changes.All(lineItemChange =>
            {
                return ValidateCurrentQuantities(previousLineItemStates, lineItemChange, lineItemStatusChanges.Status);
            });
            Require.That(areCurrentQuantitiesToSupportChange, new ErrorCode("Invalid lineItem status change", 400, $"Current lineitem quantity statuses on the order are not sufficient to support the requested change"));
        }

        public bool ValidateCurrentQuantities(List<MarketplaceLineItem> previousLineItemStates, LineItemStatusChange lineItemStatusChange, LineItemStatus lineItemStatusChangingTo)
        {
            var relatedLineItems = previousLineItemStates.Where(previousState => previousState.ID == lineItemStatusChange.ID);
            if (relatedLineItems.Count() != 1)
            {
                // if the lineitem is not found on the order, invalid change
                return false;
            }

            var existingLineItem = relatedLineItems.First();

            var existingStatusByQuantity = existingLineItem.xp.StatusByQuantity;
            if (existingStatusByQuantity == null)
            {
                return false;
            }

            var countCanBeChanged = 0;
            var validPreviousStates = LineItemStatusConstants.ValidPreviousStateLineItemChangeMap[lineItemStatusChangingTo];

            foreach (KeyValuePair<LineItemStatus, int> entry in existingStatusByQuantity)
            {
                if (validPreviousStates.Contains(entry.Key)) {
                    countCanBeChanged += entry.Value;
                }
            }
            return countCanBeChanged >= lineItemStatusChange.Quantity;
        }

        public async Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem liReq, VerifiedUserContext user)
        {
            // get me product with markedup prices correct currency and the existing line items in parellel
            var productRequest = _meProductCommand.Get(liReq.ProductID, user);
            var existingLineItemsRequest = _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, orderID, null, user.AccessToken);

            var existingLineItems = await existingLineItemsRequest;
            var li = new MarketplaceLineItem();
            
            var product = await productRequest;
            var markedUpPrice = GetLineItemUnitCost(product, liReq);
            liReq.UnitPrice = markedUpPrice;

            liReq.xp.StatusByQuantity = LineItemStatusConstants.EmptyStatuses;
            liReq.xp.StatusByQuantity[LineItemStatus.Open] = liReq.Quantity;

            var preExistingLi = ((List<MarketplaceLineItem>)existingLineItems.Items).Find(eli => LineItemsMatch(eli, liReq));
            if (preExistingLi != null)
            {
                li = await _oc.LineItems.SaveAsync<MarketplaceLineItem>(OrderDirection.Incoming, orderID, preExistingLi.ID, liReq);
            } else
            {
                li = await _oc.LineItems.CreateAsync<MarketplaceLineItem>(OrderDirection.Incoming, orderID, liReq);
            }
            await _promotionCommand.AutoApplyPromotions(orderID);
            return li;
        }

        public async Task DeleteLineItem(string orderID, string lineItemID)
        {
            await _oc.LineItems.DeleteAsync(OrderDirection.Incoming, orderID, lineItemID);
            await _promotionCommand.AutoApplyPromotions(orderID);
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
            if (!String.IsNullOrEmpty(li2.xp.PrintArtworkURL)) 
            {
                if (li2.xp.PrintArtworkURL != li1.xp.PrintArtworkURL) return false;
            }
            foreach (var spec1 in li1.Specs) {
                var spec2 = (li2.Specs as List<LineItemSpec>)?.Find(s => s.SpecID == spec1.SpecID);
                if (spec1?.Value != spec2?.Value) return false;
            }
            return true;
        }
    };
}