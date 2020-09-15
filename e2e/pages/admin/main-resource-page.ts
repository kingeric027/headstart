import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class MainResourcePage {
	createButton: Selector
	resourceList: Selector
	standardProductButton: Selector

	constructor() {
		this.createButton = Selector('button').withText(createRegExp('create'))
		this.resourceList = Selector('table').find('tr.selectable-row')
		this.standardProductButton = Selector('button').withText(
			createRegExp('standard product')
		)
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
}

export default new MainResourcePage()
