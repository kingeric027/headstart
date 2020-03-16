﻿using System;
using Marketplace.Common.Commands;
using Marketplace.Common.Exceptions;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Marketplace.Models.Exceptions;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using Action = Marketplace.Common.Models.Action;

namespace Marketplace.Common.Models
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
            this.RecordId = wi.RecordId;
            this.ResourceId = wi.ResourceId;
            this.RecordType = wi.RecordType;
            this.Level = LogLevel.Warn;
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
        public string ResourceId { get; set; }
        [Sortable]
        public string RecordId { get; set; }
        [Sortable]
        public RecordType? RecordType { get; set; }
        [Sortable]
        public Action? Action { get; set; }
        [DocIgnore]
        public JObject Current { get; set; }
        [DocIgnore]
        public JObject Cache { get; set; }
    }
}
