import { Input, Component, OnInit } from '@angular/core';
import { Supplier } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './supplier-card.component.html',
  styleUrls: ['./supplier-card.component.scss'],
})
export class OCMSupplierCard {
  @Input() supplier: Supplier = {};
  
  constructor(private context: ShopperContextService) {}

  shopSupplier(supplier: Supplier) {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.Name.toLocaleLowerCase() } });
  }
}
