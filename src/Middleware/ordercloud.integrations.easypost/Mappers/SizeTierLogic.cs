using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ordercloud.integrations.easypost
{
	// Note, this is not an EasyPost-specific concept. 
	// It was originally created by Bill Hickey as a way to get less expensive shipping rates from FreightPop by grouping lineItems. 
	// Its not currently being used anywhere because the concept doesn't fit with the EasyPost API particularly well.
	// But, I'm saving it in case its useful at some point.

	// measured in how many of the product fit in a 22x22x22 box
	[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public enum SizeTier
	{
		// ships alone
		G,

		//2-5
		A,

		// 5-15
		B,

		//15-49
		C,

		//50-99
		D,

		// 100-999
		E,

		// 1000+
		F
	}

	public class Package
	{
		public static readonly decimal FULL_PACKAGE_DIMENSION = 22; // inches
		public decimal PercentFilled { get; set; } = 0;
		public decimal Weight { get; set; } = 0; // lbs 
	}

	public static class SmartPackageMapper
	{
		private static Dictionary<SizeTier, decimal> SIZE_FACTOR_MAP = new Dictionary<SizeTier, decimal>() 
		{
			{ SizeTier.A, .385M }, // 38.5% of a full package
			{ SizeTier.B, .10M },
			{ SizeTier.C, .031M },
			{ SizeTier.D, .0134M },
			{ SizeTier.E, .0018M },
			{ SizeTier.F, .00067M }
		};

		public static List<EasyPostParcel> MapLineItemsIntoPackages(List<LineItem> lineItems)
		{
			var lineItemsThatCanShipTogether = lineItems.Where(li => li.Product.xp.SizeTier != SizeTier.G).OrderBy(lineItem => lineItem.Product.xp.SizeTier);
			var lineItemsThatShipAlone = lineItems.Where(li => li.Product.xp.SizeTier == SizeTier.G);

			var init = new List<Package>() { new Package() { } };

			var parcels = lineItemsThatCanShipTogether
				.SelectMany(lineItem => Enumerable.Repeat(lineItem, lineItem.Quantity))
				.Aggregate(init, (packages, item) =>
				{
					var percentFillToAdd = SIZE_FACTOR_MAP[item.Product.xp.SizeTier];
					var currentPackage = packages.Last();
					if (currentPackage.PercentFilled + percentFillToAdd > 0)
					{
						var newPackage = new Package() { PercentFilled = percentFillToAdd, Weight = item.Product.ShipWeight ?? 0 };
						packages.Add(newPackage);
					} else
					{
						currentPackage.PercentFilled += percentFillToAdd;
						currentPackage.Weight += item.Product.ShipWeight ?? 0;
					}
					return packages;
				});

			var combinationPackages = parcels.Select((package, index) =>
			{
				var demension = (int)Math.Ceiling(package.PercentFilled * Package.FULL_PACKAGE_DIMENSION);
				return new EasyPostParcel()
				{
					weight = (double)package.Weight,
					length = demension,
					width = demension,
					height = demension,
				};
			}).ToList();

			var individualPackages = lineItemsThatShipAlone.Select(li =>
			{
				return new EasyPostParcel()
				{
					weight = (double) li.Product.ShipWeight,
					length = (double) li.Product.ShipLength,
					width = (double) li.Product.ShipWidth,
					height = (double) li.Product.ShipHeight,
				};
			});

			return combinationPackages.Union(individualPackages).ToList();
		}
	}
}
