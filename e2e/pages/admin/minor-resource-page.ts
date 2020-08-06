import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class MinorResourcePage {
	createButton: Selector
	parentResourceDropdown: Selector
	parentResourceSearch: Selector
	resourceList: Selector

	constructor() {
		this.createButton = Selector('button').withText(createRegExp('create'))
		this.parentResourceDropdown = Selector('#parentresourcedropdown').nth(0)
		this.parentResourceSearch = Selector('#resource-search')
			.filterVisible()
			.nth(0)
		this.resourceList = Selector('table').find('tr.selectable-row')
	}

	async clickCreateButton() {
		await t.click(this.createButton)
	}

	async selectParentResourceDropdown(resource: string) {
		await t.click(this.parentResourceDropdown)
		await t.typeText(this.parentResourceSearch, resource)
		const vendorOption = this.parentResourceDropdown
			.parent()
			.find('.ps-content')
			.find('span')
			.withText(createRegExp(resource))
		await t.click(vendorOption)
	}

	async resourceExists(resouce: string) {
		return await this.resourceList.withText(resouce).exists
	}
}

export default new MinorResourcePage()
