import { Input } from '@angular/core';
import { IShopperContext } from 'marketplace';

export abstract class OCMComponent {
  // todo: the issue is that ngOnInit fires before inputs are ready. come up with a better way to do this.
  private privateContext: IShopperContext;

  @Input() set context(value: IShopperContext) {
    this.privateContext = value;
    this.ngOnContextSet();
  }

  get context() {
    return this.privateContext;
  }

  abstract ngOnContextSet(): void;
}
