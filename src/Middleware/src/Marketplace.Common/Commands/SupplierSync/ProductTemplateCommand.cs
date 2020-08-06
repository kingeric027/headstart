using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Models;
using Microsoft.AspNetCore.Http;
using Npoi.Mapper;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    public interface IProductTemplateCommand
    {
        Task<List<TemplateHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user);
        Task<TemplateProductResult> ParseProductTemplateFlat(IFormFile file, VerifiedUserContext user);
    }

    public class ProductTemplateCommand : IProductTemplateCommand
    {
        private readonly AppSettings _settings;
        public ProductTemplateCommand(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<TemplateProductResult> ParseProductTemplateFlat(IFormFile file, VerifiedUserContext user)
        {
            using var stream = file.OpenReadStream();
            var products = new Mapper(stream).Take<TemplateProductFlat>("TemplateFlat").ToList();
            var result = new TemplateProductResult()
            {
                Invalid = products
                    .Where(p => p.ErrorColumnIndex > -1)
                    .Select(p => new TemplateRowError() { Row = p.RowNumber, ColumnIndex = p.ErrorColumnIndex, ErrorMessage = p.ErrorMessage })
                    .ToList(),
                Valid = products.Where(p => p.Value?.ID != null).Select(p => p.Value).ToList()
            };
            return await Task.FromResult(result);
        }

        public async Task<List<TemplateHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user)
        {
            using var stream = file.OpenReadStream();
            var mapper = new Mapper(stream);
            var products = mapper.Take<TemplateProduct>("Products").Select(s => s.Value).ToList();
            var prices = mapper.Take<TemplatePriceSchedule>("PriceSchedules").Select(s => s.Value).ToList().AsReadOnly();
            var specs = mapper.Take<TemplateSpec>("Specs").Select(s => s.Value).ToList().AsReadOnly();
            var specoptions = mapper.Take<TemplateSpecOption>("SpecOptions").Select(s => s.Value).ToList().AsReadOnly();
            var images = mapper.Take<TemplateAsset>("Images").Select(s => s.Value).ToList().AsReadOnly();
            var attachments = mapper.Take<TemplateAsset>("Attachments").Select(s => s.Value).ToList().AsReadOnly();

            //var list = products.Select(info => new TemplateHydratedProduct()
            //{
            //    Product = info.Value,
            //    PriceSchedule = prices.FirstOrDefault(row => row.Value.ProductID == info.Value.ID)?.Value,
            //    Specs = specs.Where(s => s.Value.ProductID == info.Value.ID).Select(s => s.Value).ToList(),
            //    SpecOptions = specs.Where(s => s.Value.ProductID == info.Value.ID)
            //        .SelectMany(s => specoptions.Where(o => o.Value.SpecID == s.Value.ID).Select(o => o.Value)).ToList(),
            //    Images = images.Where(s => s.Value.ProductID == info.Value.ID).Select(o => o.Value).ToList(),
            //    Attachments = attachments.Where(s => s.Value.ProductID == info.Value.ID).Select(o => o.Value).ToList()
            //});

            var list = new List<TemplateHydratedProduct>();
            foreach (var product in products)
            {
                var p = new TemplateHydratedProduct();
                p.Product = product;
                p.PriceSchedule = prices.FirstOrDefault(row => row.ProductID == product.ID);
                p.Images = images.Where(i => i.ProductID == product.ID).Select(o => o).ToList();
                p.Attachments = attachments.Where(s => s.ProductID == product.ID).Select(o => o).ToList();
                p.Specs = specs.Where(s => s.ProductID == product.ID).Select(s => s).ToList();
                // 1:38
                //var o = from options in specoptions
                //    join sp in specs on options.SpecID equals sp.ID
                //    join pp in products on sp.ProductID equals pp.ID
                //        select options;
                // :53
                var o = specoptions.Where(x => p.Specs.Any(s => s.ID == x.SpecID));
                p.SpecOptions = o.ToList();
                list.Add(p);
            }

            return await Task.FromResult(list.ToList());
        }
    }

    public class TemplateHydratedProduct
    {
        public TemplateProduct Product { get; set; }
        public TemplatePriceSchedule PriceSchedule { get; set; }
        public IList<TemplateSpec> Specs { get; set; }
        public IList<TemplateSpecOption> SpecOptions { get; set; }
        public IList<TemplateAsset> Images { get; set; }
        public IList<TemplateAsset> Attachments { get; set; }
    }

    public class TemplateProductResult
    {
        public List<TemplateProductFlat> Valid = new List<TemplateProductFlat>();
        public List<TemplateRowError> Invalid = new List<TemplateRowError>();
    }

    public class TemplateRowError
    {
        public int Row { get; set; }
        public int ColumnIndex { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TemplateProductFlat : IMarketplaceObject
    {
        [Required]
        public string ID { get; set; }
        public bool Active { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int QuantityMultiplier { get; set; }
        public decimal? ShipWeight { get; set; }
        public decimal? ShipHeight { get; set; }
        public decimal? ShipWidth { get; set; }
        public decimal? ShipLength { get; set; }
        [Required]
        public string TaxCategory { get; set; }
        [Required]
        public string TaxCode { get; set; }
        public string TaxDescription { get; set; }
        public string UnitOfMeasureQty { get; set; }
        public string UnitOfMeasure { get; set; }
        public bool IsResale { get; set; }
        public bool ApplyTax { get; set; }
        public bool ApplyShipping { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public bool UseCumulativeQuantity { get; set; }
        public bool RestrictedQuantity { get; set; }
        [Required]
        public double Price { get; set; }
        public string ImageTitle { get; set; }
        public string Url { get; set; }
        public AssetType Type { get; set; }
        public List<string> Tags { get; set; }
        public string FileName { get; set; }

    }

    public class TemplateProduct
    {
        public string ID { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int QuantityMultiplier { get; set; }
        public decimal? ShipWeight { get; set; }
        public decimal? ShipHeight { get; set; }
        public decimal? ShipWidth { get; set; }
        public decimal? ShipLength { get; set; }
        public string TaxCategory { get; set; }
        public string TaxCode { get; set; }
        public string TaxDescription { get; set; }
        public string UnitOfMeasureQty { get; set; }
        public string UnitOfMeasure { get; set; }
        public bool IsResale { get; set; }
    }

    public class TemplatePriceSchedule
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public bool ApplyTax { get; set; }
        public bool ApplyShipping { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public bool UseCumulativeQuantity { get; set; }
        public bool RestrictedQuantity { get; set; }
    }

    public class TemplateSpec
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public int ListOrder { get; set; }
        public string DefaultValue { get; set; }
        public bool Required { get; set; }
        public bool AllowOpenText { get; set; }
        public string DefaultOptionID { get; set; }
        public bool DefinesVariant { get; set; }
        //public IList<TemplateSpecOption> SpecOptions { get; set; } = new List<TemplateSpecOption>();
    }

    public class TemplateSpecOption
    {
        public string ID { get; set; }
        public string SpecID { get; set; }
        public string Value { get; set; }
        public int ListOrder { get; set; }
        public bool IsOpenText { get; set; }
        public PriceMarkupType PriceMarkupType { get; set; }
        public decimal? PriceMarkup { get; set; }
        public string Description { get; set; }
    }

    public class TemplateAsset
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string Title { get; set; }
        public bool Active { get; set; }
        public string Url { get; set; }
        public AssetType Type { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string FileName { get; set; }
    }
}