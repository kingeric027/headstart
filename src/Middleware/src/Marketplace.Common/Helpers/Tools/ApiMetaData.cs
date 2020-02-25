using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Marketplace.Helpers.Extensions;

namespace Marketplace.Common.Helpers.Tools
{
    public class ApiMetaData
    {
        public IList<ApiSection> Sections { get; set; }
        public IEnumerable<ApiResource> Resources => Sections.SelectMany(a => a.Resources).OrderBy(r => r.Name);
        public IList<ApiModel> Models { get; set; }
        public IList<ApiEnum> Enums { get; set; }
        public IList<ErrorCode> ErrorCodes { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class ApiSection
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IList<string> Description { get; set; }
        public IList<ApiResource> Resources { get; set; }
    }

    public class ApiResource
    {
        public Type ControllerType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<ApiEndpoint> Endpoints { get; set; }
        public IList<string> Comments { get; set; }
    }

    public class ApiEndpoint
    {
        public MethodInfo MethodInfo { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Used by docs (but not SDKs) for "Me", so ListCostCenters is under "Cost Centers" sub-resource, for example
        /// </summary>
        public string SubResource { get; set; }
        public string Description { get; set; }
        public System.Net.Http.HttpMethod HttpVerb { get; set; }
        public string Route { get; set; }
        public IList<ApiParameter> PathArgs { get; set; }
        public IList<ApiParameter> QueryArgs { get; set; }
        public ApiModel RequestModel { get; set; }
        public ApiModel ResponseModel { get; set; }
        public int HttpStatus { get; set; }
        public IList<string> RequiredRoles { get; set; }
        public IList<string> Comments { get; set; }
        public bool IsList { get; set; }
        public bool HasListArgs { get; set; }
    }

    public abstract class ApiField
    {
        public abstract Type Type { get; }
        public string Name { get; set; }
        public string SimpleType { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public abstract bool HasDefaultValue { get; }
        public abstract object DefaultValue { get; }
    }

    public class ApiParameter : ApiField
    {
        public ParameterInfo ParamInfo { get; set; }
        public bool IsListArg { get; set; }
        public override Type Type => ParamInfo.ParameterType;
        public override bool HasDefaultValue => ParamInfo.HasDefaultValue;
        public override object DefaultValue => ParamInfo.DefaultValue;
    }

    public class ApiProperty : ApiField
    {
        public PropertyInfo PropInfo { get; set; }
        public override Type Type => PropInfo.PropertyType;
        public override bool HasDefaultValue => PropInfo.HasAttribute<DefaultValueAttribute>();
        public override object DefaultValue => PropInfo.GetAttribute<DefaultValueAttribute>()?.Value;
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public object SampleData { get; set; }
    }

    public class ApiModel
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// if this is a list, for example, get the model that it's a list of
        /// </summary>
        public ApiModel InnerModel { get; set; }
        public IList<ApiProperty> Properties { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsWriteOnly { get; set; }
        public bool IsPartial { get; set; }
        public object Sample { get; set; }
    }

    public class ApiEnum
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Description { get; set; }
        public IList<ApiEnumValue> Values { get; set; }
    }

    public class ApiEnumValue
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ErrorCode
    {
        public string FullCode { get; set; }
        public string Category => FullCode.Split('.')[0];
        public string Name => FullCode.Split('.')[1];
        public string Description { get; set; }
    }
}
