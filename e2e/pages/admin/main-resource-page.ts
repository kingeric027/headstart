import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class MainResourcePage {
	createButton: Selector
	resourceList: Selector

	constructor() {
		this.createButton = Selector('button').withText(createRegExp('create'))
		this.resourceList = Selector('table')
			.find('tr')
			.withAttribute('select', 'table-body-row')
	}

	async clickCreateButton() {
		await t.click(this.createButton)
	}

	async resourceExists(resource: string) {
		return await this.resourceList.withText(resource).exists
	}
}

export default new MainResourcePage()
