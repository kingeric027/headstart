import { Input } from '@angular/core';
import { IShopperContext } from 'shopper-context-interface';

export class OCMComponent {
    // todo: the issue is that ngOnInit fires before inputs are ready. come up with a better way to do this.
    observersSet: boolean;
    @Input() context: IShopperContext;
}
