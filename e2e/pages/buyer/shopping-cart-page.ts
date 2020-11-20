import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class ShoppingCartPage {
	checkoutButton: Selector
	products: Selector

	constructor() {
		this.checkoutButton = Selector('button').withText(
			createRegExp('checkout')
		)
		this.products = Selector('.card-body')
	}

	async clickCheckoutButton() {
		await t.click(this.checkoutButton)
	}
}

export default new ShoppingCartPage()
