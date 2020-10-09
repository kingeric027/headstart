using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.CosmosDB.BulkExecutor.BulkUpdate;
using System.Linq;
using Microsoft.Azure.Documents.Linq;
using ordercloud.integrations.library;
using Cosmonaut.Attributes;
using Newtonsoft.Json.Linq;

namespace ordercloud.integrations.library
{
    public interface ICosmosBulkEditor
	{
        Task RunBulkUpdateAsync<T>(string collectionName, Func<JObject, List<UpdateOperation>> updateFunc) where T : CosmosObject;
    }

    // https://github.com/Azure/azure-cosmosdb-bulkexecutor-dotnet-getting-started/blob/master/BulkUpdateSample/BulkUpdateSample/Program.cs
    public class CosmosBulkEditor : ICosmosBulkEditor
    {
        private readonly string DatabaseName;
        private readonly DocumentClient client;

        public CosmosBulkEditor(CosmosConfig config)
        {
            DatabaseName = config.DatabaseName;
            client = new DocumentClient(new Uri(config.EndpointUri), config.PrimaryKey, new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            });
        }

        public async Task RunBulkUpdateAsync<T>(string collectionName, Func<JObject, List<UpdateOperation>> updateFunc) where T: CosmosObject
        {
            var collection = GetCollectionIfExists(client, DatabaseName, collectionName);
            var partitionKeyProperty = typeof(T).GetProperties().FirstOrDefault(prop => prop.HasAttribute<CosmosPartitionKeyAttribute>());

            // Prepare for bulk update.
            long totalNumberOfDocumentsUpdated = 0;
            double totalTimeTakenSec = 0;
            BulkUpdateResponse bulkUpdateResponse = null;
            var token = new CancellationTokenSource().Token;

            // Set retry options high for initialization (default values).
            client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
            client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 9;

            IBulkExecutor bulkExecutor = new BulkExecutor(client, collection);
            await bulkExecutor.InitializeAsync();

            // Set retries to 0 to pass control to bulk executor.
            client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 0;
            client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 0;

            // Generate update items.
            var documents = await GetAllExistingDocuments(collection);
            Console.WriteLine(String.Format("\nFound {0} Documents to update. Beginning.", documents.Count));

            var updateItems = documents.Select(doc => {
                var partitionKey = doc.Value<string>(partitionKeyProperty.Name);
                var id = doc.Value<string>("id");
                return new UpdateItem(id, partitionKey, updateFunc(doc));
            }).ToList();
         
            // Invoke bulk update
            await Task.Run(async () =>
            {
                do
                {
                    try
                    {
                        bulkUpdateResponse = await bulkExecutor.BulkUpdateAsync(
                            updateItems: updateItems,
                            maxConcurrencyPerPartitionKeyRange: null,
                            cancellationToken: token);
                    }
                    catch (DocumentClientException de)
                    {
                        Console.WriteLine("Document client exception: {0}", de);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: {0}", e);
                        break;
                    }
                } while (bulkUpdateResponse.NumberOfDocumentsUpdated < updateItems.Count);

                LogProgress(bulkUpdateResponse);

                totalNumberOfDocumentsUpdated += bulkUpdateResponse.NumberOfDocumentsUpdated;
                totalTimeTakenSec += bulkUpdateResponse.TotalTimeTaken.TotalSeconds;
            },
            token);
        }

        private async Task<List<JObject>> GetAllExistingDocuments(DocumentCollection collection)
		{
            var list = new List<JObject>();
            using (var queryable = client.CreateDocumentQuery<JObject>(collection.SelfLink).AsDocumentQuery())
            {
                while (queryable.HasMoreResults)
                {
                    var batch = await queryable.ExecuteNextAsync<JObject>();
                    list = list.Concat(batch).ToList();
                }
            }
            return list;
        }

        private static DocumentCollection GetCollectionIfExists(DocumentClient client, string databaseName, string collectionName)
        {
            if (GetDatabaseIfExists(client, databaseName) == null) return null;

            return client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(databaseName))
                .Where(c => c.Id == collectionName).AsEnumerable().FirstOrDefault();
        }

        private static Database GetDatabaseIfExists(DocumentClient client, string databaseName)
        {
            return client.CreateDatabaseQuery().Where(d => d.Id == databaseName).AsEnumerable().FirstOrDefault();
        }

        private void LogProgress(BulkUpdateResponse response)
        {
            Console.WriteLine(String.Format("\nSummary for collection"));
            Console.WriteLine("--------------------------------------------------------------------- ");
            Console.WriteLine(String.Format("Updated {0} docs @ {1} updates/s, {2} RU/s in {3} sec",
                response.NumberOfDocumentsUpdated,
                Math.Round(response.NumberOfDocumentsUpdated / response.TotalTimeTaken.TotalSeconds),
                Math.Round(response.TotalRequestUnitsConsumed / response.TotalTimeTaken.TotalSeconds),
                response.TotalTimeTaken.TotalSeconds));
            Console.WriteLine(String.Format("Average RU consumption per document update: {0}",
                (response.TotalRequestUnitsConsumed / response.NumberOfDocumentsUpdated)));
            Console.WriteLine("---------------------------------------------------------------------\n ");
        }
    }
}
