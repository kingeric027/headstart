using System;
using System.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Models
{
    [DocIgnore]
    public class WorkItem
    {
        public WorkItem() { }
        public WorkItem(string path)
        {
            var split = path.Split("/");
            this.ResourceId = split[0];
            this.RecordId = split[split.Length - 1].Replace(".json", "");
            switch (split[2])
            {
                case "product":
                    this.RecordType = RecordType.Product;
                    break;
                case "priceschedule":
                    this.RecordType = RecordType.PriceSchedule;
                    break;
                case "productfacet":
                    this.RecordType = RecordType.ProductFacet;
                    break;
                case "specproductassignment":
                    this.RecordType = RecordType.SpecProductAssignment;
                    break;
                case "specoption":
                    this.RecordType = RecordType.SpecOption;
                    break;
                case "spec":
                    this.RecordType = RecordType.Spec;
                    break;
                case "buyer":
                    this.RecordType = RecordType.Buyer;
                    break;
                case "user":
                    this.RecordType = RecordType.User;
                    break;
                case "usergroup":
                    this.RecordType = RecordType.UserGroup;
                    break;
                case "address":
                    this.RecordType = RecordType.Address;
                    break;
                case "usergroupassignment":
                    this.RecordType = RecordType.UserGroupAssignment;
                    break;
                case "addressassignment":
                    this.RecordType = RecordType.AddressAssignment;
                    break;
                case "costcenter":
                    this.RecordType = RecordType.CostCenter;
                    break;
                case "catalogassignment":
                    this.RecordType = RecordType.CatalogAssignment;
                    break;
                case "catalog":
                    this.RecordType = RecordType.Catalog;
                    break;
                default:
                    throw new OrchestrationException(OrchestrationErrorType.WorkItemDefinition, path);
            }
        }
        public string ResourceId { get; set; }
        public string RecordId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RecordType RecordType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Action Action { get; set; }
        public JObject Current { get; set; } // not used for delete
        public JObject Cache { get; set; } // not used for create
        public JToken Diff { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecordType
    {
        Product, PriceSchedule, Spec, SpecOption, SpecProductAssignment, ProductFacet,
        Buyer, User, UserGroup, Address, CostCenter, UserGroupAssignment, AddressAssignment, 
        CatalogAssignment, Catalog
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Action { Ignore, Create, Update, Patch, Delete, Get }

    public static class WorkItemMethods
    {
        public static Action DetermineAction(WorkItem wi)
        {
            // we want to ensure a condition is met before determining an action
            // so that if there is a case not compensated for it flows to an exception
            try
            {
                // first check if there is a cache object, if not it's a CREATE event
                if (wi.Cache == null)
                    return Action.Create;

                // if cache does exists, and there is no difference ignore the action
                if (wi.Cache != null && wi.Diff == null)
                    return (wi.RecordType == RecordType.SpecProductAssignment || wi.RecordType == RecordType.UserGroupAssignment || wi.RecordType == RecordType.CatalogAssignment)
                        ? Action.Ignore
                        : Action.Get;

                // special case for SpecAssignment because there is no ID for the OC object
                // but we want one in orchestration to handle caching
                // in further retrospect I don't think we need to handle updating objects when only the ID is being changed
                // maybe in the future a true business case will be necessary to do this
                if ((wi.RecordType == RecordType.SpecProductAssignment || wi.RecordType == RecordType.UserGroupAssignment || wi.RecordType == RecordType.CatalogAssignment) 
                    && wi.Diff.Children().Count() == 1 && wi.Diff.SelectToken("ID").Path == "ID")
                    return Action.Ignore;

                if (wi.Cache != null && wi.Diff != null)
                {
                    // cache exists, we want to force a PUT when xp has deleted properties because 
                    // it's the only way to delete the properties
                    // otherwise we want to PATCH the existing object
                    return wi.Cache.HasDeletedXp(wi.Current) ? Action.Update : Action.Patch;
                }
            }
            catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.ActionEvaluationError, wi, ex.Message);
            }

            throw new OrchestrationException(OrchestrationErrorType.ActionEvaluationError, wi, "Unable to determine action");
        }
    }
}
