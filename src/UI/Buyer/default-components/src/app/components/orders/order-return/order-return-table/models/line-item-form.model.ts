import { FormControl, Validators } from '@angular/forms';
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';

export class LineItemForm {
    id = new FormControl();
    selected = new FormControl();
    quantityToReturnOrCancel = new FormControl();
    returnReason = new FormControl();
    lineItem: MarketplaceLineItem;

    constructor(lineItem: MarketplaceLineItem, action: string) {
        if (lineItem.ID) this.id.setValue(lineItem.ID);
        this.lineItem = lineItem;
        this.selected.setValue(false);
        if (lineItem.Quantity === lineItem.xp?.LineItemReturnInfo?.QuantityToReturn || 
            lineItem.Quantity === (lineItem.xp as any)?.LineItemCancelInfo?.QuantityToCancel){
            this.selected.disable();
        } 
        this.quantityToReturnOrCancel.disable();
        this.quantityToReturnOrCancel.setValidators([
            Validators.required,
            Validators.min(1),
            Validators.max( 
                action === 'return' ?
                (lineItem.QuantityShipped - (lineItem.xp?.LineItemReturnInfo?.QuantityToReturn || 0)) : 
                (lineItem.Quantity - lineItem.QuantityShipped - ((lineItem.xp as any)?.LineItemCancelInfo?.QuantityToCancel || 0))
            )
        ]);
        this.returnReason.disable();
        this.returnReason.setValidators([Validators.required]);
    }
} 