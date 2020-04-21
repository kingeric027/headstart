import { Component, EventEmitter, Input, Output, OnChanges } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ListAddress, OcProductService } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/products/product.service';
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

    constructor(private ocProductService: OcProductService) {}

    ngOnChanges() {
        this.setLocationID();
    }

    private setLocationID(): void {
        const url = window.location.href;
        const splitUrl = url.split('/');
        const endUrl = splitUrl[splitUrl.length - 1];
        this.determineIfDeletable(endUrl);
    }

    private async determineIfDeletable(locationID: string): Promise<void> {
        const productList = await this.ocProductService.List({pageSize: 1, filters: {ShipFromAddressID: locationID}}).toPromise();
        this.locationHasNoProducts = productList.Items.length ? false : true;
        this.canDelete.emit(this.locationHasNoProducts);
    }

    updateResourceFromEvent(event: any, field: string): void {
        this.updateResource.emit({ value: event.target.value, field });
    }

    handleAddressSelect(address) {
        this.selectAddress.emit(address);
    }
}
