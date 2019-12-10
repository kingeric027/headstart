import { Component, OnInit } from '@angular/core';
import { ListAddress } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-address-list [addresses]="addresses"></ocm-address-list>
  `,
})
export class AddressListWrapperComponent implements OnInit {
  addresses: ListAddress;

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit() {
    this.addresses = this.activatedRoute.snapshot.data.addresses;
  }
}
