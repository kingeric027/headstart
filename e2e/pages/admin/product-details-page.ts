import { Selector, t } from 'testcafe'
import randomString from '../../helpers/random-string'
import { createRegExp } from '../../helpers/regExp-helper'
import {
	scrollIntoView,
	clickLeftOfElement,
} from '../../helpers/element-helper'
import loadingHelper from '../../helpers/loading-helper'

class ProductDetailsPage {
	nameField: Selector
	skuField: Selector
	activeToggle: Selector
	quantityPerUnitField: Selector
	unitOfMeasureField: Selector
	taxCategoryDropdown: Selector
	taxCategoryOptions: Selector
	taxCodeDropdown: Selector
	taxCodeOptions: Selector
	productWeightField: Selector
	shipAddressDropdown: Selector
	shipAddressOptions: Selector
	pricingTab: Selector
	buyerVisibilityTab: Selector
	priceField: Selector
	createButton: Selector
	sizeTierDropdown: Selector
	sizeTierOptions: Selector

	constructor() {
		this.nameField = Selector('#Name')
		this.skuField = Selector('#ID')
		this.activeToggle = Selector('label')
			.withText(createRegExp('active'))
			.find('span')
		this.quantityPerUnitField = Selector('#UnitOfMeasureQty')
		this.unitOfMeasureField = Selector('#UnitOfMeasureUnit')
		this.taxCategoryDropdown = Selector('#TaxCodeCategory')
		this.taxCategoryOptions = this.taxCategoryDropdown.find('option')
		this.productWeightField = Selector('#ShipWeight')
		this.taxCodeDropdown = Selector('#productTaxCodeDropdown')
		this.taxCodeOptions = this.taxCodeDropdown.parent().find('button')
		this.shipAddressDropdown = Selector('#ShipFromAddressID')
		this.shipAddressOptions = this.shipAddressDropdown.find('option')
		this.pricingTab = Selector('a').withText(createRegExp('pricing'))
		this.buyerVisibilityTab = Selector('a').withText(
			createRegExp('buyer visibility')
		)
		this.priceField = Selector('#Price')
		this.createButton = Selector('button')
			.withText(createRegExp('create'))
			.withAttribute('type', 'submit')
		this.sizeTierDropdown = Selector('#SizeTier')
		this.sizeTierOptions = this.sizeTierDropdown.find('option')
	}

	async createDefaultProduct() {
		const productName = `AutomationProduct_${randomString(5)}`
		await t.typeText(this.nameField, productName)
		await t.typeText(this.quantityPerUnitField, '1')
		await t.typeText(this.unitOfMeasureField, 'Unit')
		await scrollIntoView('#TaxCodeCategory')
		await t.click(this.taxCategoryDropdown)
		await t.click(this.taxCategoryOptions.withText(createRegExp('freight')))
		await t.click(this.taxCodeDropdown)
		await t.click(
			this.taxCodeOptions.withText(
				createRegExp('delivery by company vehicle')
			)
		)
		await t.click(this.shipAddressDropdown)
		await t.click(
			this.shipAddressOptions.withText(createRegExp('automation'))
		)
		await t.typeText(this.productWeightField, '5')
		await t.click(this.sizeTierDropdown)
		await t.click(
			this.sizeTierOptions.withText(createRegExp('2 - 5 units will fit'))
		)
		await t.click(this.pricingTab)
		await t.typeText(this.priceField, '5')
		await clickLeftOfElement(this.priceField)
		await t.click(this.createButton)
		await loadingHelper.waitForLoadingBar()

		return productName
	}

	async clickBuyerVisibilityTab() {
		await t.click(this.buyerVisibilityTab)
	}
}

export default new ProductDetailsPage()
