/* eslint-disable prettier/prettier */
import { Selector, t } from 'testcafe'
import { createRegExp } from '../../helpers/regExp-helper'

class AddressBookPage {
    addAddressButton: Selector
    address: Selector
    smartyStreetsSuggestionHeader: Selector

	constructor() {
        this.addAddressButton = Selector('#add-address-button')
        this.address = Selector('ocm-address-card').nth(1)
        this.smartyStreetsSuggestionHeader = Selector('address-suggestion')
	}

	async clickAddAddressButton() {
        await t.click(this.addAddressButton)
    }

    async addressExists(address: string) {
        await this.address()
        const shadowAddressDataExists = Selector(() => document.querySelectorAll('ocm-address-card')[1]
            .shadowRoot.querySelector('.address-detail'))
            .withText(createRegExp(address)).exists
        return shadowAddressDataExists
    }
}

export default new AddressBookPage()
