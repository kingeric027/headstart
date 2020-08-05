using System.IO;
using System.Threading.Tasks;
using OrderCloud.SDK;
using Microsoft.WindowsAzure.Storage.Blob;
using OfficeOpenXml;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using OrderCloud.AzureStorage;
namespace Marketplace.Common.Commands
{
    public class DownloadReportCommand
    {
        private readonly BlobService _blob;
        private readonly CloudBlobContainer _container;


        public DownloadReportCommand(BlobService blob)
        {
            _blob = blob;
            _container = _blob.BlobClient.GetContainerReference("downloads");
        }

        //public void AddDownloadRequestToQueue(string orgID, string reportType)
        //{
        //    var message = "download" + "reportType";
        //    _iq.DropMessage(message, orgID);
        //}

        public async Task ExportToExcel(string reportType, string[] headers, IEnumerable<JObject> data)
        {
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add(reportType);
                worksheet = SetHeaders(headers, worksheet);
                worksheet = SetValues(data, headers, worksheet);
                excel.Save();
                var fileReference = _container.GetAppendBlobReference($"{reportType}.xlsx");

                using (Stream stream = await fileReference.OpenWriteAsync(true))
                {
                    excel.SaveAs(stream);
                }
            }
        }
        private ExcelWorksheet SetHeaders(string[] headers, ExcelWorksheet worksheet)
        {
            int col = 1;
            foreach (var header in headers)
            {
                var concatHeader = header.Contains(".") ? header.Split('.')[0] == "xp" ? header.Split('.')[1] : $"{header.Split('.')[0]} {header.Split('.')[1]}" : header;
                var humanizedHeader = Regex.Replace(concatHeader, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                // [ROW, COLUMN]
                worksheet.Cells[1, col].Value = humanizedHeader;
                col++;
            }
            return worksheet;
        }
        private ExcelWorksheet SetValues(IEnumerable<JObject> data, string[] headers, ExcelWorksheet worksheet)
        {
            int row = 2;
            foreach (var dataSet in data)
            {
                int col = 1;
                foreach (var header in headers)
                {
                    if (header.Contains("."))
                    {
                        var cellValue = dataSet[header.Split(".")[0]][header.Split(".")[1]];
                        worksheet.Cells[row, col].Value = cellValue.ToString();
                    }
                    else
                    {
                        worksheet.Cells[row, col].Value = dataSet[header].ToString();
                    }
                    col++;
                }
                row++;
            }
            return worksheet;
        }
    }
}
