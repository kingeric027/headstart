import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class ProductDetailPage {
	addToCartButton: Selector
	requestQuoteButton: Selector
	viewQuoteRequestButton: Selector

	constructor() {
		this.addToCartButton = Selector('button').withText(
			createRegExp('add to cart')
		)
		this.requestQuoteButton = Selector('button').withText(
			createRegExp('request quote')
		)
		this.viewQuoteRequestButton = Selector('button').withText(
			createRegExp('view quote request')
		)
	}

	async clickAddToCartButton() {
		await t.click(this.addToCartButton)
	}

	async clickRequestQuoteButton() {
		await t.click(this.requestQuoteButton)
	}

	async clickViewQuoteRequestButton() {
		await t.click(this.viewQuoteRequestButton)
		await loadingHelper.thisWait()
	}
}

export default new ProductDetailPage()
