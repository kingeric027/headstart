import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ListAddress } from '@ordercloud/angular-sdk';
@Component({
    selector: 'app-buyer-location-edit',
    templateUrl: './buyer-location-edit.component.html',
    styleUrls: ['./buyer-location-edit.component.scss'],
})
export class BuyerLocationEditComponent {
    @Input()
    resourceForm: FormGroup;
    @Input()
    filterConfig;
    @Input()
    suggestedAddresses: ListAddress;
    @Output()
    updateResource = new EventEmitter<any>();
    updateResourceFromEvent(event: any, field: string): void {
        this.updateResource.emit({ value: event.target.value, field });
    }
}
