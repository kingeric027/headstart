import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { SafeUrl } from '@angular/platform-browser';
import { KitService } from '@app-seller/kits/kits.service';
import { getProductMainImageUrlOrPlaceholder } from '@app-seller/products/product-image.helper';
import { MarketplaceKitProduct } from '@app-seller/shared/services/middleware-api/middleware-kit.service';
import { ListAddress } from '@ordercloud/angular-sdk';

@Component({
    selector: 'app-kits-edit',
    templateUrl: './kits-edit.component.html',
    styleUrls: ['./kits-edit.component.scss'],
})
export class KitsEditComponent implements OnInit {
    kitProductEditable: any;
    kitProductStatic: any
    @Input()
    kitProductForm: FormGroup;
    @Input()
    set orderCloudProduct(product: MarketplaceKitProduct) {
        if (product) this.kitProductEditable = product;
        this.kitProductEditable = this.kitService.emptyResource;
        this.kitProductStatic = this.kitService.emptyResource;
    }
    @Input() readonly: boolean;
    @Input()
    filterConfig;
    @Output()
    updateResource = new EventEmitter<any>();
    @Input()
    addresses: ListAddress;
    @Input()
    isCreatingNew: boolean;
    constructor(
        private kitService: KitService,
    ) { }

    ngOnInit() { }

    getProductPreviewImage(): string | SafeUrl {
        return getProductMainImageUrlOrPlaceholder(this.kitProductEditable?.Product);
    }
}
