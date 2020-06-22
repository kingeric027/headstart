import { Component, Input, OnChanges, ViewChild, OnDestroy } from '@angular/core';
import { Supplier, ListPage } from 'ordercloud-javascript-sdk';
import { faTimes, faFilter } from '@fortawesome/free-solid-svg-icons';
import { FormControl, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile } from 'rxjs/operators';
import { SupplierFilters, ShopperContextService } from 'marketplace';
import { SupplierCategoryConfig } from 'marketplace-javascript-sdk';

@Component({
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss'],
})
export class OCMSupplierList implements OnChanges, OnDestroy {
  @Input() suppliers: ListPage<Supplier>;
  _supplierCategoryConfig: SupplierCategoryConfig;
  @ViewChild('popover', { static: false }) public popover: NgbPopover;
  alive = true;
  searchTermForSuppliers: string = null;
  filterForm: FormGroup;
  faTimes = faTimes;
  faFilter = faFilter;
  serviceCategory = '';
  activeFilters = {};
  activeFilterCount = 0;

  constructor(private context: ShopperContextService) {}

  @Input() set supplierCategoryConfig(value: SupplierCategoryConfig) {
    this._supplierCategoryConfig = value;
    this.setForm();
    this.context.supplierFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  ngOnChanges(): void {
    this.activeFilterCount = Object.keys(this.context.supplierFilters.activeFiltersSubject.value.activeFilters).length;
  }

  setForm(): void {
    const formGroup = {};
    this._supplierCategoryConfig.Filters.forEach(filter => {
      formGroup[filter.Path] = new FormControl('');
    });
    this.filterForm = new FormGroup(formGroup);
  }

  searchSuppliers(searchStr: string): void {
    this.searchTermForSuppliers = searchStr;
    this.context.supplierFilters.searchBy(searchStr);
  }

  changePage(page: number): void {
    this.context.supplierFilters.toPage(page);
    window.scrollTo(0, null);
  }

  applyFilters(): void {
    const filters = {};
    this._supplierCategoryConfig.Filters.forEach(filter => {
      filters[filter.Path] = this.filterForm.value[filter.Path];
    });
    this.context.supplierFilters.filterByFields(filters);
    this.popover.close();
  }

  clearFilters(): void {
    this.context.supplierFilters.clearAllFilters();
  }

  openPopover(): void {
    this.popover.open();
  }

  closePopover(): void {
    this.popover.close();
  }

  ngOnDestroy(): void {
    this.alive = false;
  }

  private handleFiltersChange = (filters: SupplierFilters): void => {
    if (filters.activeFilters) {
      this.searchTermForSuppliers = filters.search || '';
      this._supplierCategoryConfig.Filters.forEach(filter => {
        this.filterForm.controls[filter.Path].setValue(filters.activeFilters[filter.Path]);
      });
    }
  };
}
