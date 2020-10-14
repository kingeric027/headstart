import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { SuperMarketplaceProduct, MarketplaceKitProduct } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../services/temp-sdk/temp-sdk.service';

@Component({
  template: `
    <ocm-product-chili-configuration [template]="template"> </ocm-product-chili-configuration>
  `,
})
export class ProductChiliConfigurationWrapperComponent implements OnInit {
  template: any;
  constructor(private activatedRoute: ActivatedRoute, protected currentUser: CurrentUserService, private tempSdk: TempSdk) { }

  async ngOnInit(): Promise<void> {
    this.template = await this.activatedRoute.snapshot.data.template;
  }
}
