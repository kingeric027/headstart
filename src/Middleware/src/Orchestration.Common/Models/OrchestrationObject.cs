using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Orchestration.Common.Models
{
    public interface IOrchestrationObject
    {
        string ID { get; set; }
    }
}
