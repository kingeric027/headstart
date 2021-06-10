using Microsoft.WindowsAzure.Storage.Blob;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headstart.API.Commands
{
    public class StoreFrontCommand
    {

        private readonly IOrderCloudClient _oc;

        public StoreFrontCommand(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task CreateNewStoreFront(ApiClient client)
        {
            // 1. Create new api client
            var createdClient = await _oc.ApiClients.CreateAsync(client);

            //2 call out to Azure to create a new storage container
            string containerName = "container-" + Guid.NewGuid();
            // Get a reference to a sample container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
        }
    }
}
