/* eslint-disable prettier/prettier */
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
import addressBookPage from '../../pages/buyer/address-book-page'
import { address } from 'faker'
import addressBookForm from '../../pages/buyer/address-book-form'

fixture`Address Tests`
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

test('Can I add a valid address? | 87548', async t => {
    await buyerHeaderPage.clickAccountButton()
    await buyerHeaderPage.clickMyAddressesLink()
    await addressBookPage.clickAddAddressButton()
    await addressBookForm.enterFirstName('Jane')
	// await await t.expect(await addressBookPage.clickAddAddressButton()).ok()
})

// test('Can I checkout with 1 item? | 2473', async t => {
// 	const productName = '100 CLASS T-SHIRT'
// 	await buyerHeaderPage.search(productName)
// 	await productListPage.clickProduct(productName)
// 	await productDetailPage.clickAddToCartButton()
// 	await buyerHeaderPage.clickCartButton()
// 	await shoppingCartPage.clickCheckoutButton()
// 	await checkoutPage.clickSaveAndContinueButton()
// 	await checkoutPage.selectShippingOption(productName, 'day')
// 	await checkoutPage.clickSaveAndContinueButton()
// 	await checkoutPage.selectCreditCard(t.ctx.testUser.FirstName)
// 	await checkoutPage.enterCVV('900')
// 	await checkoutPage.clickSaveAndContinueButton()
// 	await checkoutPage.clickSubmitOrderButton()
// 	await loadingHelper.thisWait()
// 	await t.expect(await orderDetailPage.productExists(productName)).ok()
// })
