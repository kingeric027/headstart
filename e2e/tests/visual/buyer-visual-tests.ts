/* eslint-disable @typescript-eslint/ban-ts-ignore */
import Eyes from '@applitools/eyes-testcafe'
import { t } from 'testcafe'
import { checkWindow, getBrowsers } from '../../helpers/eyes-helper'
import loadingHelper from '../../helpers/loading-helper'
import { adminClientSetup, buyerTestSetup } from '../../helpers/test-setup'
import buyerHeaderPage from '../../pages/buyer/buyer-header-page'
import checkoutPage from '../../pages/buyer/checkout-page'
import orderDetailPage from '../../pages/buyer/order-detail-page'
import productDetailPage from '../../pages/buyer/product-detail-page'
import productListPage from '../../pages/buyer/product-list-page'
import shoppingCartPage from '../../pages/buyer/shopping-cart-page'
import testConfig from '../../testConfig'

const eyes = new Eyes()
const appName = 'Headstart Buyer'

fixture`Headstart Buyer Visual Tests`
	.meta('TestRun', 'Visual')
	.before(async ctx => {
		ctx.adminClientAuth = await adminClientSetup()
	})
	.afterEach(async () => {
		await eyes.close()
	})
	.after(async () => {
		await eyes.waitForResults()
	})
	.page(testConfig.buyerAppUrl)

test('Buyer Login Page', async t => {
	await t.maximizeWindow()
	await eyes.open({
		appName: appName,
		// @ts-ignore
		testName: t.testRun.test.name,
		// @ts-ignore
		browser: getBrowsers(),
		t,
		accessibilityValidation: { level: 'AA', guidelinesVersion: 'WCAG_2_0' },
	})
	// @ts-ignore
	await checkWindow(eyes, t.testRun.test.name)
})

test.before(async t => {
	await buyerTestSetup(t.fixtureCtx.adminClientAuth)
})('Buyer Homepage', async t => {
	await eyes.open({
		appName: appName,
		// @ts-ignore
		testName: t.testRun.test.name,
		// @ts-ignore
		browser: getBrowsers(),
		t,
		accessibilityValidation: { level: 'AA', guidelinesVersion: 'WCAG_2_0' },
	})
	// @ts-ignore
	await checkWindow(eyes, t.testRun.test.name)
})

test.before(async t => {
	t.ctx.testUser = await buyerTestSetup(t.fixtureCtx.adminClientAuth)
})('Buyer Checkout Workflow', async t => {
	await eyes.open({
		appName: appName,
		// @ts-ignore
		testName: t.testRun.test.name,
		// @ts-ignore
		browser: getBrowsers(),
		t,
		accessibilityValidation: { level: 'AA', guidelinesVersion: 'WCAG_2_0' },
	})
	const productName = '100 CLASS T-SHIRT'
	await buyerHeaderPage.search(productName)
	await t.wait(2000)
	await checkWindow(eyes, 'Product List Page')
	await productListPage.clickProduct(productName)
	await t.wait(2000)
	await checkWindow(eyes, 'Product Detail Page')
	await productDetailPage.clickAddToCartButton()
	await buyerHeaderPage.clickCartButton()
	await t.wait(2000)
	await checkWindow(eyes, 'Cart Page')
	await shoppingCartPage.clickCheckoutButton()
	await t.wait(2000)
	await checkWindow(eyes, 'Shipping Address Page')
	await checkoutPage.clickSaveAndContinueButton()
	await t.wait(2000)
	await checkWindow(eyes, 'Shipping Selection Page')
	await checkoutPage.selectShippingOption(productName, '3 days')
	await checkoutPage.clickSaveAndContinueButton()
	await t.wait(2000)
	await checkWindow(eyes, 'Payment Page')
	await checkoutPage.selectCreditCard(t.ctx.testUser.FirstName)
	await checkoutPage.enterCVV('900')
	await checkoutPage.clickSaveAndContinueButton()
	await t.wait(2000)
	await checkWindow(eyes, 'Confirm Order Page')
	await checkoutPage.clickSubmitOrderButton()
	await loadingHelper.thisWait()
	await t.expect(await orderDetailPage.productExists(productName)).ok()
	await t.wait(2000)
	await checkWindow(eyes, 'Order Details Page')
})
