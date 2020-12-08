﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Flurl.Http;
using ordercloud.integrations.library;
using ordercloud.integrations.tecra.Models;
using ordercloud.integrations.tecra.Storage;
using OrderCloud.SDK;
using Polly.Retry;
using Polly;
using Flurl.Http.Configuration;

namespace ordercloud.integrations.tecra
{
    public interface IOrderCloudIntegrationsTecraService
    {
        Task<TecraToken> GetToken();
        Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token);
        Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id);
        Task<string> GetTecraFrame(string token, string id);
        Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder);
        Task<string> GetTecraProofByStoreID(string token, string id);
        Task<string> GetTecraPDFByStoreID(string token, string id);

    }
    public class OrderCloudTecraConfig
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ChiliUrl { get; set; }
        public string Environment { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string WorkspaceID { get; set; }
        public string SettingsID { get; set; }
        public string StoreID { get; set; }
        public string BlobStorageHostUrl { get; set; }
        public string BlobStorageConnectionString { get; set; }
    }



    public class OrderCloudIntegrationsTecraService : IOrderCloudIntegrationsTecraService
    {
        private readonly IFlurlClient _flurl;
        private readonly IChiliBlobStorage _blob;
        public OrderCloudTecraConfig Config { get; }

        public OrderCloudIntegrationsTecraService(OrderCloudTecraConfig config, IChiliBlobStorage blob, IFlurlClientFactory flurlFactory)
        {
            Config = config;
            _flurl = flurlFactory.Get($"{Config.BaseUrl}/");
            _blob = blob;
        }
        private IFlurlRequest Token(string resource)
        {
            return _flurl.Request($"{resource}");
        }
        private IFlurlRequest Request(string resource, string token)
        {
            return _flurl.Request($"{resource}").WithOAuthBearerToken(token);
        }
        public async Task<TecraToken> GetToken() 
        {
            var request = "grant_type=client_credentials&client_id=" + Config.ClientId + "&client_secret=" + Config.ClientSecret;
            return await this.Token("auth/token").PostStringAsync(request).ReceiveJson<TecraToken>();
        }

        public async Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token)
        {
            return await this.Request("api/chili/documents", token).SetQueryParam("storeid", Config.StoreID).GetJsonAsync<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder)
        {
            TecraDocumentRequest request = new TecraDocumentRequest();
            request.chiliurl = Config.ChiliUrl;
            request.environment = Config.Environment;
            request.username = Config.Username;
            request.password = Config.Password;
            request.folder = folder;
            return await this.Request("api/chili/alldocuments", token).PostJsonAsync(request).ReceiveJson<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id)
        {
            TecraDocumentRequest request = new TecraDocumentRequest();
            return await this.Request($"api/chili/{id}/variabledefinitions", token).SetQueryParam("storeid", Config.StoreID).GetJsonAsync<TecraSpec[]>();
        }
        public async Task<string> GetTecraFrame(string token, string id)
        {
            //TODO - Make wsid and folder dynamic
            TecraFrameParams tparams = new TecraFrameParams();
            tparams.docid = id;
            tparams.storeid= Config.StoreID;
            tparams.wsid = Config.WorkspaceID;
            tparams.folder = "root";
            tparams.vpid = "";

            return await this.Request($"api/v1/chili/loadtemplatebystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }

        private AsyncRetryPolicy Retry()
        {
            // retries three times, waits five seconds in-between failures
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                });
        }
        private async Task<string> DownloadProof(string token, string id) 
        {
            TecraProofParams tparams = new TecraProofParams();
            string azureFilePath = "";
            tparams.docid = id;
            tparams.storeid = Config.StoreID;
            tparams.page = 1;

            //Download the png
            string proofURL = await this.Request("api/v1/chili/getproofimagebystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
            string fileName = id + ".png";

            //Save it in Azure Storage
            WebClient wc = new WebClient();
            using (MemoryStream stream = new MemoryStream(wc.DownloadData(proofURL)))
            {
                azureFilePath = await _blob.UploadAsset(fileName, stream, "image/png");
            }

            return azureFilePath;

        }
        
        private async Task<string> DownloadPDF(string token, string id) 
        {
            //TODO - Make wsid and folder dynamic
            TecraPDFParams tparams = new TecraPDFParams();
            string azureFilePath = "";
            tparams.docid = id;
            tparams.storeid = Config.StoreID;
            tparams.settingsid = Config.SettingsID;

            //Download the png
            string pdfURL = await this.Request("api/v1/chili/generatepdfbystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
            string fileName = id + ".pdf";

            //Save it in Azure Storage
            WebClient wc = new WebClient();
            using (MemoryStream stream = new MemoryStream(wc.DownloadData(pdfURL)))
            {
                azureFilePath = await _blob.UploadAsset(fileName, stream, "application/pdf");
            }

            return azureFilePath;

        }

        public async Task<string> GetTecraProofByStoreID(string token, string id)
        {
            return await Retry()
                    .ExecuteAsync(() => {
                        return DownloadProof(token, id);
                    });
            
        }
        public async Task<string> GetTecraPDFByStoreID(string token, string id)
        {
            return await Retry()
                    .ExecuteAsync(() => {
                        return DownloadPDF(token, id);
                    });

        }
    }
}
