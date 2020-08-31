using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Newtonsoft.Json;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;

namespace Marketplace.Tests
{
    public static class ShippingRatesTestData
    {
        private static string OrderWorksheetBase = @"{
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


        public static OrderCalculatePayload BuildOrderCalculatePayload(ShipmentInputAndExpectedResult input)
        {
            var orderWorksheet = JsonConvert.DeserializeObject<MarketplaceOrderWorksheet>(OrderWorksheetBase);
            orderWorksheet.LineItems = BuildLineItems(input.ShipFrom, input.ShipTo, input.Products);
            var orderCalculatePayload = new OrderCalculatePayload<MarketplaceOrderWorksheet>()
            {
                ConfigData = new CheckoutIntegrationConfiguration{
                    ExcludePOProductsFromShipping = false,
                    ExcludePOProductsFromTax = true,
                },
                OrderWorksheet = orderWorksheet
            };
            return orderCalculatePayload.Reserialize<OrderCalculatePayload>();
        }

        public static OrderCalculatePayload BuildOrderCalculatePayload(MarketplaceAddressSupplier shipFrom, MarketplaceAddressBuyer shipTo, List<Tuple<MarketplaceProduct, int>> products)
        {
            var orderWorksheet = JsonConvert.DeserializeObject<MarketplaceOrderWorksheet>(OrderWorksheetBase);
            orderWorksheet.LineItems = BuildLineItems(shipFrom, shipTo, products);
            var orderCalculatePayload = new OrderCalculatePayload<MarketplaceOrderWorksheet>()
            {
                ConfigData = new CheckoutIntegrationConfiguration
                {
                    ExcludePOProductsFromShipping = false,
                    ExcludePOProductsFromTax = true,
                },
                OrderWorksheet = orderWorksheet
            };
            return orderCalculatePayload.Reserialize<OrderCalculatePayload>();
        }

        private static List<MarketplaceLineItem> BuildLineItems(MarketplaceAddressSupplier shipFrom, MarketplaceAddressBuyer shipTo, List<Tuple<MarketplaceProduct, int>> Products)
        {
            var lineItems = new List<MarketplaceLineItem>();

            foreach (var item in Products)
            {
                var product = item.Item1;
                var quantity = item.Item2;

                var marketplaceLineItemProduct = new MarketplaceLineItemProduct()
                {
                    ShipWidth = product.ShipWidth,
                    ShipWeight = product.ShipWeight,
                    ShipHeight = product.ShipHeight,
                    ShipLength = product.ShipLength,
                    xp = product.xp,
                    QuantityMultiplier = product.QuantityMultiplier,
                    Description = product.Description,
                    ID = product.ID,
                    Name = product.Name
                };
                var lineItem = new MarketplaceLineItem()
                {
                    ShipFromAddress = shipFrom,
                    ShippingAddress = shipTo,
                    Product = marketplaceLineItemProduct,
                    LineTotal = 10,
                    Quantity = quantity
                };
                lineItems.Add(lineItem);
            }
            return lineItems;
        }

        private static MarketplaceAddressSupplier FirstChoiceWarehouse = new MarketplaceAddressSupplier()
        {
            Street1 = "13025 George Weber Dr #5",
            State = "MN",
            City = "Rogers",
            Zip = "55374",
            Country = "US"
        };

        private static MarketplaceAddressSupplier ZogicWarehouse = new MarketplaceAddressSupplier()
        {
            Street1 = "309 Pittsfiels Road",
            State = "MA",
            City = "Lenox",
            Zip = "01240",
            Country = "US"
        };

        private static MarketplaceAddressBuyer ShipTo1 = new MarketplaceAddressBuyer()
        {
            Street1 = "821 BROWN SCHOOL RD",
            City = "Evansville",
            State = "WI",
            Country = "US",
            Zip = "53536"
        };

        private static MarketplaceAddressBuyer ShipTo2 = new MarketplaceAddressBuyer()
        {
            Street1 = "3290 SE 58TH AVE",
            City = "Ocala",
            State = "FL",
            Country = "US",
            Zip = "34480"
        };

        private static MarketplaceAddressBuyer ShipTo3 = new MarketplaceAddressBuyer()
        {
            Street1 = "18124 COLE CT",
            City = "EDEN PRAIRIE",
            State = "MN",
            Zip = "55347-6501",
            Country = "US"
        };

        private static MarketplaceAddressBuyer ShipTo4 = new MarketplaceAddressBuyer()
        {
            Street1 = "600 E COLORADO BLVD",
            City = "Pasadena",
            State = "CA",
            Zip = "91101",
            Country = "US"
        };

        private static MarketplaceAddressBuyer ShipTo5 = new MarketplaceAddressBuyer()
        {
            Street1 = "210 FRONTIER ST, STE 1",
            City = "Lexington",
            State = "NE",
            Zip = "68850",
            Country = "US"
        };

        public static ShipmentInputAndExpectedResult FirstChoiceLanyards = new ShipmentInputAndExpectedResult()
        {
            ShipFrom = FirstChoiceWarehouse,
            ShipTo = ShipTo4,
            Products = new List<Tuple<MarketplaceProduct, int>>() {
            new Tuple<MarketplaceProduct, int>(new MarketplaceProduct(){
                ID  = "1",
                ShipWeight = 0.06M,
                ShipWidth = 1,
                ShipLength = 10,
                ShipHeight = 1,
                xp = new ProductXp {
                    SizeTier = SizeTier.E
                }
            }, 100) },
            ExpectedCost = 14.62M
        };

        public static ShipmentInputAndExpectedResult FirstChoiceSmallDifferentItems = new ShipmentInputAndExpectedResult()
        {
            ShipFrom = FirstChoiceWarehouse,
            ShipTo = ShipTo5,
            Products = new List<Tuple<MarketplaceProduct, int>>() {
            new Tuple<MarketplaceProduct, int>(
                new MarketplaceProduct(){
                ID  = "1",
                Name = "Wall Clock",
                ShipWeight = 7M,
                ShipWidth = 4,
                ShipLength = 10,
                ShipHeight = 2,
                xp = new ProductXp
                {
                    SizeTier = SizeTier.B
                }
            }, 4),
            new Tuple<MarketplaceProduct, int>(
                new MarketplaceProduct(){
                ID  = "2",
                Name = "Vest",
                ShipWeight = 0.95M,
                ShipWidth = 6,
                ShipLength = 4,
                ShipHeight = 1,
                xp = new ProductXp
                {
                    SizeTier = SizeTier.C
                }
            }, 1),
            new Tuple<MarketplaceProduct, int>(
                new MarketplaceProduct(){
                ID  = "3",
                Name = "Shirt",
                ShipWeight = 0.57M,
                ShipWidth = 6,
                ShipLength = 4,
                ShipHeight = 1,
                xp = new ProductXp
                {
                    SizeTier = SizeTier.C
                }
            }, 1)
            },
            ExpectedCost = 123.01M
        };

        public static ShipmentInputAndExpectedResult ZogicsOneLargeItem = new ShipmentInputAndExpectedResult()
        {
            ShipFrom = ZogicWarehouse,
            ShipTo = ShipTo3,
            Products = new List<Tuple<MarketplaceProduct, int>>() {
            new Tuple<MarketplaceProduct, int>(
                new MarketplaceProduct(){
                ID  = "1",
                Name = "Hand Soap",
                ShipWeight = 36.5M,
                ShipWidth = 10,
                ShipLength = 10,
                ShipHeight = 10,
                xp = new ProductXp
                {
                    SizeTier = SizeTier.A
                }
            }, 1),
            },
            ExpectedCost = 25.20M
        };

        public class ShipmentInputAndExpectedResult
        {
            public MarketplaceAddressSupplier ShipFrom { get; set; }
            public MarketplaceAddressBuyer ShipTo { get; set; }
            public List<Tuple<MarketplaceProduct, int>> Products { get; set; }
            public decimal ExpectedCost { get; set; }
        }

    }
}
