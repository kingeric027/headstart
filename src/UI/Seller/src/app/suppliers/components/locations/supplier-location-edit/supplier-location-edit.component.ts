import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ListAddress } from '@ordercloud/angular-sdk';
@Component({
    selector: 'app-supplier-location-edit',
    templateUrl: './supplier-location-edit.component.html',
    styleUrls: ['./supplier-location-edit.component.scss'],
})
export class SupplierLocationEditComponent {
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

    updateResourceFromEvent(event: any, field: string): void {
        this.updateResource.emit({ value: event.target.value, field });
    }

    handleAddressSelect(address) {
        this.selectAddress.emit(address);
    }
}
