using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ordercloud.integrations.library;
using OrderCloud.AzureStorage;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IProductUpdateCommand
    {
        Task CleanUpProductHistoryData();
        Task SendAllProductUpdateEmails();
       //    Task SendProductUpdateEmail(string supplierID);
        ISheet SetHeaders(List<string> headers, ISheet worksheet);
        ISheet SetValues(IEnumerable<object> data, ISheet worksheet);
    }

    public class ProductUpdateCommand : IProductUpdateCommand
    {
        private readonly ProductHistoryQuery _productUpdate;
        private readonly IOrderCloudClient _oc;
        private readonly BlobService _blob;
        private readonly CloudBlobContainer _container;
        private readonly ISendgridService _sendgridService;
        public ProductUpdateCommand(IOrderCloudClient oc, ProductHistoryQuery productUpdate, BlobService blob, ISendgridService sendGrid)
        {
            _productUpdate = productUpdate;
            _oc = oc;
            _blob = blob;
            _container = _blob.BlobClient.GetContainerReference("productupdate");
            _sendgridService = sendGrid;
    }

        public async Task SendAllProductUpdateEmails()
        {
            var productUpdateData = await BuildProductUpdateData();
            var excel = new XSSFWorkbook();
            var worksheet = excel.CreateSheet("ProductUpdate");
            var date = DateTime.UtcNow.ToString("MMddyyyy");
            var time = DateTime.Now.ToString("hmmss.ffff");
            var fileName = $"ProductUpdate-{date}-{time}.xlsx";
            var fileReference = _container.GetAppendBlobReference(fileName);
            var headers = typeof(ProductUpdateData).GetProperties().Select(p => p.Name).ToList(); //    Use the property names as column headers
            var worksheetWithHeaders = SetHeaders(headers, worksheet);
            var worksheetWithData = SetValues(productUpdateData, worksheetWithHeaders);
            //  get every person we are going to send the email to.
            //  send this as an attachment to each of them.
            using (Stream stream = await fileReference.OpenWriteAsync(true))
            {
                excel.Write(stream);
            }
            
        }

        public async Task SendDataToUsers(CloudAppendBlob fileReference, string fileName)
        {
            var usersToSend = await _oc.AdminUsers.ListAsync(filters: "xp.ProductEmails=true");
            foreach(var user in usersToSend.Items) 
            {
                
            }
        }

        public ISheet SetHeaders(List<string> headers, ISheet worksheet)
        {
            var headerRow = worksheet.CreateRow(0);
            for (var i = 0; i < headers.Count(); i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
            }
            return worksheet;
        }

        public ISheet SetValues(IEnumerable<object> data, ISheet worksheet)
        {
            int i = 1;
            var colNumber = worksheet.GetRow(0).Count(); // number of values in first row (headers)
            foreach (var item in data)
            {
                var dataJSON = JObject.FromObject(item);
                IRow sheetRow = worksheet.CreateRow(i++);
                for (var j = 0; j < colNumber; j++)
                {
                    var rowCell = sheetRow.CreateCell(j);
                    var allValues = dataJSON.Values().ToList();
                    var val = allValues[j];
                    var value = val.ToString();
                    rowCell.SetCellValue(value);
                }
            }
            return worksheet;
        }

        public async Task<List<ProductUpdateData>> BuildProductUpdateData()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var yesterDaysUpdates = await _productUpdate.ListProductsByDate(new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0));
            var dataToSend = new List<ProductUpdateData>();
            foreach (var update in yesterDaysUpdates)
            {
                var productUpdates = (await _productUpdate.ListProducts(update.ProductID));
                var previousUpdates = productUpdates.Where(p => p.id != update.id).ToList();
                ProductHistory mostRecentUpdate = null;
                if (previousUpdates.Count != 0)
                {
                    var mostRecentUpdateDate = previousUpdates.Max(p => p.DateLastUpdated);
                    mostRecentUpdate = previousUpdates.Where(p => p.DateLastUpdated == mostRecentUpdateDate).FirstOrDefault();
                }
                var productUpdate = new ProductUpdateData()
                {
                    TimeOfUpdate = update.DateLastUpdated,
                    ProductID = update.ProductID,
                    Action = update.Action
                };
                if (mostRecentUpdate == null || mostRecentUpdate.Product == null)
                {
                    productUpdate.OldProductType = null;
                    productUpdate.OldActiveStatus = null;
                    productUpdate.OldUnitMeasure = null;
                }
                else
                {
                    if (update.Product.xp.ProductType != mostRecentUpdate.Product.xp.ProductType)
                    {
                        productUpdate.OldProductType = mostRecentUpdate.Product.xp.ProductType;
                        productUpdate.NewProductType = update.Product.xp.ProductType;
                    }
                    if (update.Product.xp.UnitOfMeasure != mostRecentUpdate.Product.xp.UnitOfMeasure)
                    {
                        productUpdate.OldUnitMeasure = mostRecentUpdate.Product.xp.UnitOfMeasure;
                        productUpdate.NewUnitMeasure = update.Product.xp.UnitOfMeasure;
                    }
                    if (update.Product.Active != mostRecentUpdate.Product.Active)
                    {
                        productUpdate.OldActiveStatus = mostRecentUpdate.Product.Active;
                        productUpdate.NewActiveStatus = update.Product.Active;
                    }
                }
                dataToSend.Add(productUpdate);
            }
            return dataToSend;
        }

        //public async Task SendProductUpdateEmail(string supplierID)
        //{
        //    var yesterday = DateTime.Now.AddDays(-1);
        //    var yesterDaysUpdates = _productUpdate.ListProductsByDate(new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0));
        //    var updatedProducts = await _productUpdate.ListProducts(supplierID, now);
        //    //  now we need to just send an email
        //}

        public async Task CleanUpProductHistoryData()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var yesterDaysUpdates = _productUpdate.ListProductsByDate(new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0));
        }
    }


}
