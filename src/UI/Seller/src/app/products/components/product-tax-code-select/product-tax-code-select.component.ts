import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { SuperMarketplaceProduct, ListPage } from 'marketplace-javascript-sdk';
import TaxCodes from 'marketplace-javascript-sdk/dist/api/TaxCodes';

@Component({
  selector: 'product-tax-code-select-component',
  templateUrl: './product-tax-code-select.component.html',
  styleUrls: ['./product-tax-code-select.component.scss'],
})
export class ProductTaxCodeSelect {
  @Input()
  productForm: FormGroup;
  @Input()
  superMarketplaceProductEditable: SuperMarketplaceProduct;
  @Input()
  taxCodes: ListPage<TaxCodes>;
  @Output()
  handleTaxCodeCategorySelection = new EventEmitter<any>();
  @Output()
  handleTaxCodeSelection = new EventEmitter<any>();
  @Output()
  handleTaxCodesSearched = new EventEmitter<string>();
  @Output()
  onScrollEnd = new EventEmitter<string>();

  onTaxCodeCategorySelection(event): void {
    this.handleTaxCodeCategorySelection.emit(event);
  }

  handleSelectTaxCode(taxCodeSelection: TaxCodes): void {
    const event = {
      target: {
        value: taxCodeSelection,
      },
    };
    this.handleTaxCodeSelection.emit(event);
  }

  onTaxCodesSearched(searchTerm: string) {
    this.handleTaxCodesSearched.emit(searchTerm);
  }

  handleScrollEnd(searchTerm: string) {
    this.onScrollEnd.emit(searchTerm);
  }
}
