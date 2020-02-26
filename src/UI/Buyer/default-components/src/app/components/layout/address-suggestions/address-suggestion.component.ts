import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BuyerAddress, ListBuyerAddress, ShopperContextService } from 'marketplace';
import { faSquare, faCheckSquare } from '@fortawesome/free-regular-svg-icons';

@Component({
  templateUrl: './address-suggestion.component.html',
  styleUrls: ['./address-suggestion.component.scss'],
})

export class OCMAddressSuggestion implements OnInit {
  @Input() suggestedAddresses: ListBuyerAddress;
  @Output() selectedAddress = new EventEmitter<BuyerAddress>();
  activeAddress: BuyerAddress;
  faSquare = faSquare;
  faCheckSquare = faCheckSquare;
  constructor(public context: ShopperContextService) { }

  ngOnInit() {
  }

  setActiveAddress(address) {
    this.activeAddress = address;
    this.selectedAddress.emit(address);
  }
}
