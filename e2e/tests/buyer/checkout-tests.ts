import { ClientFunction } from 'testcafe'
import testConfig from '../../testConfig'
import {
	adminClientSetup,
	buyerTestSetup,
	baseTestCleanup,
} from '../../helpers/test-setup'
import buyerHeaderPage from '../../pages/buyer/buyer-header-page'
import productListPage from '../../pages/buyer/product-list-page'
import productDetailPage from '../../pages/buyer/product-detail-page'
import shoppingCartPage from '../../pages/buyer/shopping-cart-page'
import checkoutPage from '../../pages/buyer/checkout-page'
import loadingHelper from '../../helpers/loading-helper'
import orderDetailPage from '../../pages/buyer/order-detail-page'
import requestQuoteForm from '../../pages/buyer/request-quote-form'

const getLocation = ClientFunction(() => document.location.href)

fixture`Checkout Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.adminClientAuth = await adminClientSetup()
	})
	.beforeEach(async t => {
		t.ctx.testUser = await buyerTestSetup(t.fixtureCtx.adminClientAuth)
	})
	.afterEach(async t => {
		await baseTestCleanup(
			t.ctx.testUser.ID,
			'0005',
			t.fixtureCtx.adminClientAuth
		)
	})
	.page(testConfig.buyerAppUrl)

test('Can I checkout with 1 item? | 2473', async t => {
	await buyerHeaderPage.search('7 Mile T-Shirt')
	await productListPage.clickProduct('7 Mile T-Shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectShippingOption('7 Mile T-Shirt', '3 days')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectCreditCard(t.ctx.testUser.FirstName)
	await checkoutPage.enterCVV('900')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.clickSubmitOrderButton()
	await loadingHelper.thisWait()
	await t.expect(await orderDetailPage.productExists('7 Mile T-Shirt')).ok()
})

test('Can I checkout with multiple items in my cart? | 2475', async t => {
	await buyerHeaderPage.search('7 Mile T-Shirt')
	await productListPage.clickProduct('7 Mile T-Shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.search('Neon T-shirt')
	await productListPage.clickProduct('Neon T-shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectShippingOption('7 Mile T-Shirt', '3 days')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectCreditCard(t.ctx.testUser.FirstName)
	await checkoutPage.enterCVV('900')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.clickSubmitOrderButton()
	await loadingHelper.thisWait()
	await t.expect(await orderDetailPage.productExists('7 Mile T-Shirt')).ok()
	await t.expect(await orderDetailPage.productExists('Neon T-shirt')).ok()
})

test('Can I checkout with all items being shipped from different locations? | 2477', async t => {
	await buyerHeaderPage.search('7 Mile T-Shirt')
	await productListPage.clickProduct('7 Mile T-Shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.search('Rubber Octagonal Dumbbell')
	await productListPage.clickProduct('Rubber Octagonal Dumbbell')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectShippingOption('7 Mile T-Shirt', '3 days')
	await checkoutPage.selectShippingOption(
		'Rubber Octagonal Dumbbell',
		'3 days'
	)
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectCreditCard(t.ctx.testUser.FirstName)
	await checkoutPage.enterCVV('900')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.clickSubmitOrderButton()
	await loadingHelper.thisWait()
	await t.expect(await orderDetailPage.productExists('7 Mile T-Shirt')).ok()
	await t
		.expect(await orderDetailPage.productExists('Rubber Octagonal Dumbbell'))
		.ok()
})

test('Can the User add an address during checkout? | 19689', async t => {
	await buyerHeaderPage.search('7 Mile T-Shirt')
	await productListPage.clickProduct('7 Mile T-Shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.selectAddNewAddress()
	await checkoutPage.enterDefaultAddress(
		t.ctx.testUser.FirstName,
		t.ctx.testUser.LastName
	)
	await checkoutPage.clickSaveAndContinueButton()
})

test('Can a User checkout with a PO product in the cart? | 19977', async t => {
	await buyerHeaderPage.search('accessfob')
	await productListPage.clickProduct('accessfob')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectShippingOption('accessfob', '3 days')
	await checkoutPage.clickSaveAndContinueButton()
	await loadingHelper.thisWait()
	await checkoutPage.clickContinueButton()
	await checkoutPage.clickSubmitOrderButton()
	await loadingHelper.thisWait()
	await t.expect(await orderDetailPage.productExists('accessfob')).ok()
})

test('Can a user checkout with a PO product and standard product in their cart? | 19978', async t => {
	await buyerHeaderPage.search('7 Mile T-Shirt')
	await productListPage.clickProduct('7 Mile T-Shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.search('accessfob')
	await productListPage.clickProduct('accessfob')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectShippingOption('7 Mile T-Shirt', '3 days')
	await checkoutPage.selectShippingOption('accessfob', '3 days')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectCreditCard(t.ctx.testUser.FirstName)
	await checkoutPage.enterCVV('900')
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.clickSubmitOrderButton()
	await loadingHelper.thisWait()
	await t.expect(await orderDetailPage.productExists('7 Mile T-Shirt')).ok()
	await t.expect(await orderDetailPage.productExists('accessfob')).ok()
})

test('Can I request a quote product? | 19979', async t => {
	await buyerHeaderPage.search('4 X 4 Foot Siege Storage Rack - X1 Package')
	await productListPage.clickProduct(
		'4 X 4 Foot Siege Storage Rack - X1 Package'
	)
	await productDetailPage.clickRequestQuoteButton()
	await requestQuoteForm.enterPhoneNumber('1231231234')
	await requestQuoteForm.clickSubmitForQuoteButton()
	await productDetailPage.clickViewQuoteRequestButton()
	await t
		.expect(
			await orderDetailPage.productExists(
				'4 X 4 Foot Siege Storage Rack - X1 Package'
			)
		)
		.ok()
})
