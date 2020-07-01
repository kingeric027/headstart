import { FormControl, Validators } from '@angular/forms';
import { MarketplaceLineItem } from 'marketplace-javascript-sdk';

export class LineItemForm {
    id = new FormControl();
    selected = new FormControl();
    quantityToReturn = new FormControl();
    returnReason = new FormControl();
    lineItem: MarketplaceLineItem;

    constructor(lineItem: MarketplaceLineItem) {
        if (lineItem.ID) this.id.setValue(lineItem.ID);
        this.lineItem = lineItem;
        this.selected.setValue(false);
        if (lineItem.Quantity === lineItem.xp?.LineItemReturnInfo?.QuantityToReturn || lineItem.QuantityShipped !== lineItem.Quantity) this.selected.disable();
        debugger;
        this.quantityToReturn.disable();
        this.quantityToReturn.setValidators([
            Validators.required,
            Validators.min(1),
            Validators.max(lineItem.Quantity - (lineItem.xp?.LineItemReturnInfo?.QuantityToReturn || 0))
        ]);
        this.returnReason.disable();
        this.returnReason.setValidators([Validators.required]);
    }
} 