/* eslint-disable prettier/prettier */
import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class AddressBookForm {
    firstNameField: Selector
    lastNameField: Selector
    street1Field: Selector
    street2Field: Selector
    cityField: Selector
    stateDropdown: Selector
    stateOptions: Selector
    zipField: Selector
    phoneField: Selector
    saveAddressButton: Selector

	constructor() {
        this.firstNameField = Selector('#FirstName').nth(1)
        this.lastNameField = Selector('#LastName').nth(1)
        this.street1Field = Selector('#Street1').nth(1)
        this.street2Field = Selector('#Street2').nth(1)
        this.cityField = Selector('#City').nth(1)
        this.stateDropdown = Selector('#State').nth(1)
        this.stateOptions = this.stateDropdown.find('option')
        this.zipField = Selector('#zipCode').nth(1)
        this.phoneField = Selector('#Phone').nth(1)
        this.saveAddressButton = Selector('#address-save-button').nth(1)
	}

    async enterFirstName(firstName: string) {
        await t.typeText(this.firstNameField, firstName)
    }

    async removeFirstName() {
        await t.click(this.firstNameField)
        await t.pressKey('ctrl+a delete')
    }

    async enterLastName(lastName: string) {
        await t.typeText(this.lastNameField, lastName)
    }

    async removeLastName() {
        await t.click(this.lastNameField)
        await t.pressKey('ctrl+a delete')
    }

    async enterStreet1(street1: string) {
        await t.typeText(this.street1Field, street1)
    }

    async removeStreet1() {
        await t.click(this.street1Field)
        await t.pressKey('ctrl+a delete')
    }

    async enterStreet2(street2: string) {
        await t.typeText(this.street2Field, street2)
    }

    async enterCity(city: string) {
        await t.typeText(this.cityField, city)
    }

    async enterState(state: string) {
        await t.click(this.stateDropdown)
        await t.click(this.stateOptions.withText(createRegExp(state)))
    }

    async enterZip(zip: string) {
        await t.typeText(this.zipField, zip)
    }

    async enterPhone(phone: string) {
        await t.typeText(this.phoneField, phone)
    }

    async removeZip() {
        await t.click(this.zipField)
        await t.pressKey('ctrl+a delete')
    }

    async clickSaveAddressButton() {
        await t.click(this.saveAddressButton)
    }

    async isButtonDisabled() {
        await t.expect(this.saveAddressButton.hasAttribute('disabled')).ok()
    }
}

export default new AddressBookForm()
