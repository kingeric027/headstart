import { Component, ChangeDetectorRef, NgZone, AfterViewChecked } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { StorefrontsService } from '../../storefronts/storefronts.service';
import { JDocument } from '@ordercloud/headstart-sdk';
import { StorefrontPageService } from '../storefront-page.service';
import { getPsHeight } from '@app-seller/shared/services/dom.helper';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
@Component({
  selector: 'app-storefront-page-table',
  templateUrl: './storefront-page-table.component.html',
  styleUrls: ['./storefront-page-table.component.scss'],
})
export class StorefrontPageTableComponent extends ResourceCrudComponent<JDocument> implements AfterViewChecked {
  route = 'pages';
  resourceHeight: number;
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
    this.redirectToFirstParentIfNeeded();
  }

  ngAfterViewChecked(): void {
    this.resourceHeight = getPsHeight('additional-item-resource');
  }

  private async redirectToFirstParentIfNeeded() {
    if (this.storefrontsService) {
      const parentResourceID = await this.storefrontsService.getParentResourceID();
      if (parentResourceID === REDIRECT_TO_FIRST_PARENT) {
        await this.storefrontsService.listResources();
        this.ocService.selectParentResource(this.storefrontsService.resourceSubject.value.Items[0]);
      }
    }
  }

  resourceOrEmptyDoc(): JDocument {
    if (!this.resourceInSelection.ID) {
      // console.log(this.storefrontPageService.emptyResource);
      return this.storefrontPageService.emptyResource;
    } else {
      // console.log(this.resourceInSelection);
      return this.resourceInSelection;
    }
  }

  onPageSaved(event): void {
    console.log(event);
    // HeadStartSDK.Documents.SaveAssignment('cms-page-schema', {
    //   DocumentID: event.ID,
    //   ParentResourceID: null,
    //   ResourceID: this.parentResourceID,
    //   ResourceType: 'ApiClients',
    // });
    this.selectResource(event);
  }

  back(): void {
    this.selectResource({});
  }
}
