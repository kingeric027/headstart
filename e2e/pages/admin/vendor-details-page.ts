import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'
import randomString from '../../helpers/random-string'

class VendorDetailsPage {
	activeToggle: Selector
	nameField: Selector
	currencySelector: Selector
	currencyOptions: Selector
	standardProductTypeCheckbox: Selector
	USCountryCheckbox: Selector
	freightPOPToggle: Selector
	createButton: Selector

	constructor() {
		this.activeToggle = Selector('#Active').parent()
		this.nameField = Selector('#Name')
		this.currencySelector = Selector('#Currency')
		this.currencyOptions = this.currencySelector.find('option')
		this.standardProductTypeCheckbox = Selector('#Standard').parent()
		this.USCountryCheckbox = Selector('#US').parent()
		this.freightPOPToggle = Selector('#SyncFreightPop').parent()
		this.createButton = Selector('button').withText(createRegExp('create'))
	}

	async createDefaultVendor() {
		const vendorName = `AutomationVendor_${randomString(5)}`
		await t.click(this.activeToggle)
		await t.typeText(this.nameField, vendorName)
		await t.click(this.currencySelector)
		await t.click(
			this.currencyOptions.withText(createRegExp('united states dollar'))
		)
		await t.click(this.freightPOPToggle)
		await t.click(this.standardProductTypeCheckbox)
		await t.click(this.USCountryCheckbox)
		await t.click(this.createButton)

		return vendorName
	}
}

export default new VendorDetailsPage()
