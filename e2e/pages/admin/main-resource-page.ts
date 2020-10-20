import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class MainResourcePage {
	createButton: Selector
	resourceList: Selector
	standardProductButton: Selector
	quoteProductButton: Selector
	purchaseOrderProductButton: Selector
	searchBar: Selector

	constructor() {
		this.createButton = Selector('button').withText(createRegExp('create'))
		this.resourceList = Selector('table').find('tr.selectable-row')
		this.standardProductButton = Selector('button').withText(
			createRegExp('standard product')
		)
		this.quoteProductButton = Selector('button').withText(
			createRegExp('quote product')
		)
		this.purchaseOrderProductButton = Selector('button').withText(
			createRegExp('purchase order product')
		)
		this.searchBar = Selector('#product-search')
	}

	async clickCreateButton() {
		await t.click(this.createButton)
	}

	async resourceExists(resource: string) {
		//I think this line may refresh the selector to get new elements, not sure though
		this.resourceList
		return this.resourceList.withText(resource).exists
	}

	async clickCreateNewStandardProduct() {
		await t.click(this.createButton)
		await t.click(this.standardProductButton)
	}

	async clickCreateNewQuoteProduct() {
		await t.click(this.createButton)
		await t.click(this.quoteProductButton)
	}

	async clickCreateNewPurchaseOrderProduct() {
		await t.click(this.createButton)
		await t.click(this.purchaseOrderProductButton)
	}

	async searchForResource(resourceName: string) {
		await t.typeText(this.searchBar, resourceName)
	}

	async selectResource(resourceName: string) {
		await t.click(this.resourceList.withText(resourceName))
		await loadingHelper.waitForLoadingBar()
	}
}

export default new MainResourcePage()
