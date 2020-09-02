using Marketplace.Common.Services.ShippingIntegration;
using NUnit.Framework;
using ordercloud.integrations.avalara;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.freightpop;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Marketplace.Tests.ShippingRatesTestData;

namespace Marketplace.Tests
{
    public class ShippingRatesTests
    {

        private IOCShippingIntegration _OCShippingIntegration;

       


        // could be achieved through multiple test cases, but I couldn't figure out how to do that with C# objects defined in code
        //[Test]
        //public async Task large_quantity_of_one_small_product()
        //{
        //    var testData = ShippingRatesTestData.FirstChoiceLanyards;
        //    await RunThreeDayRateComparisonTest(testData);
        //}

        //[Test]
        //public async Task medium_quantity_of_a_few_small_products()
        //{
        //    var testData = ShippingRatesTestData.FirstChoiceSmallDifferentItems;
        //    await RunThreeDayRateComparisonTest(testData);
        //}

        //[Test]
        //public async Task one_large_product()
        //{
        //    var testData = ShippingRatesTestData.ZogicsOneLargeItem;
        //    await RunThreeDayRateComparisonTest(testData);
        //}

        //private async Task RunThreeDayRateComparisonTest(ShipmentInputAndExpectedResult testData) { 
        //    var orderCaclulatePayload = ShippingRatesTestData.BuildOrderCalculatePayload(testData);
        //    var rateResponse = await _OCShippingIntegration.GetRatesAsync(orderCaclulatePayload);
        //    var slowestMethodCost = GetHighestTransitDays(rateResponse);
        //    var (min, max) = GetRateWindow(testData.ExpectedCost);
        //    Assert.Greater(slowestMethodCost, min);
        //    Assert.Less(slowestMethodCost, max);
        //}

        //private decimal GetHighestTransitDays(ShipEstimateResponse shipEstimateResponse)
        //{
        //    // should only be 1 ship from ship to combination in these tests for now
        //    var firstShippingEstimate = shipEstimateResponse.ShipEstimates.First();
        //    var sortedShipMethods = firstShippingEstimate.ShipMethods.OrderByDescending(method => method.EstimatedTransitDays);
        //    var slowestMethod = sortedShipMethods.First();
        //    return slowestMethod.Cost;
        //}

        //private (decimal, decimal) GetRateWindow(decimal expectedCost)
        //{
        //    var min = expectedCost * .85M;
        //    var max = expectedCost * 1.15M;
        //    return (min, max);
        //}
    }
}
