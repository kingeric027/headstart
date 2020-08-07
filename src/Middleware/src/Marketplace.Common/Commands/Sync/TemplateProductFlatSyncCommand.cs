using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Commands.SupplierSync;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Queries;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Models.Extended;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;

namespace Marketplace.Common.Commands
{
    public class TemplateProductFlatSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AssetQuery _assets;
        public TemplateProductFlatSyncCommand(AppSettings settings, LogQuery log, IOrderCloudClient oc, AssetQuery assets) : base(settings, oc, assets, log)
        {
            _oc = oc;
            _assets = assets;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<TemplateProductFlat>();
            try
            {
                obj.ID = wi.RecordId;
                var priceSchedule = new MarketplacePriceSchedule()
                {
                    ID = obj.ID,
                    ApplyShipping = obj.ApplyShipping,
                    ApplyTax = obj.ApplyTax,
                    MaxQuantity = obj.MaxQuantity,
                    MinQuantity = obj.MinQuantity,
                    Name = obj.Name,
                    PriceBreaks = new List<PriceBreak>()
                        {new PriceBreak() {Price = obj.Price.To<decimal>(), Quantity = obj.QuantityMultiplier}},
                    RestrictedQuantity = obj.RestrictedQuantity,
                    UseCumulativeQuantity = obj.UseCumulativeQuantity
                };
                var product = new MarketplaceProduct()
                {
                    Active = true,
                    AutoForward = false,
                    DefaultSupplierID = wi.ResourceId,
                    ID = obj.ID,
                    Name = obj.Name,
                    Description = obj.Description,
                    QuantityMultiplier = obj.QuantityMultiplier,
                    ShipWeight = obj.ShipWeight,
                    ShipLength = obj.ShipLength,
                    ShipHeight = obj.ShipHeight,
                    ShipWidth = obj.ShipWidth,
                    xp = new ProductXp()
                    {
                        IsResale = obj.IsResale,
                        Accessorials = new List<ProductAccessorial>(),
                        Tax = new TaxProperties()
                        {
                            Category = obj.TaxCategory,
                            Code = obj.TaxCode,
                            Description = obj.TaxDescription
                        },
                        UnitOfMeasure = new UnitOfMeasure()
                        {
                            Qty = obj.UnitOfMeasureQty,
                            Unit = obj.UnitOfMeasure
                        }
                    }
                };
                var image = new Asset();
                var asset = new AssetUpload()
                {
                    ID = wi.RecordId,
                    Type = AssetType.Image,
                    Active = true,
                    Url = obj.Url,
                    FileName = obj.FileName,
                    Title = obj.ImageTitle,
                    Tags = obj.Tags, //.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                };
                try
                {
                    priceSchedule = await _oc.PriceSchedules.CreateAsync<MarketplacePriceSchedule>(priceSchedule, wi.Token);
                }
                catch (OrderCloudException exists) when (IdExists(exists))
                {
                    //continue on
                }
                finally
                {
                    product = await _oc.Products.CreateAsync<MarketplaceProduct>(product, wi.Token);
                    image = await _assets.Create(asset, await new VerifiedUserContext().Define(wi.Token));
                    await _oc.Products.SaveAssignmentAsync(new ProductAssignment()
                    {
                        ProductID = product.ID,
                        PriceScheduleID = priceSchedule.ID
                    }, wi.Token);
                }
                
                return JObject.FromObject(Map(product, priceSchedule, image));
            }
            catch (OrderCloudException exId) when (IdExists(exId))
            {
                // handle 409 errors by refreshing cache
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateExistsError,
                    Message = exId.Message,
                    Level = LogLevel.Error, 
                    OrderCloudErrors = exId.Errors
                });
                return await GetAsync(wi);
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error, 
                    OrderCloudErrors = ex.Errors
                });
                throw new Exception(OrchestrationErrorType.CreateGeneralError.ToString(), ex);
            }
            catch (Exception e)
            {
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateGeneralError,
                    Message = e.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.CreateGeneralError.ToString(), e);
            }
        }

        public async Task<JObject> UpdateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<TemplateProductFlat>(OrchestrationSerializer.Serializer);
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                var priceSchedule = await _oc.PriceSchedules.SaveAsync<MarketplacePriceSchedule>(wi.RecordId, new MarketplacePriceSchedule()
                {
                    ID = obj.ID,
                    ApplyShipping = obj.ApplyShipping,
                    ApplyTax = obj.ApplyTax,
                    MaxQuantity = obj.MaxQuantity,
                    MinQuantity = obj.MinQuantity,
                    Name = obj.Name,
                    PriceBreaks = new List<PriceBreak>() { new PriceBreak() { Price = obj.Price.To<decimal>(), Quantity = obj.QuantityMultiplier } },
                    RestrictedQuantity = obj.RestrictedQuantity,
                    UseCumulativeQuantity = obj.UseCumulativeQuantity
                }, wi.Token);
                var product = await _oc.Products.SaveAsync<MarketplaceProduct>(wi.RecordId, new MarketplaceProduct()
                {
                    Active = true,
                    AutoForward = false,
                    DefaultSupplierID = wi.ResourceId,
                    ID = obj.ID,
                    Name = obj.Name,
                    Description = obj.Description,
                    QuantityMultiplier = obj.QuantityMultiplier,
                    ShipWeight = obj.ShipWeight,
                    ShipLength = obj.ShipLength,
                    ShipHeight = obj.ShipHeight,
                    ShipWidth = obj.ShipWidth,
                    xp = new ProductXp()
                    {
                        IsResale = obj.IsResale,
                        Accessorials = new List<ProductAccessorial>(),
                        Tax = new TaxProperties()
                        {
                            Category = obj.TaxCategory,
                            Code = obj.TaxCode,
                            Description = obj.TaxDescription
                        },
                        UnitOfMeasure = new UnitOfMeasure()
                        {
                            Qty = obj.UnitOfMeasureQty,
                            Unit = obj.UnitOfMeasure
                        }
                    }
                }, wi.Token);
                var image = await _assets.Update(wi.RecordId, new Asset()
                {
                    Type = AssetType.Image,
                    Active = true,
                    Url = obj.Url,
                    FileName = obj.FileName,
                    Title = obj.ImageTitle,
                    Tags = obj.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                }, await new VerifiedUserContext().Define(wi.Token));
                await _oc.Products.SaveAssignmentAsync(new ProductAssignment()
                {
                    ProductID = product.ID,
                    PriceScheduleID = priceSchedule.ID
                }, wi.Token);
                return JObject.FromObject(Map(product, priceSchedule, image));
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.UpdateGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.UpdateGeneralError.ToString(), ex);
            }
        }

        private static TemplateProductFlat Map(MarketplaceProduct product, MarketplacePriceSchedule priceSchedule, Asset asset = null)
        {
            return new TemplateProductFlat
            {
                ID = product.ID,
                Active = product.Active,
                ApplyShipping = priceSchedule.ApplyShipping,
                ApplyTax = priceSchedule.ApplyTax,
                Description = product.Description,
                IsResale = product.xp.IsResale,
                MaxQuantity = priceSchedule.MaxQuantity,
                MinQuantity = priceSchedule.MinQuantity,
                Name = product.Name,
                Price = priceSchedule.PriceBreaks.FirstOrDefault()?.Price,
                ShipHeight = product.ShipHeight,
                ShipLength = product.ShipLength,
                ShipWidth = product.ShipWidth,
                ShipWeight = product.ShipWeight,
                QuantityMultiplier = product.QuantityMultiplier,
                RestrictedQuantity = priceSchedule.RestrictedQuantity,
                TaxCategory = product.xp.Tax.Category,
                TaxCode = product.xp.Tax.Code,
                TaxDescription = product.xp.Tax.Description,
                UseCumulativeQuantity = priceSchedule.UseCumulativeQuantity,
                UnitOfMeasure = product.xp.UnitOfMeasure.Unit,
                UnitOfMeasureQty = product.xp.UnitOfMeasure.Qty,
                ImageTitle = asset?.Title,
                Url = asset?.Url,
                Type = asset?.Type ?? AssetType.Image,
                Tags = asset?.Tags.JoinString(","),
                FileName = asset?.FileName
            };
        }

        public async Task<JObject> PatchAsync(WorkItem wi)
        {
            var objProduct = wi.Diff.ToObject<PartialMarketplaceProduct>(OrchestrationSerializer.Serializer);
            var objPrice = wi.Diff.ToObject<PartialPriceSchedule>(OrchestrationSerializer.Serializer);
            // assets don't support partials
            try
            {
                //TODO: partial mapping
                var product = await _oc.Products.PatchAsync<MarketplaceProduct>(wi.RecordId, objProduct, wi.Token);
                var priceSchedule = await _oc.PriceSchedules.PatchAsync<MarketplacePriceSchedule>(wi.RecordId, objPrice, wi.Token);
                return JObject.FromObject(Map(product, priceSchedule, null));
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.PatchGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.PatchGeneralError.ToString(), ex);
            }
        }

        public Task<JObject> DeleteAsync(WorkItem wi)
        {
            throw new NotImplementedException();
        }

        public async Task<JObject> GetAsync(WorkItem wi)
        {
            try
            {
                var product = await _oc.Products.GetAsync<MarketplaceProduct>(wi.RecordId, wi.Token);
                var priceSchedule = await _oc.PriceSchedules.GetAsync<MarketplacePriceSchedule>(product.DefaultPriceScheduleID, wi.Token);
                var image = await _assets.Get(wi.RecordId, await new VerifiedUserContext().Define(wi.Token));
                return JObject.FromObject(Map(product, priceSchedule, image));
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.GetGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.GetGeneralError.ToString(), ex);
            }
        }
    }
}
