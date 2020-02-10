using Marketplace.Common.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceSupplierMapper
    {
        public static Supplier Map(MarketplaceSupplier supplier)
        {
            return new Supplier
            {
                Name = supplier.Name,
                Active = true,
                xp =
                {
                    Description = supplier.Description,
                    SupportContact = new MarketplaceContact
                    {
                        Name = supplier.SupportContact.Name,
                        Phone = supplier.SupportContact.Phone,
                        Email = supplier.SupportContact.Email
                    },
                    LogoURL = supplier.LogoURL
                }
            };
        }

        public static MarketplaceSupplier Map(Supplier supplier)
        {
            return new MarketplaceSupplier
            {
                ID = supplier.ID,
                Name = supplier.Name,
                Active = supplier.Active,
                Description = supplier.xp?.Description,
                LogoURL = supplier.xp?.LogoURL,
                SupportContact = new MarketplaceContact {
                    Name = supplier.xp?.SupportContact.Name,
                    Phone = supplier.xp?.SupportContact.Phone,
                    Email = supplier.xp?.SupportContact.Email
                }
            };
        }
    }
}
