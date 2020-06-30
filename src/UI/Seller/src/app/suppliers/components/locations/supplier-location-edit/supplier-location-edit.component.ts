import { Component, EventEmitter, Input, Output, OnChanges } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ListAddress, OcProductService } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/products/product.service';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'app-supplier-location-edit',
    templateUrl: './supplier-location-edit.component.html',
    styleUrls: ['./supplier-location-edit.component.scss'],
})
export class SupplierLocationEditComponent implements OnChanges {
    locationHasNoProducts: boolean;
    @Input()
    resourceForm: FormGroup;
    @Input()
    filterConfig;
    @Input()
    suggestedAddresses: ListAddress;
    @Output()
    updateResource = new EventEmitter<any>();
    @Output()
    selectAddress = new EventEmitter<any>();
    @Output()
    canDelete = new EventEmitter<boolean>();

    constructor(private middleware: MiddlewareAPIService,
        private activatedRoute: ActivatedRoute,) {}

    ngOnChanges() {
        this.setLocationID();
    }

    private setLocationID(): void {
        const url = window.location.href;
        const splitUrl = url.split('/');
        const endUrl = splitUrl[splitUrl.length - 1];
        const urlParams = this.activatedRoute.snapshot.params;
        if(urlParams.locationID) {
            this.determineIfDeletable(urlParams.locationID)
        }
    }

    private async determineIfDeletable(locationID: string): Promise<void> {
        const hasNoProducts = await this.middleware.isLocationDeletable(locationID)
        this.canDelete.emit(hasNoProducts);
    }

    updateResourceFromEvent(event: any, field: string): void {
        this.updateResource.emit({ value: event.target.value, field });
    }

    handleAddressSelect(address) {
        this.selectAddress.emit(address);
    }
}
