using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marketplace.Common.Models
{
    class SendGridModels
    {
        public class EmailTemplate
        {
            public object Data { get; set; }
            public EmailDisplayText Message { get; set; }
        }

        public class ProductInfo
        {
            public string ProductName { get; set; }
            public string ImageURL { get; set; }
            public string ProductID { get; set; }
            public int Quantity { get; set; }
            public decimal LineTotal { get; set; }
            public string SpecCombo { get; set; }
        }

        public class OrderTemplateData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string OrderID { get; set; }
            public string DateSubmitted { get; set; }
            public string ShippingAddressID { get; set; }
            public Address ShippingAddress { get; set; }
            public string BillingAddressID { get; set; }
            public Address BillingAddress { get; set; }
            public Address BillTo { get; set; }
            public IEnumerable<ProductInfo> Products { get; set; }
            public decimal Subtotal { get; set; }
            public decimal TaxCost { get; set; }
            public decimal ShippingCost { get; set; }
            public decimal PromotionalDiscount { get; set; }
            public decimal Total { get; set; }
            public  string Currency { get; set; }
        }

        public class QuoteOrderTemplateData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Location { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public HSOrder Order { get; set; }
        }

    }
}
