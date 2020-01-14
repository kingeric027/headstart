using System;
using Marketplace.Helpers.Models;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers.Sync
{
    public static class SpecMapper
    {
        public static Spec Map(Marketplace.Helpers.Models.MarketplaceSpec obj)
        {
            var a = new OrderCloud.SDK.Spec
            {
                ID = obj.ID,
                Required = obj.Required,
                AllowOpenText = obj.AllowOpenText,
                DefaultOptionID = obj.DefaultOptionID,
                DefaultValue = obj.DefaultValue,
                DefinesVariant = obj.DefinesVariant,
                ListOrder = obj.ListOrder,
                Name = obj.Name,
                xp = new {
                    UI = JObject.FromObject(obj.UI)
                }
            };
            return a;
        }
    }
}
