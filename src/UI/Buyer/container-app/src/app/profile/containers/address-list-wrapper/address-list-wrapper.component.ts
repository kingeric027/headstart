import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ListAddress } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'profile-address-list-wrapper',
  templateUrl: './address-list-wrapper.component.html',
  styleUrls: ['./address-list-wrapper.component.scss'],
})
export class MeAddressListWrapperComponent implements OnInit {
  addresses: ListAddress;

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit() {
    this.addresses = this.activatedRoute.snapshot.data.addresses;
  }
}
