import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class AdminHeaderPage {
	accountDropdown: Selector
	logoutButton: Selector
	vendorsDropdown: Selector
	allVendorsLink: Selector
	usersLink: Selector
	warehousesLink: Selector

	constructor() {
		this.accountDropdown = Selector('div.current-user')
		this.logoutButton = Selector('a').withAttribute('href', '/login')
		this.vendorsDropdown = Selector('a').withText(createRegExp('vendor'))
		this.allVendorsLink = Selector('a').withText(createRegExp('all vendors'))
		this.usersLink = Selector('a').withText(createRegExp('users'))
		this.warehousesLink = Selector('a').withText(createRegExp('warehouses'))
	}

	async logout() {
		await t.click(this.accountDropdown)
		await t.click(this.logoutButton)
	}

	async selectAllVendors() {
		await t.click(this.vendorsDropdown)
		await t.click(this.allVendorsLink)
	}

	async selectVendorUsers() {
		await t.click(this.vendorsDropdown)
		await t.click(this.usersLink.filterVisible().nth(0))
	}

	async selectVendorWarehouses() {
		await t.click(this.vendorsDropdown)
		await t.click(this.warehousesLink.filterVisible().nth(0))
	}
}

export default new AdminHeaderPage()
