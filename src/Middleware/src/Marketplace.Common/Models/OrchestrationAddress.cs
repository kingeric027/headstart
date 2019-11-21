using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationAddress : OrchestrationObject
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public string Country { get; set; }
        public string Phone { get; set; }
        public string AddressName { get; set; }
        public OrchestrationAddressXp xp { get; set; } = new OrchestrationAddressXp();
    }

    public class OrchestrationAddressXp
    {

    }
}
