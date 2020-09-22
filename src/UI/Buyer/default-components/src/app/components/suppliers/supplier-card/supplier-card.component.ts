import { Input, Component } from '@angular/core';
import { Supplier } from 'ordercloud-javascript-sdk';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './supplier-card.component.html',
  styleUrls: ['./supplier-card.component.scss'],
})
export class OCMSupplierCard {
  _supplier: Supplier;
  @Input() set supplier(s: Supplier) {
    this._supplier = s;
    this.logoUrl = `${this.context.appSettings.middlewareUrl}/assets/${this.context.appSettings.sellerID}/Suppliers/${s.ID}/thumbnail?size=s`;
  }
  logoUrl: string = '';
  constructor(private context: ShopperContextService) {}

  shopSupplier(supplier: Supplier): void {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.Name.toLocaleLowerCase() } });
  }
}
