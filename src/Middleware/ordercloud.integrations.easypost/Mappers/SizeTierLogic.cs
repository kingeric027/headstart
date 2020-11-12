using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ordercloud.integrations.library.extensions;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
		// This is intended to be a max parcel dimension, but is not being honored because of a percentage bug. see line 77
		public static readonly decimal FULL_PACKAGE_DIMENSION = 11; // inches. 
        public static readonly decimal DEFAULT_WEIGHT = 5;
		public decimal PercentFilled { get; set; } = 0;
		public decimal Weight { get; set; } = 0; // lbs 
	}

	public static class SmartPackageMapper
	{
		// Any parcel sent to easy post for a fedex account with a dimension over 33 returns no rates.
		private static readonly int FEDEX_MAX_PARCEL_DIMENSION = 33; // inches. 

		private static readonly Dictionary<SizeTier, decimal> SIZE_FACTOR_MAP = new Dictionary<SizeTier, decimal>() 
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

			var packages = lineItemsThatCanShipTogether
				.SelectMany(lineItem => Enumerable.Repeat(lineItem, lineItem.Quantity))
				.Aggregate(new List<Package>(), (packagesInProgress, item) =>
				{
					if (packagesInProgress.Count == 0) packagesInProgress.Add(new Package());
					var percentFillToAdd = SIZE_FACTOR_MAP[item.Product.xp.SizeTier];
					var currentPackage = packagesInProgress.Last();
					if (currentPackage.PercentFilled + percentFillToAdd > 100) // this should be a 1, not a 100. However, this mathematically correct packaging produces very high rates.
					{
						var newPackage = new Package() { PercentFilled = percentFillToAdd, Weight = item.Product.ShipWeight ?? 0 };
						packagesInProgress.Add(newPackage);
					} else
					{
						currentPackage.PercentFilled += percentFillToAdd;
						currentPackage.Weight += item.Product.ShipWeight ?? 0;
					}
					return packagesInProgress;
				});

			var combinationPackages = packages.Select((package, index) =>
			{
				var dimension = (int)Math.Ceiling(package.PercentFilled * Package.FULL_PACKAGE_DIMENSION);
				return new EasyPostParcel()
				{
					weight = (double)package.Weight,
					length = dimension,
					width = dimension,
					height = dimension,
				};
			}).ToList();

			var individualPackages = lineItemsThatShipAlone.Select(li => new EasyPostParcel()
            {
                // length/width/height cannot be zero otherwise we'll get an error (422 Unprocessable Entity) from easypost
                weight = (double) (li.Product.ShipWeight ?? Package.DEFAULT_WEIGHT),
                length = (double) (li.Product.ShipLength.IsNullOrZero() ? Package.FULL_PACKAGE_DIMENSION : li.Product.ShipLength),
                width = (double) (li.Product.ShipWidth.IsNullOrZero() ? Package.FULL_PACKAGE_DIMENSION : li.Product.ShipWidth),
                height = (double) (li.Product.ShipHeight.IsNullOrZero() ? Package.FULL_PACKAGE_DIMENSION : li.Product.ShipHeight),
            });

			var parcels = combinationPackages
				.Union(individualPackages)
				.Select(CapParcelDimensions)
				.ToList();

			return parcels;

		}
		private static EasyPostParcel CapParcelDimensions(EasyPostParcel parcel)
		{
			parcel.height = Math.Min(parcel.height, FEDEX_MAX_PARCEL_DIMENSION);
			parcel.width = Math.Min(parcel.width, FEDEX_MAX_PARCEL_DIMENSION);
			parcel.length = Math.Min(parcel.length, FEDEX_MAX_PARCEL_DIMENSION);
			return parcel;
		}
	}
}
