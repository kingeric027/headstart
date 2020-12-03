import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class ProductListPage {
	products: Selector
	facets: Selector

	constructor() {
		this.products = Selector('ocm-product-card')
		this.facets = Selector('ocm-facet-multiselect')
	}

	async clickProduct(product: string) {
		await t.click(this.products.withText(createRegExp(product)))
	}

	async applyFacet(facetName: string, facetSelection: string) {
		const selectedFacet = this.facets.withText(createRegExp(facetName))
		const facetlabels = selectedFacet.find('label')
		const facetSelectionValue = facetlabels.withText(
			createRegExp(facetSelection)
		)

		await t.click(facetSelectionValue)
	}
}

export default new ProductListPage()
