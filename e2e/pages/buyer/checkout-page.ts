import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class CheckoutPage {
	saveAndContinueButton: Selector
	shippingForms: Selector

	constructor() {
		this.saveAndContinueButton = Selector('button').withText(
			createRegExp('save and continue')
		)
		this.shippingForms = Selector('ocm-lineitem-table')
	}

	async clickSaveAndContinueButton() {
		await t.click(this.saveAndContinueButton)
	}

	async selectShippingOption(product: string, shippingOption: string) {
		const productForm = this.shippingForms.withText(createRegExp(product))
		const options = productForm.find('select')
		const selectedOption = options
			.find('option')
			.withText(createRegExp(shippingOption))
		await t.expect(options.visible).ok()
		await t.click(options)
		await t.click(selectedOption)
	}
}

export default new CheckoutPage()
