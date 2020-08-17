﻿using System.IO;
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

        public async Task ExportToExcel(ReportTypeEnum reportType, string[] headers, IEnumerable<object> data)
        {
            var excel = new XSSFWorkbook();
            var worksheet = excel.CreateSheet(reportType.ToString());
            var date = DateTime.UtcNow.ToString("MMddyyyy");
            var time = DateTime.Now.ToString("hmmss.ffff");
            var fileReference = _container.GetAppendBlobReference($"{reportType}-{date}-{time}.xlsx");
            SetHeaders(headers, worksheet);
            SetValues(data, headers, worksheet);
            using (Stream stream = await fileReference.OpenWriteAsync(true))
            {
                excel.Write(stream);
            }
        }

        private void SetHeaders(string[] headers, ISheet worksheet)
        {
            var header = worksheet.CreateRow(0);
            for (var i = 0; i < headers.Count(); i++)
            {
                var cell = header.CreateCell(i);
                var concatHeader = headers[i].Contains(".") ? headers[i].Split('.')[0] == "xp" ? headers[i].Split('.')[1] : $"{headers[i].Split('.')[0]} {headers[i].Split('.')[1]}" : headers[i];
                var humanizedHeader = Regex.Replace(concatHeader, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
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
                        var cellValue = dataJSON[header.Split(".")[0]][header.Split(".")[1]];
                        cell.SetCellValue(cellValue.ToString());
                    }
                    else
                    {
                        cell.SetCellValue(dataJSON[header].ToString());
                    }
                }
            }
        }
    }
}
