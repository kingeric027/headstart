using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationUser : OrchestrationObject
    {
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime TermsAccepted { get; set; }
        [Required]
        public bool Active { get; set; }
        public OrchestrationUserXp xp { get; set; } = new OrchestrationUserXp();
    }

    public class OrchestrationUserXp
    {
    }
}
