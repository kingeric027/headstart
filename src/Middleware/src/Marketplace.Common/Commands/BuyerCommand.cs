﻿using Marketplace.Models;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using System.Linq;
using ordercloud.integrations.cms;
using System.Dynamic;
using System;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceBuyerCommand
    {
        Task<SuperMarketplaceBuyer> Create(SuperMarketplaceBuyer buyer, VerifiedUserContext user, bool isSeedingEnvironment = false);
        Task<SuperMarketplaceBuyer> Get(string buyerID, string token = null);
        Task<SuperMarketplaceBuyer> Update(string buyerID, SuperMarketplaceBuyer buyer, string token);
    }
    public class MarketplaceBuyerCommand : IMarketplaceBuyerCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly IDocumentQuery _query;

        public MarketplaceBuyerCommand(AppSettings settings, IOrderCloudClient oc, IDocumentQuery query)
        {
            _settings = settings;
            _oc = oc;
            _query = query;
        }
        public async Task<SuperMarketplaceBuyer> Create(SuperMarketplaceBuyer superBuyer, VerifiedUserContext user, bool isSeedingEnvironment = false)
        {
            var createdBuyer = await CreateBuyerAndRelatedFunctionalResources(superBuyer.Buyer, user, isSeedingEnvironment);
            var createdMarkup = await CreateMarkup(superBuyer.Markup, createdBuyer.ID, user.AccessToken);
            return new SuperMarketplaceBuyer()
            {
                Buyer = createdBuyer,
                Markup = createdMarkup
            };
        }

        public async Task<SuperMarketplaceBuyer> Update(string buyerID, SuperMarketplaceBuyer superBuyer, string token)
        {
            // to prevent changing buyerIDs
            superBuyer.Buyer.ID = buyerID;

            var updatedBuyer = await _oc.Buyers.SaveAsync<MarketplaceBuyer>(buyerID, superBuyer.Buyer, token);
            var updatedMarkup = await UpdateMarkup(superBuyer.Markup, superBuyer.Buyer.ID, token);
            return new SuperMarketplaceBuyer()
            {
                Buyer = updatedBuyer,
                Markup = updatedMarkup
            };
        }

        public async Task<SuperMarketplaceBuyer> Get(string buyerID, string token = null)
        {
            var request = token != null ? _oc.Buyers.GetAsync<MarketplaceBuyer>(buyerID, token) : _oc.Buyers.GetAsync<MarketplaceBuyer>(buyerID);
            var buyer = await request;

            // to move into content docs logic
            var markupPercent = buyer.xp?.MarkupPercent ?? 0;
            var markup = new BuyerMarkup()
            {
                Percent = markupPercent
            };

            return new SuperMarketplaceBuyer()
            {
                Buyer = buyer,
                Markup = markup
            };
        }

        public async Task<MarketplaceBuyer> CreateBuyerAndRelatedFunctionalResources(MarketplaceBuyer buyer, VerifiedUserContext user, bool isSeedingEnvironment = false)
        {
            var token = isSeedingEnvironment ? user.AccessToken : null;
            buyer.ID = "{buyerIncrementor}";
            buyer.Active = true;
            var ocBuyer = await _oc.Buyers.CreateAsync(buyer, user.AccessToken);
            buyer.ID = ocBuyer.ID;
            var ocBuyerID = ocBuyer.ID;

            // create base security profile assignment
            await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
            {
                BuyerID = ocBuyerID,
                SecurityProfileID = CustomRole.MPBaseBuyer.ToString()
            }, token);

            // list message senders
            var msList = await _oc.MessageSenders.ListAsync(accessToken: token);
            // create message sender assignment
            var assignmentList = msList.Items.Select(ms =>
            {
                return new MessageSenderAssignment
                {
                    MessageSenderID = ms.ID,
                    BuyerID = ocBuyerID
                };
            });
            await Throttler.RunAsync(assignmentList, 100, 5, a => _oc.MessageSenders.SaveAssignmentAsync(a, token));

            await _oc.Incrementors.CreateAsync(new Incrementor { ID = $"{ocBuyerID}-UserIncrementor", LastNumber = 0, LeftPaddingCount = 5, Name = "User Incrementor" }, token);
            await _oc.Incrementors.CreateAsync(new Incrementor { ID = $"{ocBuyerID}-LocationIncrementor", LastNumber = 0, LeftPaddingCount = 4, Name = "Location Incrementor" }, token);

            await _oc.Catalogs.SaveAssignmentAsync(new CatalogAssignment()
            {
                BuyerID = ocBuyer.ID,
                CatalogID = ocBuyer.ID,
                ViewAllCategories = true,
                ViewAllProducts = false
            }, token);
            return buyer;
        }

        private async Task<BuyerMarkup> CreateMarkup(BuyerMarkup markup, string buyerID, string token)
        {
            // to move from xp to contentdocs, that logic will go here instead of a patch
            var updatedBuyer = await _oc.Buyers.PatchAsync(buyerID, new PartialBuyer() { xp = new { MarkupPercent = markup.Percent } }, token);
            return new BuyerMarkup()
            {
                Percent = (int)updatedBuyer.xp.MarkupPercent
            };
        }

        private async Task<BuyerMarkup> UpdateMarkup(BuyerMarkup markup, string buyerID, string token)
        {
            // to move from xp to contentdocs, that logic will go here instead of a patch
            // currently duplicate of the function above, this might need to be duplicated since there wont be a need to save the contentdocs assignment again
            var updatedBuyer = await _oc.Buyers.PatchAsync(buyerID, new PartialBuyer() { xp = new { MarkupPercent = markup.Percent } });
            return new BuyerMarkup()
            {
                Percent = (int)updatedBuyer.xp.MarkupPercent
            };
        }
    }
}
