using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
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
        public ProductUpdateCommand(IOrderCloudClient oc, ProductHistoryQuery productUpdate)
        {
            _productUpdate = productUpdate;
            _oc = oc;
        }

        public async Task SendAllProductUpdateEmails()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var yesterDaysUpdates = await _productUpdate.ListProductsByDate(new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0));
            var dataToSend = new List<ProductUpdateData>();
            foreach (var update in yesterDaysUpdates)
            {
                var productUpdates = (await _productUpdate.ListProducts(update.ProductID));
                var previousUpdates = productUpdates.Where(p => p.id != update.id).ToList();
                ProductHistory mostRecentUpdate = null;
                if(previousUpdates.Count!=0)
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
                if(mostRecentUpdate == null || mostRecentUpdate.Product == null)
                {
                    productUpdate.OldProductType = null;
                    productUpdate.OldActiveStatus = null;
                    productUpdate.OldUnitMeasure = null;
                } else
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
            var excel = new XSSFWorkbook();
            var worksheet = excel.CreateSheet("ProductUpdate");
            var date = DateTime.UtcNow.ToString("MMddyyyy");
            var time = DateTime.Now.ToString("hmmss.ffff");
            var fileName = $"ProductUpdate-{date}-{time}.xlsx";
            var headers = typeof(ProductUpdateData).GetProperties().Select(p => p.Name).ToList(); //    Use the property names as column headers
            var worksheetWithHeaders = SetHeaders(headers, worksheet);
            var worksheetWithData = SetValues(dataToSend, worksheetWithHeaders);
            //  var fileReference = _container.GetAppendBlobReference(fileName);
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
