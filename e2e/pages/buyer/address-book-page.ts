/* eslint-disable prettier/prettier */
import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class AddressBookPage {
    addAddressButton: Selector

	constructor() {
        this.addAddressButton = Selector('.btn').withText(createRegExp('add'))
	}

	async clickAddAddressButton() {
		await t.click(this.addAddressButton)
    }

}

export default new AddressBookPage()
