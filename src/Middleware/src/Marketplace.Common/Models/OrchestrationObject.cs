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
        [Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string ID { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
    }
}
