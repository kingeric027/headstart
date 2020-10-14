using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

	//public static class SmartPackageMapper
	//{
	//    public static RateRequestBody Map(List<LineItem> obj)
	//    {
	//        var firstLineItem = obj[0];
	//        var shipToAddress = firstLineItem.ShippingAddress;
	//        var shipFromAddress = firstLineItem.ShipFromAddress;


	//        return new RateRequestBody
	//        {
	//            ConsigneeAddress = RateAddressMapper.Map(shipToAddress),
	//            ShipperAddress = RateAddressMapper.Map(shipFromAddress),
	//            Items = MapLineItemsIntoPackages(obj),
	//            Accessorials = AccessorialMapper.Map(obj, shipToAddress, shipFromAddress)
	//        };
	//    }

	//    private static Dictionary<SizeTier, decimal> SIZE_FACTOR_MAP = new Dictionary<SizeTier, decimal>()
	//    {
	//        { SizeTier.A, .385M },
	//        { SizeTier.B, .10M },
	//        { SizeTier.C, .031M },
	//        { SizeTier.D, .0134M },
	//        { SizeTier.E, .0018M },
	//        { SizeTier.F, .00067M }
	//    };

	//    private static Dictionary<decimal, int> PERCENT_FILL_TO_SIDE_LENGTH_MAP = new Dictionary<decimal, int>()
	//    {
	//        { 0.0117392937640872M, 5 },
	//        { 0.0202854996243426M, 6 },
	//        { 0.0322126220886552M, 7 },
	//        { 0.048084147257701M, 8 },
	//        { 0.0684635612321563M, 9 },
	//        { 0.0939143501126972M, 10 },
	//        { 0.125M, 11 },
	//        { 0.162283996994741M, 12 },
	//        { 0.206329827197596M, 13 },
	//        { 0.257700976709241M, 14 },
	//        { 0.316960931630353M, 15 },
	//        { 0.384673178061608M, 16 },
	//        { 0.461401202103681M, 17 },
	//        { 0.54770848985725M, 18 },
	//        { 0.64415852742299M, 19 },
	//        { 0.751314800901578M, 20 },
	//        { 0.869740796393689M, 21 },
	//        { 1M, 22 }
	//    };

	//    private static decimal MAX_PERCENT_FILL = .80M;

	//    private static List<Item> MapLineItemsIntoPackages(List<MarketplaceLineItem> lineItems)
	//    {
	//        var lineItemsCanCombineBySizeDescending = lineItems.Where(li => li.Product.xp.SizeTier != SizeTier.G).OrderBy(lineItem => lineItem.Product.xp.SizeTier);

	//        // tuple item1 = percent filled, item 2 = weight
	//        var packageContents = new List<Tuple<decimal, decimal>>();
	//        var currentPercentFilled = 0M;
	//        var currentWeight = 0M;

	//        foreach (var lineItem in lineItemsCanCombineBySizeDescending)
	//        {
	//            var i = 0;
	//            var percentByQuantity = SIZE_FACTOR_MAP[lineItem.Product.xp.SizeTier];
	//            while (i < lineItem.Quantity)
	//            {
	//                if (currentPercentFilled + percentByQuantity < MAX_PERCENT_FILL)
	//                {
	//                    currentPercentFilled += percentByQuantity;
	//                    currentWeight += lineItem.Product.ShipWeight ?? 0M;
	//                }
	//                else
	//                {
	//                    packageContents.Add(new Tuple<decimal, decimal>(currentPercentFilled, currentWeight));
	//                    currentWeight = 0;
	//                    currentPercentFilled = 0;
	//                }
	//                i++;
	//            }
	//        }

	//        if (currentPercentFilled > 0 && currentWeight > 0)
	//        {
	//            packageContents.Add(new Tuple<decimal, decimal>(currentPercentFilled, currentWeight));
	//        }

	//        var packages = packageContents.Select(packageContents => GetSideLengthAndWeight(packageContents));

	//        var combinationPackages = packages.Select((package, index) =>
	//        {
	//            return new Item()
	//            {
	//                Weight = package.Item2,
	//                Length = package.Item1,
	//                Width = package.Item1,
	//                Height = package.Item1,
	//                Quantity = 1,
	//                PackageId = index.ToString(),
	//                PackageType = PackageType.Box,
	//                FreightClass = 1,
	//                Unit = Unit.lbs_inch
	//            };
	//        }).ToList();

	//        var individualLineItems = lineItems.Where(li => li.Product.xp.SizeTier == SizeTier.G);
	//        var individualPackages = individualLineItems.Select(li =>
	//        {
	//            return new Item()
	//            {
	//                Weight = li.Product.ShipWeight ?? 0,
	//                Length = li.Product.ShipLength ?? 0,
	//                Width = li.Product.ShipWidth ?? 0,
	//                Height = li.Product.ShipHeight ?? 0,
	//                Quantity = li.Quantity,
	//                PackageId = li.ID,
	//                PackageType = PackageType.Box,
	//                FreightClass = 1,
	//                Unit = Unit.lbs_inch
	//            };
	//        });

	//        return combinationPackages.Union(individualPackages).ToList();
	//    }

	//    private static Tuple<int, decimal> GetSideLengthAndWeight(Tuple<decimal, decimal> packageContents)
	//    {
	//        var (percentFilled, weight) = packageContents;
	//        var bigEnoughBox = PERCENT_FILL_TO_SIDE_LENGTH_MAP.First(entry =>
	//        {
	//            var boxPercentSide = entry.Key;
	//            var sideLength = entry.Value;
	//            var paddedPercentBoxSize = boxPercentSide * MAX_PERCENT_FILL;

	//            return percentFilled < paddedPercentBoxSize;
	//        });

	//        return new Tuple<int, decimal>(bigEnoughBox.Value, weight);

	//    }
	//}
}
