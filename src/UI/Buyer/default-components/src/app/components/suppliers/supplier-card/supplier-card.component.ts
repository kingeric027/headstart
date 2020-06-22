import { Input, Component } from '@angular/core';
import { Supplier } from 'ordercloud-javascript-sdk';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './supplier-card.component.html',
  styleUrls: ['./supplier-card.component.scss'],
})
export class OCMSupplierCard {
  @Input() supplier = {} as Supplier;

  constructor(private context: ShopperContextService) {}

  shopSupplier(supplier: Supplier): void {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.Name.toLocaleLowerCase() } });
  }
}
