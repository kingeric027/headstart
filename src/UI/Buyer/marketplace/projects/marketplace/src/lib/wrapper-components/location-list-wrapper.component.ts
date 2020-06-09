import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { BuyerLocationWithCert } from '../shopper-context';

@Component({
  template: `
    <ocm-location-list [locations]="locations"></ocm-location-list>
  `,
})
export class LocationListWrapperComponent implements OnInit {
  locations: BuyerLocationWithCert[];

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.locations = this.activatedRoute.snapshot.data.locations;
  }
}
