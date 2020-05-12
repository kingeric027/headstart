﻿using System.ComponentModel.DataAnnotations;
using ordercloud.integrations.openapispec;

namespace Marketplace.Models.Extended
{
	[SwaggerModel]
    public class QuoteOrderInfo
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [MaxLength(200, ErrorMessage = "Quote request comments cannot exceed 200 characters")]
        public string Comments { get; set; }
    }
}
