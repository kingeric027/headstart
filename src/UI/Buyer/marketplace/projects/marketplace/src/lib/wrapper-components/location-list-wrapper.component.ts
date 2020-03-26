import { Component, OnInit } from '@angular/core';
import { ListAddress } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { BuyerCreditCard } from 'marketplace-javascript-sdk';
import { BuyerLocationWithCert } from '../shopper-context';

@Component({
  template: `
    <ocm-location-list [locations]="locations"></ocm-location-list>
  `,
})
export class LocationListWrapperComponent implements OnInit {
  locations: BuyerLocationWithCert[];

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit() {
    this.locations = this.activatedRoute.snapshot.data.locations;
  }
}
