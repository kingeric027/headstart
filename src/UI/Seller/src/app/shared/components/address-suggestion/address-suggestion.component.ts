import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { ListBuyerAddress, BuyerAddress } from '@ordercloud/angular-sdk';

@Component({
    selector: 'address-suggestion',
    templateUrl: './address-suggestion.component.html',
    styleUrls: ['./address-suggestion.component.scss'],
})

export class AddressSuggestionComponent {
    @Input()
    suggestedAddresses: ListBuyerAddress;
    @Output()
    selectedAddress = new EventEmitter<BuyerAddress>();
    activeAddress: BuyerAddress;
    constructor() { }

    setActiveAddress(address) {
        this.activeAddress = address;
        this.selectedAddress.emit(address);
    }
}
