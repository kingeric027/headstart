import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { ClientFunction } from 'testcafe'
import loginPage from '../../pages/login-page'
import testConfig from '../../testConfig'
import headerPage from '../../pages/buyer/buyer-header-page'
import {
	adminClientSetup,
	loginTestSetup,
	loginTestCleanup,
	buyerTestSetup,
} from '../../helpers/test-setup'
import buyerHeaderPage from '../../pages/buyer/buyer-header-page'
import productListPage from '../../pages/buyer/product-list-page'
import productDetailPage from '../../pages/buyer/product-detail-page'
import shoppingCartPage from '../../pages/buyer/shopping-cart-page'
import checkoutPage from '../../pages/buyer/checkout-page'

const getLocation = ClientFunction(() => document.location.href)

fixture`Checkout Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.adminClientAuth = await adminClientSetup()
	})
	.page(testConfig.buyerAppUrl)

test
	.before(async t => {
		t.ctx.testUser = await buyerTestSetup(t.fixtureCtx.adminClientAuth)
	})
	.after(async t => {
		await loginTestCleanup(
			t.ctx.testUser.ID,
			'0005',
			t.fixtureCtx.adminClientAuth
		)
	})('Checkout Test', async t => {
	await buyerHeaderPage.search('7 Mile T-Shirt')
	await productListPage.clickProduct('7 Mile T-Shirt')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await shoppingCartPage.clickCheckoutButton()
	await checkoutPage.clickSaveAndContinueButton()
	await checkoutPage.selectShippingOption('7 Mile T-Shirt', '3 days')
	await checkoutPage.clickSaveAndContinueButton()
	await t.debug()
	//add credit card for user in setup
})
