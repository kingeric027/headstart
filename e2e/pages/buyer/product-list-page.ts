import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class ProductListPage {
	products: Selector

	constructor() {
		this.products = Selector('ocm-product-card')
	}

	async clickProduct(product: string) {
		await t.click(this.products.withText(createRegExp(product)))
	}
}

export default new ProductListPage()
