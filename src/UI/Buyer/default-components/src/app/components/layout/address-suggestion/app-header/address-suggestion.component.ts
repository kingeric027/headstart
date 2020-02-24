import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BuyerAddress, ListBuyerAddress, ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './address-suggestion.component.html',
  styleUrls: ['./address-suggestion.component.scss'],
})

export class OCMAddressSuggestion implements OnInit {
  @Input() suggestedAddresses: ListBuyerAddress;
  @Output() selectedAddress = new EventEmitter<BuyerAddress>();
  activeAddress: BuyerAddress;
  constructor(public context: ShopperContextService) { }

  ngOnInit() {
  }

  setActiveAddress(address) {
    this.activeAddress = address;
    this.selectedAddress.emit(address);
  }
}
