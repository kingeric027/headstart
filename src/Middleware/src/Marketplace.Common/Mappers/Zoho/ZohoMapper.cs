using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Models;
using Microsoft.EntityFrameworkCore.Internal;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers.Zoho
{
    public static class ZohoContactMapper
    {
        public static ZohoContact Map(MarketplaceBuyer buyer, MarketplaceUser user, MarketplaceUserGroup group, ZohoCurrency currency, MarketplaceAddress address)
        {
            return new ZohoContact()
            {
                company_name = buyer.ID,
                contact_name = buyer.Name,
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

        public static ZohoContact Map(ZohoContact contact, MarketplaceBuyer buyer, MarketplaceUser user, MarketplaceUserGroup group,
            ZohoCurrency currency, MarketplaceAddress address)
        {
            contact.company_name = buyer.ID;
            contact.contact_name = buyer.Name;
            contact.billing_address = ZohoAddressMapper.Map(address);
            contact.shipping_address = ZohoAddressMapper.Map(address);
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
        public static ZohoLineItem Map(LineItem item, MarketplaceProduct product)
        {
            // TODO: handle the purchase information. ie, the supplier product setup pricing/cost
            return new ZohoLineItem()
            {
                item_type = "sales_and_purchases",
                name = product.Name,
                rate = decimal.ToDouble(item.UnitPrice.Value),
                purchase_description = $"{product.Name} from {item.SupplierID}", // debug removal
                description = $"{product.Name} from {item.SupplierID}",
                sku = product.ID, // debug removal
                unit = product.xp.UnitOfMeasure.Unit, // debug removal
                purchase_rate = decimal.ToDouble(item.UnitPrice.Value * .5M),
                product_type = "goods", //TODO: MODEL ~ evaluate // debug removal
                is_taxable = true //TODO: MODEL ~ evaluate with Tax authority
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

        public static ZohoLineItem Map(ZohoLineItem zItem, LineItem item, MarketplaceProduct product)
        {
            // TODO: handle the purchase information. ie, the supplier product setup pricing/cost
            return new ZohoLineItem()
            {
                item_id = zItem.item_id,
                item_type = "sales_and_purchases",
                name = product.Name,
                rate = decimal.ToDouble(item.UnitPrice.Value),
                purchase_description = $"{product.Name} from {item.SupplierID}", // debug removal
                description = $"{product.Name} from {item.SupplierID}",
                sku = product.ID, // debug removal
                unit = product.xp.UnitOfMeasure.Unit, // debug removal
                purchase_rate = decimal.ToDouble(item.UnitPrice.Value * .5M),
                product_type = "goods", //TODO: MODEL ~ evaluate // debug removal
                is_taxable = true //TODO: MODEL ~ evaluate with Tax authority
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

        public static List<ZohoLineItem> Map(MarketplaceOrder order, ZohoLineItem shipping)
        {
            return order.xp.ProposedShipmentSelections.Select(s => new ZohoLineItem()
            {
                item_id = shipping.item_id,
                item_type = "sales_and_purchases",
                name = $"{shipping.name} for {s.SupplierID}",
                rate = decimal.ToDouble(s.Rate),
                description = $"{shipping.description} for {s.SupplierID}",
                sku = shipping.sku,
                quantity = 1
                //TODO: MODEL ~ Avalara integration evaluation
                //avatax_tax_code = "FR"
            }).ToList();
        }
    }

    public static class ZohoSalesOrderMapper
    {
        public static ZohoSalesOrder Map(MarketplaceOrder order, List<ZohoLineItem> items, ZohoContact contact, ListPage<LineItem> lineitems)
        {
            return new ZohoSalesOrder()
            {
                reference_number = order.ID,
                salesorder_id = order.ID, // expecting this to fail
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
                        quantity = lineitems.Items.FirstOrDefault(li => li.ProductID == item.sku)?.Quantity,
                        rate = decimal.ToDouble(lineitems.Items.FirstOrDefault(li => li.ProductID == item.sku).UnitPrice.Value)
                    };
                }).ToList(),
                tax_total = decimal.ToDouble(order.TaxCost),
                customer_name = contact.contact_name,
                sub_total = decimal.ToDouble(order.Subtotal),
                total = decimal.ToDouble(order.Total),
                taxes = new List<ZohoTax>() //TODO: consult with Oliver on taxes
                    {
                        new ZohoTax()
                        {
                            tax_id = order.xp.AvalaraTaxTransactionCode,
                            tax_amount = decimal.ToDouble(order.TaxCost)
                        }
                    },
                customer_id = contact.contact_id,
                currency_code = contact.currency_code,
                currency_symbol = contact.currency_symbol
                //contact_persons = new List<string>() { person.contact_person_id },
                //billing_address_id = order.BillingAddress.ID, //TODO: invalid billing address id
                //salesperson_name = $"{person.first_name} {person.last_name}", //TODO: do we have default users for our Sellers?
                //shipping_charge = decimal.ToDouble(order.ShippingCost), //TODO: Please mention any Shipping/miscellaneous charges as additional line items.
            };
        }
    }

    public static class ZohoAddressMapper
    {
        public static ZohoAddress Map(MarketplaceAddress address)
        {
            return new ZohoAddress()
            {
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
