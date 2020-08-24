﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using static Marketplace.Common.Models.ReportTemplate;

namespace Marketplace.Common.Queries
{
    public interface IReportTemplateQuery<ReportTemplate>
    {
        Task<List<ReportTemplate>> List(ReportTypeEnum reportType, VerifiedUserContext verifiedUser);
        Task<ReportTemplate> Post(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser);
        Task Delete(string id, VerifiedUserContext verifiedUser);
        Task<ReportTemplate> Get(string id, VerifiedUserContext verifiedUser);
    }

    public class ReportTemplateQuery : IReportTemplateQuery<ReportTemplate>
    {
        private readonly ICosmosStore<ReportTemplate> _store;
        public ReportTemplateQuery(ICosmosStore<ReportTemplate> store)
        {
            _store = store;
        }

        public async Task<List<ReportTemplate>> List(ReportTypeEnum reportType, VerifiedUserContext verifiedUser)
        {
            var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey($"{verifiedUser.SellerID}") };
            var templates = new List<ReportTemplate>();
            if (verifiedUser.UsrType == "admin")
            {
                templates = await _store.Query(feedOptions).Where(x => x.ReportType == reportType).ToListAsync();
            } else if (verifiedUser.UsrType == "supplier")
            {
                templates = await _store.Query(feedOptions).Where(x => x.ReportType == reportType && x.AvailableToSuppliers == true).ToListAsync();
            }
            return templates;
        }

        public async Task<ReportTemplate> Post(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser)
        {
            var template = reportTemplate;
            template.SellerID = verifiedUser.SellerID;
            var newTemplate = await _store.AddAsync(template);
            return newTemplate;
        }

        public async Task Delete(string id, VerifiedUserContext verifiedUser)
        {
            await _store.RemoveByIdAsync(id, verifiedUser.SellerID);
        }

        public async Task<ReportTemplate> Get(string id, VerifiedUserContext verifiedUser)
        {
            var template = await _store.FindAsync(id, verifiedUser.SellerID);
            return template;
        }
    }
}