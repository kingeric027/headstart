import { Input, Component } from '@angular/core';
import { Supplier } from 'ordercloud-javascript-sdk';
import { ShopperContextService } from 'marketplace';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

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
  constructor(private context: ShopperContextService, private domSanitizer: DomSanitizer) {}

  shopSupplier(supplier: Supplier): void {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.Name.toLocaleLowerCase() } });
  }
  getSupplierDescription(): SafeHtml {
    if (this._supplier && this._supplier?.xp) {
      //In order to have css styles show from the Description (since it's html in string form),
      //we need to tell Angular to bypass sanitation of the html.
      return this.domSanitizer.bypassSecurityTrustHtml(this._supplier.xp.Description);
    }
  }
}
