﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Common.Services.Zoho;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using OrderCloud.SDK;

namespace Marketplace.Tests
{
    public class ZohoTests
    {
        private const string worksheet = @"{
		'Order': {
			'ID': 'SEB000206',
			'FromUser': {
				'ID': '0003-00004',
				'Username': 'billapprover',
				'Password': null,
				'FirstName': 'bill',
				'LastName': 'hickey',
				'Email': 'test@test.com',
				'Phone': '',
				'TermsAccepted': null,
				'Active': true,
				'xp': null,
				'AvailableRoles': null,
				'DateCreated': '2020-04-10T21:23:43.137+00:00',
				'PasswordLastSetDate': '2020-04-10T21:31:48.04+00:00'
			},
			'FromCompanyID': '0003',
			'ToCompanyID': 'rQYR6T6ZTEqVrgv8x_ei0g',
			'FromUserID': '0003-00004',
			'BillingAddressID': '0003-0003',
			'BillingAddress': {
				'ID': '0003-0003',
				'DateCreated': null,
				'CompanyName': 'Basecamp Minneapolis, MN',
				'FirstName': '',
				'LastName': '',
				'Street1': '100 Hennepin Ave',
				'Street2': '',
				'City': 'Minneapolis',
				'State': 'MN',
				'Zip': '55402',
				'Country': 'US',
				'Phone': '',
				'AddressName': 'Minneapolis, MN',
				'xp': {
					'Coordinates': null,
					'Accessorials': null,
					'Email': '',
					'AvalaraCertificateID': 53,
					'AvalaraCertificateExpiration': '2020-05-31T00:00:00+00:00'
				}
			},
			'ShippingAddressID': '0003-0003',
			'Comments': '',
			'LineItemCount': 1,
			'Status': 'Open',
			'DateCreated': '2020-06-18T15:47:38.163+00:00',
			'DateSubmitted': '2020-06-18T16:05:45.43+00:00',
			'DateApproved': null,
			'DateDeclined': null,
			'DateCanceled': null,
			'DateCompleted': null,
			'Subtotal': 4.12,
			'ShippingCost': 8.7,
			'TaxCost': 1.02,
			'PromotionDiscount': 0.0,
			'Total': 13.84,
			'IsSubmitted': true,
			'xp': {
				'AvalaraTaxTransactionCode': '',
				'OrderType': 'Standard',
				'QuoteOrderInfo': null,
				'Currency': 'USD',
				'OrderReturnInfo': {
					'HasReturn': false
				},
				'ApprovalNeeded': ''
			}
		},
		'LineItems': [
			{
				'ID': 'LmeWgQvLIEyT_bIWlUQeGg',
				'ProductID': 'baseballglove',
				'Quantity': 1,
				'DateAdded': '2020-06-18T15:55:50.03+00:00',
				'QuantityShipped': 0,
				'UnitPrice': 4.11824,
				'PromotionDiscount': 0.0,
				'LineTotal': 4.12,
				'LineSubtotal': 4.12,
				'CostCenter': null,
				'DateNeeded': null,
				'ShippingAccount': null,
				'ShippingAddressID': '0003-0003',
				'ShipFromAddressID': 'Basecamp-01',
				'Product': {
					'ID': 'baseballglove',
					'Name': 'Baseball Glove',
					'Description': null,
					'QuantityMultiplier': 1,
					'ShipWeight': 1.0,
					'ShipHeight': 1.0,
					'ShipWidth': 2.0,
					'ShipLength': 1.0,
					'xp': {
						'Facets': {
							'supplier': [
								'Basecamp Fitness Distribution'
							]
						},
						'IntegrationData': null,
						'Status': 'Draft',
						'HasVariants': false,
						'Note': '',
						'Tax': {
							'Category': 'P0000000',
							'Code': 'PC030400',
							'Description': 'Clothing And Related Products (Business-To-Business)-Sports/recreational equipment'
						},
						'UnitOfMeasure': {
							'Qty': 1,
							'Unit': 'Per'
						},
						'ProductType': 'Standard',
						'IsResale': false,
						'Accessorials': null,
						'Currency': 'CHF'
					}
				},
				'Variant': null,
				'ShippingAddress': {
					'ID': '0003-0003',
					'DateCreated': null,
					'CompanyName': 'Basecamp Minneapolis, MN',
					'FirstName': '',
					'LastName': '',
					'Street1': '100 Hennepin Ave',
					'Street2': '',
					'City': 'Minneapolis',
					'State': 'MN',
					'Zip': '55402',
					'Country': 'US',
					'Phone': '',
					'AddressName': 'Minneapolis, MN',
					'xp': {
						'Coordinates': null,
						'Accessorials': null,
						'Email': '',
						'AvalaraCertificateID': 53,
						'AvalaraCertificateExpiration': '2020-05-31T00:00:00+00:00'
					}
				},
				'ShipFromAddress': {
					'ID': 'Basecamp-01',
					'DateCreated': null,
					'CompanyName': 'Basecamp Fitness Minneapolis',
					'FirstName': '',
					'LastName': '',
					'Street1': '100 Hennepin Ave',
					'Street2': null,
					'City': 'Minneapolis',
					'State': 'MN',
					'Zip': '55401-1903',
					'Country': 'US',
					'Phone': '',
					'AddressName': 'Basecamp Fitness Minneapolis',
					'xp': null
				},
				'SupplierID': 'Basecamp',
				'Specs': [],
				'xp': {
					'LineItemStatus': 'Complete',
					'LineItemReturnInfo': null,
					'LineItemImageUrl': 'https://marketplace-middleware-test.azurewebsites.net/products/baseballglove/image',
					'UnitPriceInProductCurrency': 4.0
				}
			}
		],
		'ShipEstimateResponse': {
			'ShipEstimates': [
				{
					'ID': 'Basecamp-01',
					'xp': {},
					'SelectedShipMethodID': 'FedexParcel-5e780d9d-a89c-4ab2-81cf-b48014cfe609',
					'ShipEstimateItems': [
						{
							'LineItemID': 'LmeWgQvLIEyT_bIWlUQeGg',
							'Quantity': 1
						}
					],
					'ShipMethods': [
						{
							'ID': 'FedexParcel-5e780d9d-a89c-4ab2-81cf-b48014cfe609',
							'Name': 'FEDEX_GROUND',
							'Cost': 8.7,
							'EstimatedTransitDays': 1,
							'xp': {
								'OriginalShipCost': 8.7,
								'OriginalCurrency': 'USD',
								'ExchangeRate': 1.0,
								'OrderCurrency': 'USD'
							}
						},
						{
							'ID': 'FedexParcel-7e4c88be-731f-417a-9f1a-eace3bd96845',
							'Name': 'FEDEX_EXPRESS_SAVER',
							'Cost': 15.58,
							'EstimatedTransitDays': 3,
							'xp': {
								'OriginalShipCost': 15.58,
								'OriginalCurrency': 'USD',
								'ExchangeRate': 1.0,
								'OrderCurrency': 'USD'
							}
						},
						{
							'ID': 'FedexParcel-fb0d99fb-2ca3-4a3c-9915-d85d725f215e',
							'Name': 'FEDEX_2_DAY',
							'Cost': 17.67,
							'EstimatedTransitDays': 2,
							'xp': {
								'OriginalShipCost': 17.67,
								'OriginalCurrency': 'USD',
								'ExchangeRate': 1.0,
								'OrderCurrency': 'USD'
							}
						}
					]
				}
			],
			'HttpStatusCode': 200,
			'UnhandledErrorBody': null,
			'xp': {}
		},
		'OrderCalculateResponse': {
			'LineItemOverrides': [],
			'ShippingTotal': null,
			'TaxTotal': 1.02,
			'HttpStatusCode': 200,
			'UnhandledErrorBody': null,
			'xp': {}
		},
		'OrderSubmitResponse': null
	}";

		[SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test()
        {
            var zoho_config = new ZohoClientConfig()
            {
                ClientId = "1000.LYTODQT800N5C6UWEMRKKRS3VM7RPH",
                ClientSecret = "d6c6960a7d742efd8230bd010e83eb86fae6c2dc87",
                AccessToken = "1000.e9b088c5a817701588daf498a8231d69.467c7b3949d6d37a9982c18d865a2749",
                ApiUrl = "https://books.zoho.com/api/v3",
                OrganizationID = "708781679"
            };
            var oc_config = new OrderCloudClientConfig()
            {
                AuthUrl = "https://stagingauth.ordercloud.io",
                ApiUrl = "https://stagingapi.ordercloud.io",
                ClientId = "0CC8282F-8EA9-4040-B1D9-BC03AC9FBB6B",
                ClientSecret = "ulhq0P2DrdvzjBngQhv3DLus15V3VZEGYG0vYuVtBCRrruDNCpQXl11Sfinb",
                GrantType = GrantType.ClientCredentials,
                Roles = new[] {ApiRole.FullAccess}
            };
            var command = new ZohoCommand(zoho_config, oc_config);
            var wk = JsonConvert.DeserializeObject<MarketplaceOrderWorksheet>(worksheet);
            var order = await command.CreateSalesOrder(wk);

        }
    }
}
