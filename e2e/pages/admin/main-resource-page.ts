import { Selector, t } from 'testcafe'
import loadingHelper from '../../helpers/loading-helper'
import { createRegExp } from '../../helpers/regExp-helper'

class MainResourcePage {
	createButton: Selector
	resourceList: Selector
	standardProductButton: Selector
	searchBar: Selector

	constructor() {
		this.createButton = Selector('button').withText(createRegExp('create'))
		this.resourceList = Selector('table').find('tr.selectable-row')
		this.standardProductButton = Selector('button').withText(
			createRegExp('standard product')
		)
		this.searchBar = Selector('#product-search')
	}

	async clickCreateButton() {
		await t.click(this.createButton)
	}

	async resourceExists(resource: string) {
		return await this.resourceList.withText(resource).exists
	}

	async clickCreateNewStandardProduct() {
		await t.click(this.createButton)
		await t.click(this.standardProductButton)
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
