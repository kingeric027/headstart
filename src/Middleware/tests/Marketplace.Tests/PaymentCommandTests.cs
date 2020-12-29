using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using OrderCloud.SDK;
using Marketplace.Common.Services;
using ordercloud.integrations.cardconnect;
using Marketplace.Common.Commands;
using System.Threading.Tasks;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Tests.Mocks;

namespace Marketplace.Tests
{
    class PaymentCommandTests
    {
        private IOrderCloudClient _oc;
        private IOrderCalcService _orderCalc;
        private ICreditCardCommand _ccCommand;
        private IPaymentCommand _sut;
        private string mockOrderID = "mockOrderID";
        private string mockUserToken = "mockUserToken";
        private string mockPoPaymentID = "mockPoPaymentID";
        private string mockCCPaymentID = "mockCCPaymentID";
        private string creditcard1 = "creditcard1";
        private string creditcard2 = "creditcard2";

        [SetUp]
        public void Setup()
        {
            // oc
            _oc = Substitute.For<IOrderCloudClient>();
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, mockOrderID)
                    .Returns(Task.FromResult(new MarketplaceOrderWorksheet { Order = new Models.MarketplaceOrder { ID = mockOrderID } }));
            _oc.Payments.CreateAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID, Arg.Any<MarketplacePayment>())
                .Returns(Task.FromResult(PaymentMocks.CCPayment(creditcard1, 20)));
            
            // orderCalcc
            _orderCalc = Substitute.For<IOrderCalcService>();
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(20);

            // ccCommand
            _ccCommand = Substitute.For<ICreditCardCommand>();
            _ccCommand.VoidPaymentAsync(Arg.Any<MarketplacePayment>(), Arg.Any<MarketplaceOrder>(), mockUserToken)
                .Returns(Task.FromResult(0));

            _sut = new PaymentCommand(_oc, _orderCalc, _ccCommand);
        }

        [Test]
        public async Task should_delete_stale_payments()
        {
            // Arrange
            var mockedCreditCardTotal = 20;
            var existing = PaymentMocks.PaymentList(PaymentMocks.POPayment(20));
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.CCPayment(creditcard1));
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedCreditCardTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.Received().DeleteAsync(OrderDirection.Incoming, mockOrderID, mockPoPaymentID);
            await _oc.Payments.Received().CreateAsync<MarketplacePayment>(OrderDirection.Outgoing, mockOrderID, Arg.Is<MarketplacePayment>(p => p.ID == mockCCPaymentID && p.Type == PaymentType.CreditCard && p.Amount == mockedCreditCardTotal), mockUserToken);
        }

        [Test] public async Task should_handle_new_cc_payment()
        {
            // Arrange
            var mockedCreditCardTotal = 30;
            var existing = PaymentMocks.EmptyPaymentsList();
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.CCPayment(creditcard1));
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedCreditCardTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.DidNotReceive().DeleteAsync(OrderDirection.Incoming, mockOrderID, Arg.Any<string>());
            await _oc.Payments.Received().CreateAsync<MarketplacePayment>(OrderDirection.Outgoing, mockOrderID, Arg.Is<MarketplacePayment>(p => p.ID == mockCCPaymentID && p.Type == PaymentType.CreditCard && p.Amount == mockedCreditCardTotal), mockUserToken);
        }

        [Test]
        public async Task should_handle_same_cc_different_amount()
        {
            // if the credit card hasn't changed but the amount has
            // then we should void any existing transactions if necessary and update the payment 

            // Arrange
            var mockedCreditCardTotal = 30;
            var existing = PaymentMocks.PaymentList(PaymentMocks.CCPayment(creditcard1, 20));
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.CCPayment(creditcard1));
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedCreditCardTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.DidNotReceive().DeleteAsync(OrderDirection.Incoming, mockOrderID, Arg.Any<string>());
            await _ccCommand.Received().VoidPaymentAsync(Arg.Is<MarketplacePayment>(p => p.ID == mockCCPaymentID), Arg.Is<MarketplaceOrder>(o => o.ID == mockOrderID), mockUserToken);
            await _oc.Payments.Received().PatchAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID, mockCCPaymentID, Arg.Is<PartialPayment>(p => p.Amount == mockedCreditCardTotal));
        }

        [Test]
        public async Task should_handle_different_cc_same_amount()
        {
            // if the credit card has changed we need to delete the payment
            // but should void the existing authorization before that

            // Arrange
            var mockedCreditCardTotal = 50;
            var existing = PaymentMocks.PaymentList(PaymentMocks.CCPayment(creditcard1, 50, mockCCPaymentID));
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.CCPayment(creditcard2));
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedCreditCardTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.Received().DeleteAsync(OrderDirection.Incoming, mockOrderID, mockCCPaymentID);
            await _ccCommand.Received().VoidPaymentAsync(Arg.Is<MarketplacePayment>(p => p.ID == mockCCPaymentID), Arg.Is<MarketplaceOrder>(o => o.ID == mockOrderID), mockUserToken);
            await _oc.Payments.Received().CreateAsync<MarketplacePayment>(OrderDirection.Outgoing, mockOrderID, Arg.Is<MarketplacePayment>(p => p.CreditCardID == creditcard2 && p.Amount == mockedCreditCardTotal), mockUserToken);
        }

        [Test]
        public async Task should_handle_different_cc_different_amount()
        {
            // if the credit card has changed we need to delete the payment
            // but should void the existing authorization before that

            // Arrange
            var mockedCreditCardTotal = 50;
            var existing = PaymentMocks.PaymentList(PaymentMocks.CCPayment(creditcard1, 40, mockCCPaymentID));
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.CCPayment(creditcard2));
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedCreditCardTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.Received().DeleteAsync(OrderDirection.Incoming, mockOrderID, mockCCPaymentID);
            await _ccCommand.Received().VoidPaymentAsync(Arg.Is<MarketplacePayment>(p => p.ID == mockCCPaymentID), Arg.Is<MarketplaceOrder>(o => o.ID == mockOrderID), mockUserToken);
            await _oc.Payments.Received().CreateAsync<MarketplacePayment>(OrderDirection.Outgoing, mockOrderID, Arg.Is<MarketplacePayment>(p => p.CreditCardID == creditcard2 && p.Amount == mockedCreditCardTotal), mockUserToken);
        }

        [Test]
        public async Task should_handle_same_cc_same_amount()
        {
            // do nothing, payment doesn't need updating

            // Arrange
            var mockedCreditCardTotal = 50;
            var existing = PaymentMocks.PaymentList(PaymentMocks.CCPayment(creditcard1, 50, mockCCPaymentID));
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.CCPayment(creditcard1));
            _orderCalc.GetCreditCardTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedCreditCardTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.DidNotReceive().DeleteAsync(OrderDirection.Incoming, mockOrderID, Arg.Any<string>());
            await _ccCommand.DidNotReceive().VoidPaymentAsync(Arg.Any<MarketplacePayment>(), Arg.Any<MarketplaceOrder>(), mockUserToken);
            await _oc.Payments.DidNotReceive().CreateAsync<MarketplacePayment>(Arg.Any<OrderDirection>(), mockOrderID, Arg.Any<MarketplacePayment>(), mockUserToken);
            await _oc.Payments.DidNotReceive().PatchAsync<MarketplacePayment>(Arg.Any<OrderDirection>(), mockOrderID, Arg.Any<string>(), Arg.Any<PartialPayment>());
        }

        [Test]
        public async Task should_handle_new_po_payment()
        {
            // Arrange
            var mockedPOTotal = 50;
            var existing = PaymentMocks.EmptyPaymentsList();
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.POPayment());
            _orderCalc.GetPurchaseOrderTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedPOTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.Received().CreateAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID, Arg.Is<MarketplacePayment>(p => p.ID == mockPoPaymentID && p.Amount == mockedPOTotal));
        }

        [Test]
        public async Task should_handle_existing_po_payment()
        {
            // Arrange
            var mockedPOTotal = 30;
            var existing = PaymentMocks.PaymentList(PaymentMocks.POPayment(40));
            _oc.Payments.ListAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID)
                .Returns(Task.FromResult(existing));
            var requested = PaymentMocks.Payments(PaymentMocks.POPayment());
            _orderCalc.GetPurchaseOrderTotal(Arg.Any<MarketplaceOrderWorksheet>())
                .Returns(mockedPOTotal);

            // Act
            var result = await _sut.SavePayments(mockOrderID, requested, mockUserToken);

            // Assert
            await _oc.Payments.Received().PatchAsync<MarketplacePayment>(OrderDirection.Incoming, mockOrderID, mockPoPaymentID, Arg.Is<PartialPayment>(p => p.Amount == mockedPOTotal));
        }
    }
}
