using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.EntityFrameworkCore.Internal;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.Zoho.Mappers
{
    public static class ZohoContactMapper
    {
        public static ZohoContact Map(MarketplaceSupplier supplier, MarketplaceAddressSupplier address, User user, ZohoCurrency currency)
        {
            return new ZohoContact()
            {
                company_name = supplier.ID,
                contact_name = supplier.Name,
                contact_type = "vendor",
                billing_address = ZohoAddressMapper.Map(address),
                shipping_address = ZohoAddressMapper.Map(address),
                contact_persons = new List<ZohoContactPerson>()
                {
                    new ZohoContactPerson()
                    {
                        email = user.Email,
                        first_name = user.FirstName,
                        last_name = user.LastName,
                        phone = user.Phone,
                        is_primary_contact = true
                    }
                },
                currency_id = currency.currency_id
                //TODO: Evaluate model concerns with Avalara integration
                //avatax_use_code = "use code",
                //avatax_exempt_no = "exempt no"
            };
        }

        public static ZohoContact Map(ZohoContact contact, MarketplaceSupplier supplier, MarketplaceAddressSupplier address, User user, ZohoCurrency currency)
        {
            return new ZohoContact()
            {
                contact_id = contact.contact_id,
                company_name = supplier.ID,
                contact_name = supplier.Name,
                contact_type = "vendor",
                billing_address = ZohoAddressMapper.Map(address),
                shipping_address = ZohoAddressMapper.Map(address),
                contact_persons = contact.contact_persons = (contact.contact_persons != null && contact.contact_persons.Any(c => c.email == user.Email))
                    ? new List<ZohoContactPerson>()
                    {
                        new ZohoContactPerson()
                        {
                            email = user.Email,
                            first_name = user.FirstName,
                            last_name = user.LastName,
                            is_primary_contact = false,
                            phone = user.Phone
                        }
                    } : null,
                currency_id = currency.currency_id
                //TODO: Evaluate model concerns with Avalara integration
                //avatax_use_code = "use code",
                //avatax_exempt_no = "exempt no"
            };
        }

        public static ZohoContact Map(MarketplaceBuyer buyer, MarketplaceUser user,ZohoCurrency currency, MarketplaceBuyerLocation location)
        {
            return new ZohoContact()
            {
                company_name = buyer.ID,
                contact_name = buyer.Name,
                contact_type = "customer",
                billing_address = ZohoAddressMapper.Map(location.Address),
                shipping_address = ZohoAddressMapper.Map(location.Address),
                contact_persons = new List<ZohoContactPerson>()
                {
                    new ZohoContactPerson()
                    {
                        email = user.Email,
                        first_name = user.FirstName,
                        last_name = user.LastName,
                        phone = user.Phone,
                        is_primary_contact = true
                    }
                },
                currency_id = currency.currency_id
                //TODO: Evaluate model concerns with Avalara integration
                //avatax_use_code = "use code",
                //avatax_exempt_no = "exempt no"
            };
        }

        public static ZohoContact Map(ZohoContact contact, MarketplaceBuyer buyer, MarketplaceUser user, ZohoCurrency currency, MarketplaceBuyerLocation location)
        {
            contact.company_name = buyer.ID;
            contact.contact_name = buyer.Name;
            contact.contact_type = "customer";
            contact.billing_address = ZohoAddressMapper.Map(location.Address);
            contact.shipping_address = ZohoAddressMapper.Map(location.Address);
            contact.tax_authority_id = contact.tax_authority_id;
            contact.contact_persons = (contact.contact_persons != null && contact.contact_persons.Any(c => c.email == user.Email))
                ? new List<ZohoContactPerson>()
                {
                    new ZohoContactPerson()
                    {
                        email = user.Email,
                        first_name = user.FirstName,
                        last_name = user.LastName,
                        is_primary_contact = false,
                        phone = user.Phone
                    }
                } : null;
            contact.currency_id = currency.currency_id;
            //avatax_use_code = "use code",
            //avatax_exempt_no = "exempt no"
            return contact;
        }
    }

    public static class ZohoLineItemMapper
    {
        public static ZohoLineItem Map(MarketplaceLineItem item, MarketplaceLineItemProduct product, LineItemVariant variant)
        {
            // TODO: handle the purchase information. ie, the supplier product setup pricing/cost
            return new ZohoLineItem()
            {
                item_type = "sales_and_purchases",
                name = variant?.Name ?? product.Name,
                rate = decimal.ToDouble(item.UnitPrice.Value),
                purchase_description = $"{variant?.Name ?? product.Name} from {item.SupplierID}", // debug removal
                description = $"{variant?.Name ?? product.Name} from {item.SupplierID}",
                sku = variant?.ID ?? product.ID, // debug removal
                unit = product.xp?.UnitOfMeasure?.Unit, // debug removal
                purchase_rate = decimal.ToDouble(item.UnitPrice.Value * .5M),
                product_type = "goods", //TODO: MODEL ~ evaluate // debug removal
                is_taxable = true, // product?.xp?.IsResale //TODO: this fails when false unless "tax_exemption_id" is included. Need to figure out what that value is.
                //tax_exemption_id = "",
                //quantity = item.Quantity, // debug removal
                //item_total = decimal.ToDouble(item.LineTotal), // debug removal
                //status = "active", // debug removal
                //line_item_id = item.ID, // debug removal
                //line_id = item.ID, // debug removal
                //pricebook_rate = decimal.ToDouble(item.UnitPrice.Value), // debug removal
                //source = product.DefaultSupplierID, // debug removal
                //item_id = item.ID, //TODO: failed
                //TODO: MODEL ~ Avalara integration evaluation
                //avatax_tax_code = product.xp.Tax.Code,
            };
        }

        public static ZohoLineItem Map(ZohoLineItem zItem, MarketplaceLineItem item, MarketplaceLineItemProduct product, LineItemVariant variant)
        {
            // TODO: handle the purchase information. ie, the supplier product setup pricing/cost
            return new ZohoLineItem()
            {
                item_id = zItem.item_id,
                item_type = "sales_and_purchases",
                name = variant?.Name ?? product.Name,
                rate = decimal.ToDouble(item.UnitPrice.Value),
                purchase_description = $"{variant?.Name ?? product.Name} from {item.SupplierID}", // debug removal
                description = $"{variant?.Name ?? product.Name} from {item.SupplierID}",
                sku = variant?.ID ?? product.ID,
                unit = product.xp?.UnitOfMeasure?.Unit, 
                purchase_rate = decimal.ToDouble(item.UnitPrice.Value * .5M),
                product_type = "goods",
                is_taxable = true, // product?.xp?.IsResale //TODO: this fails when false unless "tax_exemption_id" is included. Need to figure out what that value is.
                //tax_exemption_id = "",
                //item_total = decimal.ToDouble(item.LineTotal), // debug removal
                //status = "active", // debug removal
                //line_item_id = item.ID, // debug removal
                //line_id = item.ID, // debug removal
                //pricebook_rate = decimal.ToDouble(item.UnitPrice.Value), // debug removal
                //source = product.DefaultSupplierID, // debug removal
                //item_id = item.ID, //TODO: failed
                //TODO: MODEL ~ Avalara integration evaluation
                //avatax_tax_code = product.xp.Tax.Code,
            };
        }

        public static List<ZohoLineItem> Map(MarketplaceOrderWorksheet orderWorksheet, ZohoLineItem shipping)
        {
            return orderWorksheet.ShipEstimateResponse.ShipEstimates.Select(shipEstimate => {
                //var choosenProposedShipmentSelection = shipmentEstimate.ShipmentMethods.First(shipmentMethod => shipmentMethod.ID == proposedShipment.SelectedProposedShipmentOptionID);
                var choosenShipMethod = shipEstimate.ShipMethods.First(shipmentMethod => shipmentMethod.ID == shipEstimate.SelectedShipMethodID);
                var supplierIDOfShipment = orderWorksheet.LineItems.First(lineItem => lineItem.ID == shipEstimate.ShipEstimateItems.First().LineItemID);
                return new ZohoLineItem()
                {
                    item_id = shipping.item_id,
                    item_type = "sales_and_purchases",

                    // need to figure out how to set supplier ID here
                    name = $"{shipping.name} for {supplierIDOfShipment}",
                    rate = decimal.ToDouble(choosenShipMethod.Cost),
                    description = $"{shipping.description} for {supplierIDOfShipment}",
                    sku = shipping.sku,
                    quantity = 1
                    //TODO: MODEL ~ Avalara integration evaluation
                    //avatax_tax_code = "FR"
                };
            }).ToList();
        }
    }

    public static class ZohoPurchaseOrderMapper
    {
        public static ZohoPurchaseOrder Map(ZohoSalesOrder salesorder, Order order, List<ZohoLineItem> items, ListPage<MarketplaceLineItem> lineitems, ZohoAddress delivery_address, ZohoContact vendor) {
            var po = new ZohoPurchaseOrder()
            {
                //delivery_address = delivery_address,
                line_items = items.Select(p => new ZohoLineItem()
                {
                    account_id = p.purchase_account_id,
                    item_id = p.item_id,
                    description = p.description,
                    rate = decimal.ToDouble(lineitems.Items.FirstOrDefault(l => l.ProductID == p.sku).UnitPrice.Value),
                    quantity = lineitems.Items.FirstOrDefault(l => l.ProductID == p.sku)?.Quantity
                }).ToList(),
                salesorder_id = salesorder.salesorder_id,
                purchaseorder_number = order.ID,
                reference_number = salesorder.reference_number,
                sub_total = decimal.ToDouble(order.Subtotal),
                tax_total = decimal.ToDouble(order.TaxCost),
                total = decimal.ToDouble(order.Total),
                //vendor_name = vendor.contact_name,
                vendor_id = vendor.contact_id,
                //contact_persons = vendor.contact_persons != null ? new List<string>() { vendor.contact_persons.FirstOrDefault()?.contact_person_id } : null,
                delivery_customer_id = salesorder.customer_id
            };
            return po;
        }
    }

    public static class ZohoSalesOrderMapper
    {
        public static ZohoSalesOrder Map(MarketplaceOrder order, List<ZohoLineItem> items, ZohoContact contact, IList<MarketplaceLineItem> lineitems)
        {
            return new ZohoSalesOrder()
            {
                reference_number = order.ID,
                salesorder_number = order.ID,
                date = order.DateSubmitted?.ToString("yyyy-MM-dd"),
                line_items = items.Select(item =>
                {
                    if (item.sku == "shipping")
                    {
                        return new ZohoLineItem()
                        {
                            item_id = item.item_id,
                            quantity = 1,
                            rate = item.rate
                        };
                    }
                    return new ZohoLineItem()
                    {
                        item_id = item.item_id,
                        quantity = lineitems.FirstOrDefault(li => li.ProductID == item.sku)?.Quantity,
                        rate = decimal.ToDouble(lineitems.FirstOrDefault(li => li.ProductID == item.sku).UnitPrice.Value)
                    };
                }).ToList(),
                tax_total = decimal.ToDouble(order.TaxCost),
                customer_name = contact.contact_name,
                sub_total = decimal.ToDouble(order.Subtotal),
                total = decimal.ToDouble(order.Total),
                //TODO: consult with Oliver on taxes
                customer_id = contact.contact_id,
                currency_code = contact.currency_code,
                currency_symbol = contact.currency_symbol,
                // same error as billing address
                //shipping_address = new ZohoAddress() {
                //    attention = $"{lineitems.FirstOrDefault()?.ShippingAddress.CompanyName}: {lineitems.FirstOrDefault()?.ShippingAddress.FirstName} {lineitems.FirstOrDefault()?.ShippingAddress.LastName}",
                //    address = lineitems.FirstOrDefault()?.ShippingAddress.Street1,
                //    street2 = lineitems.FirstOrDefault()?.ShippingAddress.Street2,
                //    city = lineitems.FirstOrDefault()?.ShippingAddress.City,
                //    state_code = lineitems.FirstOrDefault()?.ShippingAddress.State,
                //    zip = lineitems.FirstOrDefault()?.ShippingAddress.Zip,
                //    phone = lineitems.FirstOrDefault()?.ShippingAddress.Phone
                //},
                // weird Zoho error about billing address being more than 100 characters
                //billing_address = new ZohoAddress()
                //{
                //    address = order.BillingAddress.Street1,
                //    street2 = order.BillingAddress.Street2,
                //    city = order.BillingAddress.City,
                //    state_code = order.BillingAddress.State,
                //    zip = order.BillingAddress.Zip,
                //    attention = $"{order.BillingAddress.CompanyName}: {order.FromUser.FirstName} {order.FromUser.LastName}"
                //}
                //contact_persons = new List<string>() { person.contact_person_id },
                //shipping_charge = decimal.ToDouble(order.ShippingCost), //TODO: Please mention any Shipping/miscellaneous charges as additional line items.
            };
        }
    }

    public static class ZohoAddressMapper
    {
        public static ZohoAddress Map(MarketplaceAddressSupplier address)
        {
            return new ZohoAddress()
            {
                attention = address.CompanyName,
                address = address.Street1,
                street2 = address.Street2,
                city = address.City,
                state = address.State,
                zip = address.Zip,
                country = address.Country,
                phone = address.Phone,
                state_code = address.State
            };
        }
        
        public static ZohoAddress Map(MarketplaceAddressBuyer address)
        {
            return new ZohoAddress()
            {
                attention = address.CompanyName,
                address = address.Street1,
                street2 = address.Street2,
                city = address.City,
                state = address.State,
                zip = address.Zip,
                country = address.Country,
                phone = address.Phone,
                state_code = address.State
            };
        }
    }

    //public static class ZohoMapper
    //{
    //    public static ZohoContactPerson Map(MarketplaceUser user, ZohoContact contact)
    //    {
    //        return new ZohoContactPerson()
    //        {
    //            //contact_id = contact.contact_id,
    //            //contact_person_id = user.ID,
    //            email = user.Email,
    //            first_name = user.FirstName,
    //            last_name = user.LastName,
    //            is_primary_contact = false,
    //            phone = user.Phone
    //        };
    //    }

    //    

    

    //    public static ZohoAddress Map(MarketplaceAddress address)
    //    {
    //        return new ZohoAddress()
    //        {
    //            address = address.Street1,
    //            street2 = address.Street1,
    //            city = address.City,
    //            state = address.State,
    //            state_code = address.State,
    //            zip = address.Zip,
    //            country = address.Country,
    //            phone = address.Phone,
    //        };
    //    }
    //}
}
