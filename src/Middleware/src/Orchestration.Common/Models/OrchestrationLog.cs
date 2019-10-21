﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using Orchestration.Common.Exceptions;
using Orchestration.Common.Helpers;

namespace Orchestration.Common.Models
{
    public class OrchestrationLog : ICosmosObject
    {
        public OrchestrationLog()
        {
        }

        public OrchestrationLog(WorkItem wi)
        {
            this.Action = wi.Action;
            this.Current = wi.Current;
            this.Cache = wi.Cache;
            this.Diff = wi.Diff;
            this.RecordId = wi.RecordId;
            this.SupplierId = wi.SupplierId;
            this.RecordType = wi.RecordType;
            this.Level = LogLevel.Success;
        }

        public OrchestrationLog(OrchestrationException ex)
        {

        }

        public OrchestrationLog(FunctionFailedException ex)
        {

        }

        public OrchestrationLog(Exception ex)
        {

        }

        public string id { get; set; }
        [Sortable]
        public DateTimeOffset timeStamp { get; set; }
        [Sortable]
        public OrchestrationErrorType? ErrorType { get; set; }
        public string Message { get; set; }
        [Sortable]
        public LogLevel Level { get; set; }
        [Sortable]
        public string SupplierId { get; set; }
        [Sortable]
        public string RecordId { get; set; }
        [Sortable]
        public RecordType? RecordType { get; set; }
        [Sortable]
        public Action? Action { get; set; }
        public JObject Current { get; set; }
        public JObject Cache { get; set; }
        public JObject Diff { get; set; }
    }
}
