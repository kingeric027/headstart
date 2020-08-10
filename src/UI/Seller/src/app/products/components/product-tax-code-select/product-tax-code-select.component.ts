import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { SuperMarketplaceProduct, ListPage, TaxProperties } from '@ordercloud/headstart-sdk';
import TaxCodes from 'marketplace-javascript-sdk/dist/api/TaxCodes';
import { faTimesCircle, faCheckCircle } from '@fortawesome/free-solid-svg-icons';

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
  handleIsResale = new EventEmitter<boolean>();
  @Output()
  onScrollEnd = new EventEmitter<string>();
  @Input()
  readonly = false;
  @Input()
  isRequired: boolean;
  @Input()
  isCreatingNew: boolean;
  faTimesCircle = faTimesCircle;
  faCheckCircle = faCheckCircle;

  onTaxCodeCategorySelection(event): void {
    this.handleTaxCodeCategorySelection.emit(event);
  }

  handleIsResaleInput(event: boolean): void {
    return this.handleIsResale.emit(event);
  }

  handleSelectTaxCode(taxCodeSelection: TaxProperties): void {
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

  taxSelectionsValid(): boolean {
    return (
      this.isCreatingNew &&
      this.isRequired &&
      this.productForm.controls['TaxCodeCategory'].valid &&
      this.productForm.controls['TaxCode'].valid
    );
  }
}
