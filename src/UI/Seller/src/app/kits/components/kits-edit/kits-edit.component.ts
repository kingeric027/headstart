import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { KitService } from '@app-seller/kits/kits.service';

@Component({
    selector: 'app-kits-edit',
    templateUrl: './kits-edit.component.html',
    styleUrls: ['./kits-edit.component.scss'],
})
export class KitsEditComponent {
    constructor(
        changeDetectorRef: ChangeDetectorRef,
        router: Router,
        activatedRoute: ActivatedRoute,
        ngZone: NgZone
    ) { }
}
