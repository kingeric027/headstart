import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-location-management [locationID]="locationID"></ocm-location-management>
  `,
})
export class LocationManagementWrapperComponent {
  locationID: string;
  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {
    this.locationID = this.activatedRoute.snapshot.params.locationID;
  }
}
