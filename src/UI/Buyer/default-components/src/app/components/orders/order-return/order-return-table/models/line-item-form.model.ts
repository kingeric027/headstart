import { FormControl, Validators } from '@angular/forms';
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';

export class LineItemForm {
    id = new FormControl();
    selected = new FormControl();
    quantityToReturn = new FormControl();
    returnReason = new FormControl();
    lineItem: MarketplaceLineItem;

    constructor(lineItem: MarketplaceLineItem, action: string) {
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
            Validators.max( 
                action === 'return' ?
                (lineItem.QuantityShipped - (lineItem.xp?.LineItemReturnInfo?.QuantityToReturn || 0)) : 
                (lineItem.Quantity - lineItem.QuantityShipped - (lineItem.xp?.LineItemCancelInfo?.QuantityToCancel || 0))
            )
        ]);
        this.returnReason.disable();
        this.returnReason.setValidators([Validators.required]);
    }
} 