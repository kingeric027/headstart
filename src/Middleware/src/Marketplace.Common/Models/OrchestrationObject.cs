using System;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public interface IOrchestrationObject
    {
        string ID { get; set; }
        string Token { get; set; }
        string ClientId { get; set; }
    }

    public class OrchestrationObject : IOrchestrationObject
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9-_]*$", ErrorMessage = "IDs must have at least 8 characters and no more than 100, are required and can only contain characters Aa-Zz, 0-9, -, and _")]
        [MaxLength(100, ErrorMessage = "Must be a minimum of 8 and maximum of 100 characters")]
        [MinLength(8, ErrorMessage = "Must be a minimum of 8 and maximum of 100 characters")]
        public string ID { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
    }
}
