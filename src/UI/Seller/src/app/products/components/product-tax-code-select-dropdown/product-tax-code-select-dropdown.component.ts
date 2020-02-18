import { Component, Input, Output, EventEmitter, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import {
  MarketPlaceProductTaxCode,
  SuperMarketplaceProduct,
} from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ListPage } from '@app-seller/shared/services/middleware-api/listPage.interface';

@Component({
  selector: 'product-tax-code-select-dropdown',
  templateUrl: './product-tax-code-select-dropdown.component.html',
  styleUrls: ['./product-tax-code-select-dropdown.component.scss'],
})
export class ProductTaxCodeSelectDropdown implements OnChanges {
  @Input()
  taxCodes: ListPage<MarketPlaceProductTaxCode>;
  @Input()
  superMarketplaceProductEditable: SuperMarketplaceProduct;

  @Output()
  taxCodesSearched = new EventEmitter<any>();
  @Output()
  onScrollEnd = new EventEmitter<string>();
  @Output()
  onSelectTaxCode = new EventEmitter<MarketPlaceProductTaxCode>();

  @ViewChild('popover', { static: false })
  public popover: NgbPopover;
  searchTerm = '';
  productTaxCodeSelectDropdownHeight = 250;

  ngOnChanges(changes: SimpleChanges) {
    if (
      changes &&
      changes.superMarketplaceProductEditable &&
      changes.superMarketplaceProductEditable.previousValue &&
      changes.superMarketplaceProductEditable.currentValue &&
      changes.superMarketplaceProductEditable.previousValue.Product.ID !==
        changes.superMarketplaceProductEditable.currentValue.Product.ID
    ) {
      this.searchTerm = '';
    }
  }

  searchedTaxCodes(searchText: any) {
    this.searchTerm = searchText;
    this.taxCodesSearched.emit(this.searchTerm);
  }

  selectTaxCode(taxCode: MarketPlaceProductTaxCode) {
    // To clear the tax code search term when a selection is made - to refresh the list back to the starting state.
    if (this.searchTerm !== '') {
      this.searchTerm = '';
      this.taxCodesSearched.emit(this.searchTerm);
    }
    this.onSelectTaxCode.emit(taxCode);
  }

  handleScrollEnd(event) {
    if (event.target.classList.value.includes('active')) {
      this.onScrollEnd.emit(this.searchTerm);
    }
  }
}
