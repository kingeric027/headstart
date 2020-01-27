using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common.Mappers;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OrderCloud.SDK;

namespace Orchestration.Tests
{
    public class MapTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void map_test()
        {
            var str = JObject.Parse(@"{'ID':'00008949301837','Name':'Bonehead™ Short-Sleeve Shirt - 7130','ApplyTax':false,'ApplyShipping':false,'MinQuantity':1,'MaxQuantity':2500,'UseCumulativeQuantity':false,'RestrictedQuantity':false,'PriceBreaks':[{'Quantity':1,'Price':28.34}],'xp':null}");
            var obj = MarketplaceSpecMapper.Map(str.ToObject<MarketplaceSpec>());
            Assert.AreEqual(obj.Name, "Bonehead™ Short-Sleeve Shirt - 7130");

            var product = JObject.Parse(@"{
		'Facets': {
			'size': [
				'S',
				'M',
				'L',
				'XL',
				'2XL'
			],
			'color': [
				'Gulf Stream',
				'White',
				'Fossil'
			],
			'manufacturer': [
				'Columbia'
			],
			'supplier': [
				'FAST Platform'
			],
			'category': [
				'Apparel'
			]
		},
		'Shipping': null,
		'HasVariants': false,
		'Note': null,
		'UnitOfMeasure': null,
		'PriceSchedule': null,
		'IntegrationData': {
			'gtin': {
				'00008949301837': {
					'c': 'Gulf Stream',
					's': 'S'
				},
				'00008949301844': {
					'c': 'Gulf Stream',
					's': 'M'
				},
				'00008949301851': {
					'c': 'Gulf Stream',
					's': 'L'
				},
				'00008949301868': {
					'c': 'Gulf Stream',
					's': 'XL'
				},
				'00008949301875': {
					'c': 'Gulf Stream',
					's': '2XL'
				},
				'00008949680062': {
					'c': 'White',
					's': 'S'
				},
				'00008949680079': {
					'c': 'White',
					's': 'M'
				},
				'00008949680086': {
					'c': 'White',
					's': 'L'
				},
				'00008949680093': {
					'c': 'White',
					's': 'XL'
				},
				'00008949680109': {
					'c': 'White',
					's': '2XL'
				},
				'00008949680123': {
					'c': 'Fossil',
					's': 'S'
				},
				'00008949680130': {
					'c': 'Fossil',
					's': 'M'
				},
				'00008949680147': {
					'c': 'Fossil',
					's': 'L'
				},
				'00008949680154': {
					'c': 'Fossil',
					's': 'XL'
				},
				'00008949680161': {
					'c': 'Fossil',
					's': '2XL'
				}
			},
			'SupplierId': '00008949301837'
		},
		'Images': [
			{
				'URL': '{u}/front/7130_Columbia_gulfstream.jpg',
				'Tag': {
					'VariantID': '00008949301837',
					'Options': [
						'Gulf Stream',
						'S'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_gulfstream.jpg',
				'Tag': {
					'VariantID': '00008949301844',
					'Options': [
						'Gulf Stream',
						'M'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_gulfstream.jpg',
				'Tag': {
					'VariantID': '00008949301851',
					'Options': [
						'Gulf Stream',
						'L'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_gulfstream.jpg',
				'Tag': {
					'VariantID': '00008949301868',
					'Options': [
						'Gulf Stream',
						'XL'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_gulfstream.jpg',
				'Tag': {
					'VariantID': '00008949301875',
					'Options': [
						'Gulf Stream',
						'2XL'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_white.jpg',
				'Tag': {
					'VariantID': '00008949680062',
					'Options': [
						'White',
						'S'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_white.jpg',
				'Tag': {
					'VariantID': '00008949680079',
					'Options': [
						'White',
						'M'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_white.jpg',
				'Tag': {
					'VariantID': '00008949680086',
					'Options': [
						'White',
						'L'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_white.jpg',
				'Tag': {
					'VariantID': '00008949680093',
					'Options': [
						'White',
						'XL'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_white.jpg',
				'Tag': {
					'VariantID': '00008949680109',
					'Options': [
						'White',
						'2XL'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_fossil.jpg',
				'Tag': {
					'VariantID': '00008949680123',
					'Options': [
						'Fossil',
						'S'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_fossil.jpg',
				'Tag': {
					'VariantID': '00008949680130',
					'Options': [
						'Fossil',
						'M'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_fossil.jpg',
				'Tag': {
					'VariantID': '00008949680147',
					'Options': [
						'Fossil',
						'L'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_fossil.jpg',
				'Tag': {
					'VariantID': '00008949680154',
					'Options': [
						'Fossil',
						'XL'
					]
				}
			},
			{
				'URL': '{u}/front/7130_Columbia_fossil.jpg',
				'Tag': {
					'VariantID': '00008949680161',
					'Options': [
						'Fossil',
						'2XL'
					]
				}
			}
		],
		'OwnerID': null,
		'DefaultPriceScheduleID': '00008949301837',
		'AutoForward': true,
		'ID': '00008949301837',
		'Name': 'Mens Bonehead™ Short-Sleeve Shirt - 7130',
		'Description': '• 3.8 oz., 100% cotton poplin<BR>• Performance fishing gear<BR>• Self-adhesive collar points<BR>• Vented<BR>• Utility loop<BR>• Columbia PFG logo patch on rod holder on left chest<BR>• PFG logo patch on center back<BR>',
		'QuantityMultiplier': 1,
		'ShipWeight': null,
		'ShipHeight': null,
		'ShipWidth': null,
		'ShipLength': null,
		'Active': true,
		'SpecCount': 0,
		'xp': null,
		'VariantCount': 0,
		'ShipFromAddressID': null,
		'Inventory': null,
		'DefaultSupplierID': 'FASTPlatform'
	}");
            var mappedProduct = MarketplaceProductMapper.Map(product.ToObject<MarketplaceProduct>());
            Assert.AreEqual(mappedProduct.Name, "Mens Bonehead™ Short-Sleeve Shirt - 7130");

            var partial = JObject.Parse(@"{ 'Name': 'New Name'}");
            var partialMapped = MarketplaceProductMapper.Map(partial.ToObject<Partial<MarketplaceProduct>>());
            Assert.AreEqual(partialMapped.Name, "New Name");
        }
    }
}
