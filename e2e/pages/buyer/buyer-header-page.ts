import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class BuyerHeaderPage {
	accountDropdown: Selector
	logoutButton: Selector
	productsLink: Selector
	searchBar: Selector
	cartButton: Selector

	constructor() {
		this.accountDropdown = Selector('#account-dropdown').withText(
			createRegExp('account')
		)
		this.logoutButton = Selector('a').withAttribute('href', '/login')
		this.productsLink = Selector('a').withText(createRegExp('products'))
		this.searchBar = Selector('#search-addon')
		this.cartButton = Selector('span').withText(createRegExp('cart'))
	}

	async clickAccountButton() {
		await t.click(this.accountDropdown)
	}

	async logout() {
		await this.clickAccountButton()
		await t.click(this.logoutButton)
	}

	async clickProductsLink() {
		await t.click(this.productsLink)
	}

	async search(searchText: string) {
		await t.typeText(this.searchBar, searchText)
		await loadingHelper.waitForLoadingBar()
	}

	async clickCartButton() {
		await t.click(this.cartButton)
	}
}

export default new BuyerHeaderPage()
