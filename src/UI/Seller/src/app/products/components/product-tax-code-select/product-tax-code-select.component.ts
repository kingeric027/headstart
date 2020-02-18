import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';
import {
  MarketPlaceProductTaxCode,
  SuperMarketplaceProduct,
} from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ListPage } from '@app-seller/shared/services/middleware-api/listPage.interface';

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
  taxCodes: ListPage<MarketPlaceProductTaxCode>;
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
