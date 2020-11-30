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

fixture`Address Tests (US)`
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

test('Can I add a valid American address? | 87548', async t => {
    await buyerHeaderPage.clickAccountButton()
    await buyerHeaderPage.clickMyAddressesLink()
    await addressBookPage.clickAddAddressButton()
    await addressBookForm.enterFirstName('Jane')
    await addressBookForm.enterLastName('Doe')
    await addressBookForm.enterStreet1('110 N 5th St')
    await addressBookForm.enterStreet2('Suite 300')
    await addressBookForm.enterCity('Minneapolis')
    await addressBookForm.enterState('MN')
    await addressBookForm.enterZip('55403')
    await addressBookForm.enterPhone('6515554545')
    await addressBookForm.clickSaveAddressButton()
	await t.expect(await addressBookPage.addressExists('110 N 5th St Ste 300')).ok()
	await t.expect(await addressBookPage.addressExists('Minneapolis, MN 55403-1631')).ok()
})

test('Does SmartyStreets offer suggestions for bad addresses? | 87549', async t => {
	await buyerHeaderPage.clickAccountButton()
    await buyerHeaderPage.clickMyAddressesLink()
    await addressBookPage.clickAddAddressButton()
    await addressBookForm.enterFirstName('Jane')
    await addressBookForm.enterLastName('Doe')
    await addressBookForm.enterStreet1('110 N 5th St')
    await addressBookForm.enterStreet2('Suite 300')
    await addressBookForm.enterCity('King of Prussia')
    await addressBookForm.enterState('PA')
    await addressBookForm.enterZip('55444')
    await addressBookForm.enterPhone('6515554545')
	await addressBookForm.clickSaveAddressButton()
	await t.expect(addressBookPage.smartyStreetsSuggestionHeader.exists).ok()
})

test('Are required fields being enforced? | 87550', async t => {
	await buyerHeaderPage.clickAccountButton()
    await buyerHeaderPage.clickMyAddressesLink()
    await addressBookPage.clickAddAddressButton()
    await addressBookForm.enterFirstName('Jane')
    await addressBookForm.enterLastName('Doe')
    await addressBookForm.enterStreet1('110 N 5th St')
    await addressBookForm.enterCity('Minneapolis')
    await addressBookForm.enterZip('55403')
	// No state
	await addressBookForm.isButtonDisabled()
	await addressBookForm.enterState('MN')
	await addressBookForm.removeFirstName()
	// No first name
	await addressBookForm.isButtonDisabled()
	await addressBookForm.enterFirstName('Jane')
	await addressBookForm.removeLastName()
	// No last name
	await addressBookForm.isButtonDisabled()
	await addressBookForm.enterLastName('Doe')
	await addressBookForm.removeStreet1()
	// No street 1
	await addressBookForm.isButtonDisabled()
	await addressBookForm.enterStreet1('110 N 5th St')
	await addressBookForm.removeZip()
	// No zip
	await addressBookForm.isButtonDisabled()
	await addressBookForm.enterZip('55403')
	// Now okay to save
	await addressBookForm.clickSaveAddressButton()
	await t.expect(await addressBookPage.addressExists('110 N 5th St')).ok()
})
