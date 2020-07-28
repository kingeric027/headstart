import { FormControl, Validators } from '@angular/forms';
import { MarketplaceLineItem } from 'marketplace-javascript-sdk';
import { lineItemHasBeenShipped } from 'src/app/services/orderType.helper';

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
        if (lineItem.Quantity === lineItem.xp?.LineItemReturnInfo?.QuantityToReturn || 
            lineItem.Quantity === lineItem.xp?.LineItemCancelInfo?.QuantityToCancel){
            this.selected.disable();
        } 
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