using Marketplace.Helpers.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Helpers.Helpers.Attributes
{
	public static class InteropID
	{
		public const string VALIDATION_REGEX = @"^[a-zA-Z0-9-_{}]+$";

		public static string New()
		{
			return Encode(Guid.NewGuid());
		}

		/// <summary>
		/// returns the passed in ID or a newly generated one if null or whitespace.
		/// </summary>
		public static string GetOrNew(string id)
		{
			return string.IsNullOrEmpty(id) ? New() : id;
		}

		// http://madskristensen.net/post/a-shorter-and-url-friendly-guid
		private static string Encode(Guid guid)
		{
			string enc = Convert.ToBase64String(guid.ToByteArray());
			enc = enc.Replace("/", "_");
			enc = enc.Replace("+", "-");
			return enc.Substring(0, 22);
		}

		// in case we ever need to decode back to a guid
		// http://madskristensen.net/post/a-shorter-and-url-friendly-guid
		private static Guid Decode(string encoded)
		{
			encoded = encoded.Replace("_", "/");
			encoded = encoded.Replace("-", "+");
			byte[] buffer = Convert.FromBase64String(encoded + "==");
			return new Guid(buffer);
		}
	}

	public class InteropIDAttribute : RegularExpressionAttribute
	{
		public InteropIDAttribute() : base(InteropID.VALIDATION_REGEX)
		{
			AutoGen = true;
		}

		/// <summary>
		/// generate if not set. defaults to true
		/// </summary>
		public bool AutoGen { get; set; }

		public override bool IsValid(object value)
		{
			var s = value as string;
			// ID can be null, that triggers us to generate one
			return string.IsNullOrEmpty(s) || (s.Length <= 100 && base.IsValid(value));
		}

		protected override ValidationResult IsValid(object value, ValidationContext ctx)
		{
			// if model is sent with a null or empty ID, generate one
			if (string.IsNullOrEmpty(value as string) && AutoGen)
			{
				var partial = ctx.ObjectInstance as IPartial;
				if (partial != null)
				{
					partial.Values[ctx.MemberName] = JToken.FromObject(InteropID.New());
				}
				else
				{
					var idProp = ctx.ObjectType.GetProperty(ctx.MemberName);
					idProp.SetValue(ctx.ObjectInstance, InteropID.New());
				}
				return null;
			}
			return base.IsValid(value, ctx);
		}

		public override string FormatErrorMessage(string name)
		{
			return $"{name} can only contain characters Aa-Zz 0-9 - _";
		}
	}
}
