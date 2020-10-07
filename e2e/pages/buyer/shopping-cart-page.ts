import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class ShoppingCartPage {
	checkoutButton: Selector

	constructor() {
		this.checkoutButton = Selector('button').withText(
			createRegExp('checkout')
		)
	}

	async clickCheckoutButton() {
		await t.click(this.checkoutButton)
	}
}

export default new ShoppingCartPage()
