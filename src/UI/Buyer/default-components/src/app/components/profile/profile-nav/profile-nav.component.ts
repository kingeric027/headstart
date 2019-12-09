import { Component, ViewEncapsulation } from '@angular/core';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './profile-nav.component.html',
  styleUrls: ['./profile-nav.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class OCMProfileNav {
  constructor(public context: ShopperContextService) {}
}


