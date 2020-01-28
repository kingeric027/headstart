﻿
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OrderCloud.SDK;

namespace Marketplace.Common.Models.CardConnect
{
    public class CreditCardToken
    {
        public string AccountNumber { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [MinLength(4, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        [MaxLength(6, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        public string ExpirationDate { get; set; }
        public string CardholderName { get; set; }
        public string CardType { get; set; }
    }

    public class CreditCardPayment
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string OrderID { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderDirection Direction { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public double Amount { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string CreditCardID { get; set; }
        [MinLength(3, ErrorMessage = "Invalid currency specified: Must be 3 digit code. Ex: USD or CAD")]
        [MaxLength(3, ErrorMessage = "Invalid currency specified: Must be 3 digit code. Ex: USD or CAD")]
        public string Currency { get; set; }
        [MinLength(3, ErrorMessage = "Invalid CVV: Must be 3 or 4 digit code.")]
        [MaxLength(4, ErrorMessage = "Invalid CVV: Must be 3 or 4 digit code.")]
        public string CVV { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string MerchantID { get; set; }

    }
}
