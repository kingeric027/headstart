using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using OrderCloud.AzureStorage;
using System;
using static Marketplace.Common.Models.ReportTemplate;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;
using Marketplace.Common.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;

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

        public async Task<string> ExportToExcel(ReportTypeEnum reportType, ReportTemplate reportTemplate, IEnumerable<object> data)
        {
            var headers = reportTemplate.Headers.ToArray();
            var excel = new XSSFWorkbook();
            var worksheet = excel.CreateSheet(reportType.ToString());
            var date = DateTime.UtcNow.ToString("MMddyyyy");
            var time = DateTime.Now.ToString("hmmss.ffff");
            var fileName = $"{reportType}-{date}-{time}.xlsx";
            var fileReference = _container.GetAppendBlobReference(fileName);
            SetHeaders(headers, worksheet);
            SetValues(data, headers, worksheet);
            using (Stream stream = await fileReference.OpenWriteAsync(true))
            {
                excel.Write(stream);
            }
            return fileName;
        }

        private void SetHeaders(string[] headers, ISheet worksheet)
        {
            var header = worksheet.CreateRow(0);
            for (var i = 0; i < headers.Count(); i++)
            {
                var cell = header.CreateCell(i);
                //var concatHeader = headers[i].Contains(".") ? headers[i].Split('.')[0] == "xp" ? headers[i].Split('.')[1] : $"{headers[i].Split('.')[0]} {headers[i].Split('.')[1]}" : headers[i];
                var concatHeader = headers[i];
                if (headers[i].Contains("."))
                {
                    var split = headers[i].Split('.');
                    concatHeader = split[split.Length - 1];
                }
                var humanizedHeader = Regex.Replace(concatHeader, "([a-z](?=[0-9A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                cell.SetCellValue(humanizedHeader);
            }
        }

        private void SetValues(IEnumerable<object> data, string[] headers, ISheet worksheet)
        {
            int i = 1;
            foreach (var item in data)
            {
                var dataJSON = JObject.FromObject(item);
                IRow sheetRow = worksheet.CreateRow(i++);
                int j = 0;
                foreach (var header in headers)
                {
                    ICell cell = sheetRow.CreateCell(j++);
                    if (header.Contains("."))
                    {
                        if (dataJSON[header.Split(".")[0]].ToString() != "")
                        {
                            //cell.SetCellValue(dataJSON[header.Split(".")[0]][header.Split(".")[1]].ToString());
                            var split = header.Split(".");
                            var dataValue = dataJSON;
                            //bool valueNotFound = false;
                            for (var k = 0; k < split.Length-1; k++)
                            {
                                var prop = split[k];
                                //if (!dataValue.ContainsKey(prop))
                                //{
                                //    valueNotFound = true;
                                //    break;
                                //}
                                dataValue = JObject.Parse(dataValue[prop].ToString());
                            }
                            cell.SetCellValue(/*(valueNotFound || !dataValue.ContainsKey(split[split.Length - 1])) ? "" : */dataValue[split[split.Length-1]].ToString());
                        } else
                        {
                            cell.SetCellValue("");
                        }
                    }
                    else
                    {
                        if (header == "Status")
                        {
                            cell.SetCellValue(Enum.GetName(typeof(OrderStatus), Convert.ToInt32(dataJSON[header])));
                        } else
                        {
                            cell.SetCellValue(dataJSON[header].ToString());
                        }
                    }
                }
            }
        }

        public string GetSharedAccessSignature(string fileName)
        {
            var fileReference = _container.GetBlobReference(fileName);
            var sharedAccessPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(20),
                Permissions = SharedAccessBlobPermissions.Read
            };
            return fileReference.GetSharedAccessSignature(sharedAccessPolicy);
        }

    }
}
