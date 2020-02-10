using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Common.Models;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers.Zoho
{
    public static class ZohoMapper
    {
        public static ZohoContact Map(Buyer buyer)
        {
            return new ZohoContact()
            {
                company_name = buyer.Name,
                contact_name = buyer.Name
            };
        }

        public static ZohoContactPerson Map(User user, ZohoContact contact)
        {
            return new ZohoContactPerson()
            {
                contact_id = contact.contact_id,
                contact_person_id = user.ID,
                email = user.Email,
                first_name = user.FirstName,
                last_name = user.LastName,
                is_primary_contact = false,
                phone = user.Phone
            };
        }

        public static ZohoLineItem Map(Order order, LineItem item, Product product)
        {
            return new ZohoLineItem()
            {
                item_type = "sales_and_purchases",
                status = "active",
                name = product.Name,
                rate = decimal.ToDouble(item.UnitPrice.Value),
                purchase_description = $"{product.Name} from {item.SupplierID}",
                sku = product.ID,
                quantity = item.Quantity,
                unit = MapperHelper.TryGetXp(product.xp, "UnitOfMeasure.Unit"),
                //description = product.Description, //TODO: invalid value passed for description
                //item_id = item.ID, //TODO: failed 
                item_total = decimal.ToDouble(item.LineTotal),
                line_item_id = item.ID,
                line_id = item.ID,
                pricebook_rate = decimal.ToDouble(item.UnitPrice.Value),
                tax_authority_id = MapperHelper.TryGetXp(product.xp, "Tax.Code"),
                tax_id = MapperHelper.TryGetXp(product.xp, "Tax.Code"),
                tax_name = MapperHelper.TryGetXp(product.xp, "Tax.Category"),
                source = product.DefaultSupplierID
            };
        }

        public static ZohoSalesorder Map(Order order, ZohoContact contact, List<ZohoLineItem> items, ZohoContactPerson person, Buyer buyer)
        {
            return new ZohoSalesorder()
            {
                customer_id = contact.contact_id,
                date = order.DateSubmitted?.ToString("yyyy-MM-dd"),
                line_items = items,
                tax_total = decimal.ToDouble(order.TaxCost),
                reference_number = order.ID,
                contact_persons = new List<string>() { person.contact_person_id },
                //billing_address_id = order.BillingAddress.ID, //TODO: invalid billing address id
                currency_code = "USD", //TODO: handle currency when properly modeled
                currency_symbol = "USD",
                customer_name = buyer.Name,
                salesorder_id = order.ID,
                salesperson_name = $"{person.first_name} {person.last_name}",
                shipping_charge = decimal.ToDouble(order.ShippingCost),
                sub_total = decimal.ToDouble(order.Subtotal),
                taxes = new List<ZohoTax>() //TODO: consult with Oliver on taxes
                {
                    new ZohoTax()
                    {
                        tax_id = MapperHelper.TryGetXp(order.xp, "AvalaraTaxTransactionCode"),
                        tax_amount = decimal.ToDouble(order.TaxCost)
                    }
                },
                total = decimal.ToDouble(order.Total)
            };
        }

        //public static ZohoLineItem Map(Order order)
        //{
        //    var list = new List<ZohoLineItem>();
        //    var mk = (MarketplaceOrder)order;
        //    mk.xp.ProposedShipmentSelections
        //}
    }
}
