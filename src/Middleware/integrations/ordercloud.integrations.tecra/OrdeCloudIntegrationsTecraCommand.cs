using System.Collections.Generic;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using ordercloud.integrations.tecra.Models;
using OrderCloud.SDK;

namespace ordercloud.integrations.tecra
{
    public interface IOrderCloudIntegrationsTecraCommand
    {
        Task<string> TecraToken();
        Task<IEnumerable<TecraDocument>> TecraDocuments();
        Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string folder);
        Task<IEnumerable<TecraSpec>> TecraSpecs(string id);
        Task<string> TecraFrame(string id);
        Task<string> TecraProof(string id);
        Task<string> TecraPDF(string id);
    }
    public class OrderCloudIntegrationsTecraCommand : IOrderCloudIntegrationsTecraCommand
    {
        private readonly IOrderCloudIntegrationsTecraService _tecra;
        private readonly IOrderCloudClient _oc;
        public OrderCloudIntegrationsTecraCommand(IOrderCloudIntegrationsTecraService tecra, IOrderCloudClient oc)
        {
            _tecra = tecra;
            _oc = oc;
        }
        public async Task<string> TecraToken()
        {
            TecraToken auth = await _tecra.GetToken();
            return auth.access_token;
        }
        public async Task<IEnumerable<TecraDocument>> TecraDocuments()
        {
            TecraToken auth = await _tecra.GetToken();
            IEnumerable<TecraDocument> documents = await _tecra.GetTecraDocuments(auth.access_token);
            return documents;

        }
        public async Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string folder)
        {
            TecraToken auth = await _tecra.GetToken();
            IEnumerable<TecraDocument> documents = await _tecra.TecraDocumentsByFolder(auth.access_token, folder);
            return documents;

        }
        public async Task<IEnumerable<TecraSpec>> TecraSpecs(string id)
        {
            TecraToken auth = await _tecra.GetToken();
            IEnumerable<TecraSpec> specs = await _tecra.GetTecraSpecs(auth.access_token, id);
            return specs;

        }
        public async Task<string> TecraFrame(string id)
        {
            TecraToken auth = await _tecra.GetToken();
            string specs = await _tecra.GetTecraFrame(auth.access_token, id);
            return specs;

        }
        public async Task<string> TecraProof(string id)
        {
            TecraToken auth = await _tecra.GetToken();
            string specs = await _tecra.GetTecraProofByStoreID(auth.access_token, id);
            return specs;

        }
        public async Task<string> TecraPDF(string id)
        {
            TecraToken auth = await _tecra.GetToken();
            string specs = await _tecra.GetTecraPDFByStoreID(auth.access_token, id);
            return specs;

        }

    }
}
