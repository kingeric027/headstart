import { Selector, t } from 'testcafe'
import { createRegExp } from '../helpers/regExp-helper'

class HeaderPage {
	accountDropdown: Selector
	logoutButton: Selector

	constructor() {
		this.accountDropdown = Selector('#account-dropdown').withText(
			createRegExp('account')
		)
		this.logoutButton = Selector('a').withAttribute('href', '/login')
	}

	async clickAccountButton() {
		await t.click(this.accountDropdown)
	}

	async logout() {
		await this.clickAccountButton()
		await t.click(this.logoutButton)
	}
}

export default new HeaderPage()
