/* eslint-disable prettier/prettier */
import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class AddressBookForm {
    firstNameField: Selector

	constructor() {
        this.firstNameField = Selector('#FirstName')
	}

    async enterFirstName(firstName: string) {
        await t.typeText(this.firstNameField, firstName)
    }
}

export default new AddressBookForm()
