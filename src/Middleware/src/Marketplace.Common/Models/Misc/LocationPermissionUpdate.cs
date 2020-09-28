using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
    public class LocationPermissionUpdate
    {
        public List<UserGroupAssignment> AssignmentsToAdd { get; set; }
        public List<UserGroupAssignment> AssignmentsToDelete { get; set; }
    }

    [SwaggerModel]
    public class LocationApprovalThresholdUpdate
    {
        public decimal Threshold { get; set; }
    }
}


