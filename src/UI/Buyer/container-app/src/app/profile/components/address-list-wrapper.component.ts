import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ListAddress } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';

@Component({
  template: `
    <ocm-address-list [addresses]="addresses" [context]="context"></ocm-address-list>
  `,
})
export class MeAddressListWrapperComponent implements OnInit {
  addresses: ListAddress;

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit() {
    this.addresses = this.activatedRoute.snapshot.data.addresses;
  }
}
