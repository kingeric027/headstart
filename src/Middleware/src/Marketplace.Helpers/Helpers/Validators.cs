using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Helpers
{
	public class MaxSizeAttribute : StringLengthAttribute
	{
		public MaxSizeAttribute(int length) : base(length) { }

		public override bool IsValid(object value)
		{
			return base.IsValid(value?.ToString());
		}
	}

    public class MinValueAttribute : RangeAttribute
    {
        public MinValueAttribute(int value) : base(value, int.MaxValue) { }
        public MinValueAttribute(double value) : base(value, double.MaxValue) { }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be {this.Minimum} or greater.";
        }
    }

    public class MaxValueAttribute : RangeAttribute
    {
        public MaxValueAttribute(int value) : base(int.MinValue, value) { }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be less than {this.Minimum}.";
        }
    }
}
