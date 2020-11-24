/* eslint-disable prettier/prettier */
import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class Homepage {
	accountDropdown: Selector
	homepageLogo: Selector
	featuredProductsH3: Selector
	refineByH2: Selector
	supplierTitleH1: Selector
	productsLink: Selector
	suppliersLink: Selector
	myProfileLink: Selector
	myAddressesLink: Selector
	myLocationsLink: Selector
	myCreditCardsLink: Selector
	ordersDropdown: Selector
	placedByMeLink: Selector
	placedInMyLocationsLink: Selector
	awaitingMyApprovalLink: Selector

	constructor() {
		this.accountDropdown = Selector('#accountDropdown').withText(
			createRegExp('account')
		)
		this.homepageLogo = Selector('img').withAttribute('class', 'logo')
		this.featuredProductsH3 = Selector('h3').withText(createRegExp('featured products'))
		this.refineByH2 = Selector('h2').withText(createRegExp('refine by'))
		// TODO: Add ID to the h1 element there so it can be identified regardless of 
		// what word is used from the translation file (i.e. vendor, supplier, etc)
		// this.supplierTitleH1 = Selector('h1').withText(createRegExp('refine by'))
		this.productsLink = Selector('a').withText(createRegExp('products'))
		// TODO: Add ID to this element so that we are able to select it regardless of translated text
		this.suppliersLink = Selector('a').withText(createRegExp('suppliers'))
		this.myProfileLink = Selector('a').withText(createRegExp('my profile'))
		this.myAddressesLink = Selector('a').withText(
			createRegExp('my addresses')
		)
		this.myLocationsLink = Selector('a').withText(
			createRegExp('my locations')
		)
		this.myCreditCardsLink = Selector('a').withText(
			createRegExp('my credit cards')
		)
		this.ordersDropdown = Selector('#account-dropdown').withText(
			createRegExp('orders')
		)
		this.placedByMeLink = Selector('a').withText(createRegExp('placed by me'))
		this.placedInMyLocationsLink = Selector('a').withText(
			createRegExp('placed in my locations')
		)
		this.awaitingMyApprovalLink = Selector('a').withText(
			createRegExp('awaiting my approval')
		)
	}

	async clickHomepageLogo() {
		await t.click(this.homepageLogo)
	}

	async clickAccountButton() {
		await t.click(this.accountDropdown)
	}

	async clickOrdersButton() {
		await t.click(this.ordersDropdown)
	}

	async clickMyProfileLink() {
		await t.click(this.myProfileLink)
	}

	async clickMyAddressesLink() {
		await t.click(this.myAddressesLink)
	}

	async clickMyLocationsLink() {
		await t.click(this.myLocationsLink)
	}

	async clickMyCreditCardsLink() {
		await t.click(this.myCreditCardsLink)
	}

	async clickPlacedByMeLink() {
		await t.click(this.placedByMeLink)
	}

	async clickPlacedInMyLocationsLink() {
		await t.click(this.placedInMyLocationsLink)
	}

	async clickAwaitingMyApprovalLink() {
		await t.click(this.awaitingMyApprovalLink)
	}

	async clickProductsLink() {
		await t.click(this.productsLink)
	}

	async clickSuppliersLink() {
		await t.click(this.suppliersLink)
	}
}

export default new Homepage()
