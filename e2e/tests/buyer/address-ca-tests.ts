/* eslint-disable prettier/prettier */
import testConfig from '../../testConfig'
import {
	adminClientSetup,
	buyerTestSetup,
	baseTestCleanup,
} from '../../helpers/test-setup'
import buyerHeaderPage from '../../pages/buyer/buyer-header-page'
import addressBookPage from '../../pages/buyer/address-book-page'
import addressBookForm from '../../pages/buyer/address-book-form'

fixture`Address Tests (CA)`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.adminClientAuth = await adminClientSetup()
	})
	.beforeEach(async t => {
		t.ctx.testUser = await buyerTestSetup(t.fixtureCtx.adminClientAuth, 'CA')
	})
	.afterEach(async t => {
		await baseTestCleanup(
			t.ctx.testUser.ID,
			'0005',
			t.fixtureCtx.adminClientAuth
		)
	})
	.page(testConfig.buyerAppUrl)

test('Can I add a Canadian address? | 88551', async t => {
    await buyerHeaderPage.clickAccountButton()
    await buyerHeaderPage.clickMyAddressesLink()
    await addressBookPage.clickAddAddressButton()
    await addressBookForm.enterFirstName('Jane')
    await addressBookForm.enterLastName('Doe')
    await addressBookForm.enterStreet1('1150 Lorne Park Rd')
    await addressBookForm.enterCity('Mississauga')
    await addressBookForm.enterState('ON')
    await addressBookForm.enterZip('L5H 3A7')
    await addressBookForm.enterPhone('2265554545')
    await addressBookForm.clickSaveAddressButton()
	await t.expect(await addressBookPage.addressExists('1150 Lorne Park Rd')).ok()
	await t.expect(await addressBookPage.addressExists('Mississauga, ON L5H 3A7')).ok()
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
