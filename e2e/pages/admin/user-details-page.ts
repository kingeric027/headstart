import { Selector, t } from 'testcafe'
import faker = require('faker')
import { createRegExp } from '../../helpers/regExp-helper'

class UserDetailsPage {
	activeToggle: Selector
	usernameField: Selector
	emailField: Selector
	firstNameField: Selector
	lastNameField: Selector
	createButton: Selector

	constructor() {
		this.activeToggle = Selector('#Active').parent()
		this.createButton = Selector('button').withText(createRegExp('create'))
		this.usernameField = Selector('#Username')
		this.emailField = Selector('#Email')
		this.firstNameField = Selector('#FirstName')
		this.lastNameField = Selector('#LastName')
	}

	async createDefaultUser() {
		const firstName = faker.name.firstName()
		const lastName = faker.name.lastName()
		const firstNameReplaced = firstName.replace(/'/g, '')
		const lastNameReplaced = lastName.replace(/'/g, '')
		const email = `${firstNameReplaced}${lastNameReplaced}.hpmqx9la@mailosaur.io`

		await t.click(this.activeToggle)
		await t.typeText(this.usernameField, email)
		await t.typeText(this.emailField, email)
		await t.typeText(this.firstNameField, firstNameReplaced)
		await t.typeText(this.lastNameField, lastNameReplaced)

		await t.click(this.createButton)

		return email
	}
}

export default new UserDetailsPage()
