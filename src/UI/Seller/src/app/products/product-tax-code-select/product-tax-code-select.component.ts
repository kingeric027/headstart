import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MarketPlaceProductTaxCode, MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ListResource } from '@app-seller/shared/services/resource-crud/resource-crud.types';

@Component({
  selector: 'product-tax-code-select-component',
  templateUrl: './product-tax-code-select.component.html',
  styleUrls: ['./product-tax-code-select.component.scss'],
})
export class ProductTaxCodeSelect {
  @Input()
  productForm: FormGroup;
  @Input()
  marketPlaceProductEditable: MarketPlaceProduct;
  @Input()
  taxCodes: ListResource<MarketPlaceProductTaxCode>;
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

  handleSelectTaxCode(taxCodeSelection: MarketPlaceProductTaxCode): void {
    let event = {
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
