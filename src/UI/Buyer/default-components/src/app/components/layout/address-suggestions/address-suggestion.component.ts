import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { BuyerAddress } from 'ordercloud-javascript-sdk';
import { faSquare, faCheckSquare } from '@fortawesome/free-regular-svg-icons';

@Component({
  templateUrl: './address-suggestion.component.html',
  styleUrls: ['./address-suggestion.component.scss'],
})
export class OCMAddressSuggestion implements OnInit {
  @Input() suggestedAddresses: BuyerAddress;
  @Output() selectedAddress = new EventEmitter<BuyerAddress>();
  activeAddress: BuyerAddress;
  faSquare = faSquare;
  faCheckSquare = faCheckSquare;
  constructor(public context: ShopperContextService) {}

  ngOnInit(): void {}

  setActiveAddress(address: BuyerAddress): void {
    this.activeAddress = address;
    this.selectedAddress.emit(address);
  }
}
