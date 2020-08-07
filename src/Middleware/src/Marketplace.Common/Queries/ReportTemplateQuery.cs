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
            var templates = await _store.Query(
                $"select * from c where c.ReportType='{reportType}'", 
                null, 
                new FeedOptions() { PartitionKey = new PartitionKey($"{verifiedUser.ClientID}") })
                .ToListAsync();
            return templates;
        }
    }
}
