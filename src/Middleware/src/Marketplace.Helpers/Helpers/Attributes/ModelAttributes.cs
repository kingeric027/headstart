using System;

namespace Marketplace.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchableAttribute : Attribute
    {
        public int Priority { get; set; }

        public SearchableAttribute(int priority)
        {
            Priority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SortableAttribute : Attribute
    {
        public int? Priority { get; set; }
        public bool Descending { get; set; }

        public SortableAttribute() { }

        public SortableAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
