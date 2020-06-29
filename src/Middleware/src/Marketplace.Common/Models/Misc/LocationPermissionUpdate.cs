﻿using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models.Misc
{
    public class LocationPermissionUpdate
    {
        public List<UserGroupAssignment> AssignmentsToAdd { get; set; }
        public List<UserGroupAssignment> AssignmentsToDelete { get; set; }
    }

    public class LocationApprovalThresholdUpdate
    {
        public decimal Threshold { get; set; }
    }
}

