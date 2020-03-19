﻿using System.ComponentModel.DataAnnotations;

namespace Marketplace.Models.Extended
{
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