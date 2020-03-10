using System;

namespace Marketplace.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class DocSection : Attribute
    {
        public string ID { get; }
        public int ListOrder { get; set; }

        protected DocSection()
        {
            ID = GetType().Name.Replace("Attribute", "");
            ListOrder = int.MaxValue;
        }
    }

    public class DocType : Attribute
    {
        public string TypeName { get; protected set; }

        private DocType()
        {
            TypeName = GetType().Name.Replace("Attribute", "").ToLower();
        }

        public class StringAttribute : DocType { }
        public class IntegerAttribute : DocType { }
        public class DateAttribute : DocType { }
        public class ArrayAttribute : DocType { }
        public class ObjectAttribute : DocType { }
        public class NoneAttribute : DocType
        {
            public NoneAttribute()
            {
                TypeName = "";
            }
        }
    }

    /// <summary>
    /// Use to override the default test used for resources (i.e. "Cost Centers") and endpoints (i.e. "Get a Single Cost Center").
    /// Apply at the controller or action level, respectively.
    /// </summary>
    public class DocNameAttribute : Attribute
    {
        public DocNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    /// <summary>
    /// Use to decorate controllers, actions, action parameters, models, and model properties.
    /// Supports markdown. Pass multiple strings for multiple paragraphs.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class DocCommentsAttribute : Attribute
    {
        public DocCommentsAttribute(params string[] comments)
        {
            Comments = comments;
        }

        public string[] Comments { get; private set; }
    }

    /// <summary>
    /// Allows MeController to be documented as multiple resources. example: Me.ListAddresses is documented under UserPerspective/Addresses
    /// </summary>
    public class DocSubResourceAttribute : Attribute
    {
        public DocSubResourceAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    /// <summary>
    /// Use to indicate that the property can't be set through the API. Use for things like OrderStatus and DateCreated.
    /// (Can't just make property read-only because we need to set it when mapping from business object.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ApiReadOnlyAttribute : Attribute { }

    ///// <summary>
    ///// Use to indicate that the property can't be read through the API. Rarely used, passwords and credit card #s might be all.
    ///// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ApiWriteOnlyAttribute : Attribute { }


    /// <summary>
    /// Use on model properties and action params to document sample data
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DocSampleDataAttribute : Attribute
    {
        public DocSampleDataAttribute(object value)
        {
            Value = value;
        }

        public object Value { get; private set; }
    }

    /// <summary>
    /// Apply to anything you want the doc generator to exclude
    /// </summary>
    public class DocIgnoreAttribute : Attribute
    {
    }
}
