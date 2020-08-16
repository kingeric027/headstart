using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using OfficeOpenXml;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using OrderCloud.AzureStorage;
using System;
using static Marketplace.Common.Models.ReportTemplate;
//npoi
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;
using System.Data;

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
            //var headerStyle = excel.CreateCellStyle();
            //var headerFont = excel.CreateFont();
            //headerFont.IsBold = true;
            //headerStyle.SetFont(headerFont);
            worksheet = SetHeaders(headers, worksheet);
            worksheet = SetValues(data, headers, worksheet);
            using (Stream stream = await fileReference.OpenWriteAsync(true))
            {
                excel.Write(stream);
            }
        }

        private ISheet SetHeaders(string[] headers, ISheet worksheet)
        {
            var header = worksheet.CreateRow(0);
            for (var i = 0; i < headers.Count(); i++)
            {
                var cell = header.CreateCell(i);
                var concatHeader = headers[i].Contains(".") ? headers[i].Split('.')[0] == "xp" ? headers[i].Split('.')[1] : $"{headers[i].Split('.')[0]} {headers[i].Split('.')[1]}" : headers[i];
                var humanizedHeader = Regex.Replace(concatHeader, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                cell.SetCellValue(humanizedHeader);
                //cell.CellStyle = headerStyle;
                //heavy
                //worksheet.AutoSizeColumn(i);
            }
            return worksheet;
        }

        private ISheet SetValues(IEnumerable<object> data, string[] headers, ISheet worksheet)
        {
            //DataTable table = new DataTable();

            //foreach (var header in headers)
            //{
            //    table.Columns.Add(header);
            //}

            int i = 1;

            foreach (var item in data)
            {
                var dataJSON = JObject.FromObject(item);
                //DataRow row = table.NewRow();
                IRow sheetRow = worksheet.CreateRow(i++);
                int j = 0;
                foreach (var header in headers)
                {
                    ICell cell = sheetRow.CreateCell(j++);
                    if (header.Contains("."))
                    {
                        var cellValue = dataJSON[header.Split(".")[0]][header.Split(".")[1]];
                        //row[header] = cellValue.ToString();
                        cell.SetCellValue(cellValue.ToString());
                    }
                    else
                    {
                        //row[header] = dataJSON[header].ToString();
                        cell.SetCellValue(dataJSON[header].ToString());
                    }
                    //if (header.Contains("."))
                    //{
                    //    row[header]
                    //    var cellValue = dataJSON[header.Split(".")[0]][header.Split(".")[1]];

                    //    worksheet.Cells[row, col].Value = cellValue.ToString()
                    //}
                }
                //table.Rows.Add(row);
            }

            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    IRow sheetRow = worksheet.CreateRow(i + 1);

            //    for (int j = 0; j < table.Columns.Count; j++)
            //    {
            //        ICell cell = sheetRow.CreateCell(j);
            //        string cellValue = Convert.ToString(table.Rows[i][j]);
            //        cell.SetCellValue(cellValue);
            //    }
            //}


            //int row = 2;
            //foreach (var dataSet in data)
            //{
            //    var dataJSON = JObject.FromObject(dataSet);
            //    int col = 1;
            //    foreach (var header in headers)
            //    {
            //        if (header.Contains("."))
            //        {
            //            var cellValue = dataJSON[header.Split(".")[0]][header.Split(".")[1]];
            //            worksheet.Cells[row, col].Value = cellValue.ToString();
            //        }
            //        else
            //        {
            //            worksheet.Cells[row, col].Value = dataJSON[header].ToString();
            //        }
            //        col++;
            //    }
            //    row++;
            //}
            //return worksheet;
            return worksheet;
        }

        //public async Task ExportToExcel(ReportTypeEnum reportType, string[] headers, IEnumerable<object> data)
        //{
        //    using (var excel = new ExcelPackage())
        //    {
        //        var worksheet = excel.Workbook.Worksheets.Add(reportType.ToString());
        //        worksheet = SetHeaders(headers, worksheet);
        //        worksheet = SetValues(data, headers, worksheet);
        //        excel.Save();
        //        var date = DateTime.UtcNow.ToString("MMddyyyy");
        //        var time = DateTime.Now.ToString("hmmss.ffff");
        //        var fileReference = _container.GetAppendBlobReference($"{reportType}-{date}-{time}.xlsx");
        //        using (Stream stream = await fileReference.OpenWriteAsync(true))
        //        {
        //            excel.SaveAs(stream);
        //        }
        //    }
        //}
        //private ExcelWorksheet SetHeaders(string[] headers, ExcelWorksheet worksheet)
        //{
        //    int col = 1;
        //    foreach (var header in headers)
        //    {
        //        var concatHeader = header.Contains(".") ? header.Split('.')[0] == "xp" ? header.Split('.')[1] : $"{header.Split('.')[0]} {header.Split('.')[1]}" : header;
        //        var humanizedHeader = Regex.Replace(concatHeader, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        //        // [ROW, COLUMN]
        //        worksheet.Cells[1, col].Value = humanizedHeader;
        //        col++;
        //    }
        //    return worksheet;
        //}
        //private ExcelWorksheet SetValues(IEnumerable<object> data, string[] headers, ExcelWorksheet worksheet)
        //{
        //    int row = 2;
        //    foreach (var dataSet in data)
        //    {
        //        var dataJSON = JObject.FromObject(dataSet);
        //        int col = 1;
        //        foreach (var header in headers)
        //        {
        //            if (header.Contains("."))
        //            {
        //                var cellValue = dataJSON[header.Split(".")[0]][header.Split(".")[1]];
        //                worksheet.Cells[row, col].Value = cellValue.ToString();
        //            }
        //            else
        //            {
        //                worksheet.Cells[row, col].Value = dataJSON[header].ToString();
        //            }
        //            col++;
        //        }
        //        row++;
        //    }
        //    return worksheet;
        //}
    }
}
