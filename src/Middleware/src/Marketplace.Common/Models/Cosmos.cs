using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marketplace.Common.Models
{
    public interface ICosmosObject
    {
        string id { get; set; }
        DateTimeOffset timeStamp { get; set; }
    }

    /// <summary>
    /// Used to identify a class property as the primary key "id" attribute for Cosmos
    /// </summary>
    public class CosmosIdAttribute : Attribute
    {

    }


}
