import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class ProductDetailPage {
	addToCartButton: Selector

	constructor() {
		this.addToCartButton = Selector('button').withText(
			createRegExp('add to cart')
		)
	}

	async clickAddToCartButton() {
		await t.click(this.addToCartButton)
	}
}

export default new ProductDetailPage()
