using System;
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
        private const string TWO_LINEITEMS_SAME_PRODUCT_ID = @"{
            'Order': {
                'ID': 'SEB000124',
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
                'BillingAddressID': '0003-0001',
                'BillingAddress': {
                    'ID': '0003-0001',
                    'DateCreated': null,
                    'CompanyName': 'Basecamp Burlingame, Ca',
                    'FirstName': '',
                    'LastName': '',
                    'Street1': '261 California Dr',
                    'Street2': '',
                    'City': 'Burlingame',
                    'State': 'CA',
                    'Zip': '94010',
                    'Country': 'US',
                    'Phone': '',
                    'AddressName': 'Burlingame, CA',
                    'xp': {
                        'Coordinates': null,
                        'Accessorials': null,
                        'Email': 'BurlingameCA@basecampfitness.com',
                        'LocationID': 'BCF10001',
                        'AvalaraCertificateID': null,
                        'AvalaraCertificateExpiration': null
                    }
                },
                'ShippingAddressID': '0003-0001',
                'Comments': '',
                'LineItemCount': 2,
                'Status': 'Open',
                'DateCreated': '2020-06-22T16:14:11.343+00:00',
                'DateSubmitted': '2020-06-22T16:20:12.477+00:00',
                'DateApproved': null,
                'DateDeclined': null,
                'DateCanceled': null,
                'DateCompleted': null,
                'Subtotal': 50,
                'ShippingCost': 20.64,
                'TaxCost': 4.74,
                'PromotionDiscount': 0,
                'Total': 75.38,
                'IsSubmitted': true,
                'xp': {
                    'AvalaraTaxTransactionCode': '',
                    'OrderType': 'Standard',
                    'QuoteOrderInfo': null,
                    'Currency': 'USD',
                    'OrderReturnInfo': {
                        'HasReturn': false
                    },
                    'ClaimStatus': 'NoClaim',
                    'ShippingStatus': 'Processing',
                    'ApprovalNeeded': ''
                }
            },
            'LineItems': [
                {
                    'ID': 'Dnz1uBExm02yDlPIUqizzg',
                    'ProductID': '500CLASSHOODY',
                    'Quantity': 1,
                    'DateAdded': '2020-06-22T16:14:47.63+00:00',
                    'QuantityShipped': 0,
                    'UnitPrice': 25,
                    'PromotionDiscount': 0,
                    'LineTotal': 25,
                    'LineSubtotal': 25,
                    'CostCenter': null,
                    'DateNeeded': null,
                    'ShippingAccount': null,
                    'ShippingAddressID': '0003-0001',
                    'ShipFromAddressID': '012-02',
                    'Product': {
                        'ID': '500CLASSHOODY',
                        'Name': '500 Class Hoodie',
                        'Description': null,
                        'QuantityMultiplier': 1,
                        'ShipWeight': 0.8,
                        'ShipHeight': 1,
                        'ShipWidth': 12,
                        'ShipLength': 6,
                        'xp': {
                            'Facets': {
                                'supplier': [
                                    'Self Esteem Brands Distribution'
                                ],
                                'size': [
                                    'Small',
                                    'Medium',
                                    'Large',
                                    'X-Large',
                                    'X-Small'
                                ]
                            },
                            'IntegrationData': null,
                            'Status': 'Draft',
                            'HasVariants': false,
                            'Note': '',
                            'Tax': {
                                'Category': 'P0000000',
                                'Code': 'PC030000',
                                'Description': 'Clothing And Related Products (Business-To-Business)'
                            },
                            'UnitOfMeasure': {
                                'Qty': 1,
                                'Unit': 'each'
                            },
                            'ProductType': 'Standard',
                            'IsResale': true,
                            'Accessorials': null,
                            'Currency': 'USD'
                        }
                    },
                    'Variant': {
                        'ID': '500CLASSHOODY-Medium',
                        'Name': '500CLASSHOODY-Medium',
                        'Description': null,
                        'ShipWeight': null,
                        'ShipHeight': null,
                        'ShipWidth': null,
                        'ShipLength': null,
                        'xp': {
                            'SpecCombo': 'Medium',
                            'SpecValues': [
                                {
                                    'SpecName': 'Size',
                                    'SpecOptionValue': 'Medium',
                                    'PriceMarkup': ''
                                }
                            ],
                            'NewID': null
                        }
                    },
                    'ShippingAddress': {
                        'ID': '0003-0001',
                        'DateCreated': null,
                        'CompanyName': 'Basecamp Burlingame, Ca',
                        'FirstName': '',
                        'LastName': '',
                        'Street1': '261 California Dr',
                        'Street2': '',
                        'City': 'Burlingame',
                        'State': 'CA',
                        'Zip': '94010',
                        'Country': 'US',
                        'Phone': '',
                        'AddressName': 'Burlingame, CA',
                        'xp': {
                            'Coordinates': null,
                            'Accessorials': null,
                            'Email': 'BurlingameCA@basecampfitness.com',
                            'LocationID': 'BCF10001',
                            'AvalaraCertificateID': null,
                            'AvalaraCertificateExpiration': null
                        }
                    },
                    'ShipFromAddress': {
                        'ID': '012-02',
                        'DateCreated': null,
                        'CompanyName': 'Self Esteem Brands Distribution',
                        'FirstName': '',
                        'LastName': '',
                        'Street1': '5805 W 6th Ave',
                        'Street2': null,
                        'City': 'Lakewood',
                        'State': 'CO',
                        'Zip': '80214-2453',
                        'Country': 'US',
                        'Phone': '',
                        'AddressName': 'SEB Distribution Denver',
                        'xp': null
                    },
                    'SupplierID': '012',
                    'Specs': [
                        {
                            'SpecID': '500CLASSHOODYSize',
                            'Name': 'Size',
                            'OptionID': 'Medium',
                            'Value': 'Medium',
                            'PriceMarkupType': 'NoMarkup',
                            'PriceMarkup': null
                        }
                    ],
                    'xp': {
                        'LineItemStatus': 'Complete',
                        'LineItemReturnInfo': null,
                        'LineItemImageUrl': 'https://marketplace-middleware-test.azurewebsites.net/products/500CLASSHOODY/image',
                        'UnitPriceInProductCurrency': 25
                    }
                },
                {
                    'ID': 'BnJ3vXvKAki_NmyqUvqSwQ',
                    'ProductID': '500CLASSHOODY',
                    'Quantity': 1,
                    'DateAdded': '2020-06-22T16:14:53.3+00:00',
                    'QuantityShipped': 0,
                    'UnitPrice': 25,
                    'PromotionDiscount': 0,
                    'LineTotal': 25,
                    'LineSubtotal': 25,
                    'CostCenter': null,
                    'DateNeeded': null,
                    'ShippingAccount': null,
                    'ShippingAddressID': '0003-0001',
                    'ShipFromAddressID': '012-02',
                    'Product': {
                        'ID': '500CLASSHOODY',
                        'Name': '500 Class Hoodie',
                        'Description': null,
                        'QuantityMultiplier': 1,
                        'ShipWeight': 0.8,
                        'ShipHeight': 1,
                        'ShipWidth': 12,
                        'ShipLength': 6,
                        'xp': {
                            'Facets': {
                                'supplier': [
                                    'Self Esteem Brands Distribution'
                                ],
                                'size': [
                                    'Small',
                                    'Medium',
                                    'Large',
                                    'X-Large',
                                    'X-Small'
                                ]
                            },
                            'IntegrationData': null,
                            'Status': 'Draft',
                            'HasVariants': false,
                            'Note': '',
                            'Tax': {
                                'Category': 'P0000000',
                                'Code': 'PC030000',
                                'Description': 'Clothing And Related Products (Business-To-Business)'
                            },
                            'UnitOfMeasure': {
                                'Qty': 1,
                                'Unit': 'each'
                            },
                            'ProductType': 'Standard',
                            'IsResale': true,
                            'Accessorials': null,
                            'Currency': 'USD'
                        }
                    },
                    'Variant': {
                        'ID': '500CLASSHOODY-XSmall',
                        'Name': '500CLASSHOODY-XSmall',
                        'Description': null,
                        'ShipWeight': null,
                        'ShipHeight': null,
                        'ShipWidth': null,
                        'ShipLength': null,
                        'xp': {
                            'SpecCombo': 'XSmall',
                            'SpecValues': [
                                {
                                    'SpecName': 'Size',
                                    'SpecOptionValue': 'X-Small',
                                    'PriceMarkup': ''
                                }
                            ],
                            'NewID': null
                        }
                    },
                    'ShippingAddress': {
                        'ID': '0003-0001',
                        'DateCreated': null,
                        'CompanyName': 'Basecamp Burlingame, Ca',
                        'FirstName': '',
                        'LastName': '',
                        'Street1': '261 California Dr',
                        'Street2': '',
                        'City': 'Burlingame',
                        'State': 'CA',
                        'Zip': '94010',
                        'Country': 'US',
                        'Phone': '',
                        'AddressName': 'Burlingame, CA',
                        'xp': {
                            'Coordinates': null,
                            'Accessorials': null,
                            'Email': 'BurlingameCA@basecampfitness.com',
                            'LocationID': 'BCF10001',
                            'AvalaraCertificateID': null,
                            'AvalaraCertificateExpiration': null
                        }
                    },
                    'ShipFromAddress': {
                        'ID': '012-02',
                        'DateCreated': null,
                        'CompanyName': 'Self Esteem Brands Distribution',
                        'FirstName': '',
                        'LastName': '',
                        'Street1': '5805 W 6th Ave',
                        'Street2': null,
                        'City': 'Lakewood',
                        'State': 'CO',
                        'Zip': '80214-2453',
                        'Country': 'US',
                        'Phone': '',
                        'AddressName': 'SEB Distribution Denver',
                        'xp': null
                    },
                    'SupplierID': '012',
                    'Specs': [
                        {
                            'SpecID': '500CLASSHOODYSize',
                            'Name': 'Size',
                            'OptionID': 'XSmall',
                            'Value': 'X-Small',
                            'PriceMarkupType': 'NoMarkup',
                            'PriceMarkup': null
                        }
                    ],
                    'xp': {
                        'LineItemStatus': 'Complete',
                        'LineItemReturnInfo': null,
                        'LineItemImageUrl': 'https://marketplace-middleware-test.azurewebsites.net/products/500CLASSHOODY/image',
                        'UnitPriceInProductCurrency': 25
                    }
                }
            ],
            'ShipEstimateResponse': {
                'ShipEstimates': [
                    {
                        'ID': '012-02',
                        'xp': {},
                        'SelectedShipMethodID': 'FedexParcel-3cbe3fc1-23d2-46f8-8d3b-1f64e906fe02',
                        'ShipEstimateItems': [
                            {
                                'LineItemID': 'Dnz1uBExm02yDlPIUqizzg',
                                'Quantity': 1
                            },
                            {
                                'LineItemID': 'BnJ3vXvKAki_NmyqUvqSwQ',
                                'Quantity': 1
                            }
                        ],
                        'ShipMethods': [
                            {
                                'ID': 'FedexParcel-3cbe3fc1-23d2-46f8-8d3b-1f64e906fe02',
                                'Name': 'FEDEX_GROUND',
                                'Cost': 20.64,
                                'EstimatedTransitDays': 3,
                                'xp': {
                                    'OriginalShipCost': 20.64,
                                    'OriginalCurrency': 'USD',
                                    'ExchangeRate': 1,
                                    'OrderCurrency': 'USD'
                                }
                            },
                            {
                                'ID': 'FedexParcel-d8a40d0a-68e2-4b32-85c9-f436133b55ba',
                                'Name': 'FEDEX_2_DAY',
                                'Cost': 44.04,
                                'EstimatedTransitDays': 2,
                                'xp': {
                                    'OriginalShipCost': 44.04,
                                    'OriginalCurrency': 'USD',
                                    'ExchangeRate': 1,
                                    'OrderCurrency': 'USD'
                                }
                            },
                            {
                                'ID': 'FedexParcel-54019d30-86fc-4828-a760-fe6b48c52b43',
                                'Name': 'STANDARD_OVERNIGHT',
                                'Cost': 108.86,
                                'EstimatedTransitDays': 1,
                                'xp': {
                                    'OriginalShipCost': 108.86,
                                    'OriginalCurrency': 'USD',
                                    'ExchangeRate': 1,
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
                'TaxTotal': 4.74,
                'HttpStatusCode': 200,
                'UnhandledErrorBody': null,
                'xp': {}
            },
            'OrderSubmitResponse': null
        }";
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

        //[Test]
        //public async Task Test()
        //{
        //var zoho_config = new ZohoClientConfig()
        //{
        //	ClientId = "1000.LYTODQT800N5C6UWEMRKKRS3VM7RPH",
        //	ClientSecret = "d6c6960a7d742efd8230bd010e83eb86fae6c2dc87",
        //	AccessToken = "1000.e9b088c5a817701588daf498a8231d69.467c7b3949d6d37a9982c18d865a2749",
        //	ApiUrl = "https://books.zoho.com/api/v3",
        //	OrganizationID = "708781679"
        //};
        //var oc_config = new OrderCloudClientConfig()
        //{
        //	AuthUrl = "https://stagingauth.ordercloud.io",
        //	ApiUrl = "https://stagingapi.ordercloud.io",
        //	ClientId = "0CC8282F-8EA9-4040-B1D9-BC03AC9FBB6B",
        //	ClientSecret = "ulhq0P2DrdvzjBngQhv3DLus15V3VZEGYG0vYuVtBCRrruDNCpQXl11Sfinb",
        //	GrantType = GrantType.ClientCredentials,
        //	Roles = new[] { ApiRole.FullAccess }
        //};
        //var command = new ZohoCommand(zoho_config, oc_config);
        //var wk = JsonConvert.DeserializeObject<MarketplaceOrderWorksheet>(TWO_LINEITEMS_SAME_PRODUCT_ID);
        //var order = await command.CreateSalesOrder(wk);
        //	}
    }
}
