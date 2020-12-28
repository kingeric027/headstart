using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services;
using NUnit.Framework;
using NSubstitute;
using ordercloud.integrations.avalara;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common;
using ordercloud.integrations.cardconnect;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.library;
using Marketplace.Models.Models.Marketplace;
using System.Security.Claims;
using Marketplace.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Marketplace.Tests
{
    public class OrderSubmitCommandTests
    {
        private IOrderCloudClient _oc;
        private AppSettings _settings;
        private ICreditCardCommand _card;
        private IOrderSubmitCommand _sut;

        [SetUp]
        public void Setup()
        {
            _oc = Substitute.For<IOrderCloudClient>();
            _settings = Substitute.For<AppSettings>();
            _settings.CardConnectSettings = new OrderCloudIntegrationsCardConnectConfig
            {
                UsdMerchantID = "mockUsdMerchantID",
                CadMerchantID = "mockCadMerchantID",
                EurMerchantID = "mockEurMerchantID"
            };
            _settings.OrderCloudSettings = new OrderCloudSettings
            {
                IncrementorPrefix = "SEB"
            };
            _card = Substitute.For<ICreditCardCommand>();
            _card.AuthorizePayment(Arg.Any<OrderCloudIntegrationsCreditCardPayment>(), Arg.Any<VerifiedUserContext>(), Arg.Any<string>())
                    .Returns(Task.FromResult(new Payment { }));

            _oc.Orders.PatchAsync(OrderDirection.Incoming, "mockOrderID", Arg.Any<PartialOrder>()).Returns(Task.FromResult(new Order { ID = "SEB12345" }));
            _oc.AuthenticateAsync().Returns(Task.FromResult(new TokenResponse { AccessToken = "mockToken" }));
            _oc.Orders.SubmitAsync<MarketplaceOrder>(Arg.Any<OrderDirection>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new MarketplaceOrder { ID = "submittedorderid" }));
            var telemetry = new TelemetryClient(new TelemetryConfiguration
            {
                TelemetryChannel = Substitute.For<ITelemetryChannel>()
            });
            _sut = new OrderSubmitCommand(_oc, _settings, _card, telemetry); // sut is subject under test
        }

        [Test]
        public async Task should_throw_if_order_is_already_submitted()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = true }
            }));

            // Act
            var ex = Assert.ThrowsAsync<OrderCloudIntegrationException>(async () => await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment { }, TestHelpers.MockUserContext()));

            // Assert
            Assert.AreEqual("OrderSubmit.AlreadySubmitted", ex.ApiError.ErrorCode);
        }

        [Test]
        public async Task should_throw_if_order_is_missing_shipping_selections()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        },
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = null
                        }
                    }
                }
            }));

            // Act
            var ex = Assert.ThrowsAsync<OrderCloudIntegrationException>(async () => await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment { }, TestHelpers.MockUserContext()));

            // Assert
            Assert.AreEqual("OrderSubmit.MissingShippingSelections", ex.ApiError.ErrorCode);
        }

        [Test]
        public async Task should_throw_if_has_standard_lines_and_missing_payment()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            var ex = Assert.ThrowsAsync<OrderCloudIntegrationException>(async () => await _sut.SubmitOrderAsync("mockOrderID", OrderDirection.Outgoing, null, TestHelpers.MockUserContext()));

            // Assert
            Assert.AreEqual("OrderSubmit.MissingPayment", ex.ApiError.ErrorCode);
        }

        [Test]
        public async Task should_not_increment_orderid_if_is_resubmitting()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = true } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));



            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment(), TestHelpers.MockUserContext());

            // Assert
            await _oc.Orders.DidNotReceive().PatchAsync(OrderDirection.Incoming, "mockOrderID", Arg.Any<PartialOrder>());
        }

        [Test]
        public async Task should_not_increment_orderid_if_is_already_incremented()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "SEBmockOrderID", IsSubmitted = false, xp = new Models.OrderXp { } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));



            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment(), TestHelpers.MockUserContext());

            // Assert
            await _oc.Orders.DidNotReceive().PatchAsync(OrderDirection.Incoming, "mockOrderID", Arg.Any<PartialOrder>());
        }

        [Test]
        public async Task should_increment_orderid_if_has_not_been_incremented_and_is_not_resubmit()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment(), TestHelpers.MockUserContext());

            // Assert
            await _oc.Orders.Received().PatchAsync(OrderDirection.Incoming, "mockOrderID", Arg.Any<PartialOrder>());
        }

        [Test]
        public async Task should_capture_credit_card_payment_if_has_standard_lineitems()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment(), TestHelpers.MockUserContext());

            // Assert
            await _card.Received().AuthorizePayment(Arg.Any<OrderCloudIntegrationsCreditCardPayment>(), Arg.Any<VerifiedUserContext>(), Arg.Any<string>());
        }

        [Test]
        public async Task should_not_capture_credit_card_payment_if_all_po_lineitems()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.PurchaseOrder
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment(), TestHelpers.MockUserContext());

            // Assert
            await _card.DidNotReceive().AuthorizePayment(Arg.Any<OrderCloudIntegrationsCreditCardPayment>(), Arg.Any<VerifiedUserContext>(), Arg.Any<string>());
        }


        [Test]
        public async Task should_use_usd_merchant_when_appropriate()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment { Currency = "USD" }, TestHelpers.MockUserContext());

            // Assert
            await _card.Received().AuthorizePayment(Arg.Any<OrderCloudIntegrationsCreditCardPayment>(), Arg.Any<VerifiedUserContext>(), "mockUsdMerchantID");
        }

        [Test]
        public async Task should_use_cad_merchant_when_appropriate()
        {
            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment { Currency = "CAD" }, TestHelpers.MockUserContext());

            // Assert
            await _card.Received().AuthorizePayment(Arg.Any<OrderCloudIntegrationsCreditCardPayment>(), Arg.Any<VerifiedUserContext>(), "mockCadMerchantID");
        }

        [Test]
        public async Task should_use_eur_merchant_when_appropriate()
        {
            // use eur merchant account when currency is not USD and not CAD

            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID",  OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment { Currency = "MXN" }, TestHelpers.MockUserContext());

            // Assert
            await _card.Received().AuthorizePayment(Arg.Any<OrderCloudIntegrationsCreditCardPayment>(), Arg.Any<VerifiedUserContext>(), "mockEurMerchantID");
        }

        [Test]
        public async Task should_handle_direction_outgoing()
        {
            // call order submit with direction outgoing

            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID", OrderDirection.Outgoing, new OrderCloudIntegrationsCreditCardPayment { }, TestHelpers.MockUserContext());

            // Assert
            await _oc.Orders.Received().SubmitAsync<MarketplaceOrder>(OrderDirection.Outgoing, Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task should_handle_direction_incoming()
        {
            // call order submit with direction incoming

            // Arrange
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, "mockOrderID").Returns(Task.FromResult(new MarketplaceOrderWorksheet
            {
                Order = new Models.MarketplaceOrder { ID = "mockOrderID", IsSubmitted = false, xp = new Models.OrderXp { IsResubmitting = false } },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate
                        {
                            SelectedShipMethodID = "FEDEX_GROUND"
                        }
                    }
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem
                    {
                        Product = new Models.MarketplaceLineItemProduct
                        {
                            xp = new Models.ProductXp
                            {
                                ProductType = Models.ProductType.Standard
                            }
                        }
                    }
                }
            }));

            // Act
            await _sut.SubmitOrderAsync("mockOrderID", OrderDirection.Incoming, new OrderCloudIntegrationsCreditCardPayment { }, TestHelpers.MockUserContext());

            // Assert
            await _oc.Orders.Received().SubmitAsync<MarketplaceOrder>(OrderDirection.Incoming, Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
