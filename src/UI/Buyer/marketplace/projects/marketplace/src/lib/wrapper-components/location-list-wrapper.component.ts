import { Component, OnInit } from '@angular/core';
import { ListAddress } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-location-list [addresses]="addresses"></ocm-location-list>
  `,
})
export class LocationListWrapperComponent implements OnInit {
  addresses: ListAddress;

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit() {
    this.addresses = this.activatedRoute.snapshot.data.addresses;
  }
}
