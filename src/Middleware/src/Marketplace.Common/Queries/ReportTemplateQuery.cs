using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Queries
{
    public interface IReportTemplateQuery<ReportTemplate>
    {
        Task<List<ReportTemplate>> List(string reportType, VerifiedUserContext verifiedUser);
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

        public async Task<List<ReportTemplate>> List(string reportType, VerifiedUserContext verifiedUser)
        {
            Console.WriteLine(_store);
            Console.WriteLine(verifiedUser);
            var query = $"select * from c where c.ReportType='{reportType}'";
            if (verifiedUser.UsrType == "supplier")
            {
                query = query + " and c.AvailableToSuppliers='true'";
            }
            var templates = await _store.Query(
                    query,
                    null,
                    new FeedOptions() { PartitionKey = new PartitionKey($"{verifiedUser.ClientID}") })
                    .ToListAsync();
            return templates;
        }

        public async Task<ReportTemplate> Post(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser)
        {
            var template = reportTemplate;
            template.ClientID = verifiedUser.ClientID;
            var newTemplate = await _store.AddAsync(template);
            return newTemplate;
        }

        public async Task Delete(string id, VerifiedUserContext verifiedUser)
        {
            await _store.RemoveByIdAsync(id, verifiedUser.ClientID);
        }

        public async Task<ReportTemplate> Get(string id, VerifiedUserContext verifiedUser)
        {
            var template = await _store.Query(
                $"select * from c where c.id='{id}'",
                null,
                new FeedOptions() { PartitionKey = new PartitionKey($"{verifiedUser.ClientID}") })
                .FirstOrDefaultAsync();
            return template;
        }
    }
}
