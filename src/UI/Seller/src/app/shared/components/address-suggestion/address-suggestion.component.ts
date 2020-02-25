import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ListBuyerAddress, BuyerAddress } from '@ordercloud/angular-sdk';

@Component({
    templateUrl: './address-suggestion.component.html',
    styleUrls: ['./address-suggestion.component.scss'],
})

export class OCMAddressSuggestion {
    @Input() suggestedAddresses: ListBuyerAddress;
    @Output() selectedAddress = new EventEmitter<BuyerAddress>();
    activeAddress: BuyerAddress;
    constructor() { }

    setActiveAddress(address) {
        this.activeAddress = address;
        this.selectedAddress.emit(address);
    }
}
