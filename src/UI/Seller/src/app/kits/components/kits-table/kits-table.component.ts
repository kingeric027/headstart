import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { KitService } from '@app-seller/kits/kits.service';
import { MarketplaceKitProduct } from '@app-seller/shared/services/middleware-api/middleware-kit.service';

@Component({
    selector: 'app-kits-table',
    templateUrl: './kits-table.component.html',
    styleUrls: ['./kits-table.component.scss'],
})
export class KitsTableComponent extends ResourceCrudComponent<MarketplaceKitProduct> {
    constructor(
        kitService: KitService,
        changeDetectorRef: ChangeDetectorRef,
        router: Router,
        activatedRoute: ActivatedRoute,
        ngZone: NgZone
    ) {
        super(changeDetectorRef, kitService, router, activatedRoute, ngZone);
    }
}