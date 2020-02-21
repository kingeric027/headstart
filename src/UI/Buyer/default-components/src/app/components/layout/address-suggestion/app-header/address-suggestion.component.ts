import { Component, OnInit, Input } from '@angular/core';
import { ShopperContextService, ListAddress } from 'marketplace';

@Component({
  templateUrl: './address-suggestion.component.html',
  styleUrls: ['./address-suggestion.component.scss'],
})

export class OCMAddressSuggestion implements OnInit {
  @Input() suggestedAddresses: ListAddress;
  constructor(public context: ShopperContextService) { }

  ngOnInit(): void {
  }
}
