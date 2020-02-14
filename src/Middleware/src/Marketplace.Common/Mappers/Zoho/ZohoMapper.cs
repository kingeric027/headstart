using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Models;
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
                //billing_address = ZohoAddressMapper.Map(address),
                //shipping_address = ZohoAddressMapper.Map(address),
                //contact_persons = new List<ZohoContactPerson>()
                //{
                //    new ZohoContactPerson()
                //    {
                //        email = user.Email,
                //        first_name = user.FirstName,
                //        last_name = user.LastName,
                //        phone = user.Phone,
                //        is_primary_contact = true
                //    }
                //},
                //currency_id = currency.currency_id
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

    //    public static ZohoLineItem Map(MarketplaceOrder order, LineItem item, MarketplaceProduct product)
    //    {
    //        // TODO: handle the purchase information. ie, the supplier product setup pricing/cost
    //        return new ZohoLineItem()
    //        {
    //            item_type = "sales_and_purchases",
    //            status = "active",
    //            name = product.Name,
    //            rate = decimal.ToDouble(item.UnitPrice.Value),
    //            purchase_description = $"{product.Name} from {item.SupplierID}",
    //            sku = product.ID,
    //            quantity = item.Quantity,
    //            unit = product.xp.UnitOfMeasure.Unit,
    //            //description = product.Description, //TODO: invalid value passed for description
    //            //item_id = item.ID, //TODO: failed 
    //            item_total = decimal.ToDouble(item.LineTotal),
    //            line_item_id = item.ID,
    //            line_id = item.ID,
    //            pricebook_rate = decimal.ToDouble(item.UnitPrice.Value),
    //            avatax_tax_code = product.xp.Tax.Code,
    //            source = product.DefaultSupplierID
    //        };
    //    }

    //    public static ZohoSalesorder Map(MarketplaceOrder order, ZohoContact contact, List<ZohoLineItem> items, ZohoContactPerson person, Buyer buyer)
    //    {
    //        return new ZohoSalesorder()
    //        {
    //            //customer_id = contact.contact_id,
    //            date = order.DateSubmitted?.ToString("yyyy-MM-dd"),
    //            line_items = items,
    //            tax_total = decimal.ToDouble(order.TaxCost),
    //            reference_number = order.ID,
    //            //contact_persons = new List<string>() { person.contact_person_id },
    //            //billing_address_id = order.BillingAddress.ID, //TODO: invalid billing address id
    //            currency_code = "USD", //TODO: handle currency when properly modeled
    //            currency_symbol = "USD",
    //            customer_name = buyer.Name,
    //            salesorder_id = order.ID,
    //            //salesperson_name = $"{person.first_name} {person.last_name}", //TODO: do we have default users for our Sellers?
    //            //shipping_charge = decimal.ToDouble(order.ShippingCost), //TODO: Please mention any Shipping/miscellaneous charges as additional line items.
    //            sub_total = decimal.ToDouble(order.Subtotal),
    //            taxes = new List<ZohoTax>() //TODO: consult with Oliver on taxes
    //            {
    //                new ZohoTax()
    //                {
    //                    tax_id = order.xp.AvalaraTaxTransactionCode,
    //                    tax_amount = decimal.ToDouble(order.TaxCost)
    //                }
    //            },
    //            total = decimal.ToDouble(order.Total)
    //        };
    //    }

    //    public static List<ZohoLineItem> Map(MarketplaceOrder order)
    //    {
    //        return order.xp.ProposedShipmentSelections.Select(s => new ZohoLineItem()
    //            {
    //                item_type = "sales_and_purchases",
    //                name = $"Shipment Charge for {s.SupplierID}: {s.ShippingRateID}",
    //                rate = decimal.ToDouble(s.Rate),
    //                description = $"{s.SupplierID}: {s.ShippingRateID}",
    //                purchase_description = "Marketplace Shipment",
    //                sku = "shipping",
    //                quantity = 1,
    //                avatax_tax_code = "FR"
    //            }).ToList();
    //    }

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
