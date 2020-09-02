import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { StorefrontsService } from '../../storefronts/storefronts.service';
import { JDocument } from '@ordercloud/headstart-sdk';
import { StorefrontPageService } from '../storefront-page.service';
@Component({
  selector: 'app-storefront-page-table',
  templateUrl: './storefront-page-table.component.html',
  styleUrls: ['./storefront-page-table.component.scss'],
})
export class StorefrontPageTableComponent extends ResourceCrudComponent<JDocument> {
  route = 'pages';
  parentResourceID: string;
  constructor(
    private storefrontPageService: StorefrontPageService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    private storefrontsService: StorefrontsService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, storefrontPageService, router, activatedRoute, ngZone);
    console.log('resource in selection', this.resourceInSelection);
  }
  resourceOrEmptyDoc(): JDocument {
    if (!this.resourceInSelection.ID) {
      console.log(this.storefrontPageService.emptyResource);
      return this.storefrontPageService.emptyResource;
    } else {
      console.log(this.resourceInSelection);
      return this.resourceInSelection;
    }
  }
}
