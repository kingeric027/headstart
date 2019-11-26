import { Component, Input, OnChanges, OnInit, ViewChild } from '@angular/core';
import { OCMComponent } from '../../base-component';
import { ListSupplier, Supplier } from '@ordercloud/angular-sdk';
import { faTimes, faFilter } from '@fortawesome/free-solid-svg-icons';
import { FormControl, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile } from 'rxjs/operators';
import { SupplierFilters, SupplierCategoryConfig } from 'marketplace';

@Component({
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss'],
})
export class OCMSupplierList extends OCMComponent implements OnInit, OnChanges {
  @Input() suppliers: ListSupplier;
  _supplierCategoryConfig: SupplierCategoryConfig;
  @Input() set supplierCategoryConfig(value: SupplierCategoryConfig) {
    this._supplierCategoryConfig = value;
    this.setForm();
    this.context.supplierFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }
  @ViewChild('popover', { static: false }) public popover: NgbPopover;
  searchTermForSuppliers: string = null;
  filterForm: FormGroup;
  faTimes = faTimes;
  faFilter = faFilter;
  serviceCategory = '';
  activeFilters = {};
  activeFilterCount = 0;

  ngOnContextSet() {
    this.activeFilterCount = Object.keys(this.context.supplierFilters.activeFiltersSubject.value.activeFilters).length;
  }

  ngOnInit() {}
  ngOnChanges() {
    this.activeFilterCount = Object.keys(this.context.supplierFilters.activeFiltersSubject.value.activeFilters).length;
  }

  setForm() {
    const formGroup = {};
    this._supplierCategoryConfig.Filters.forEach(filter => {
      formGroup[filter.Path] = new FormControl('');
    });
    this.filterForm = new FormGroup(formGroup);
  }

  private handleFiltersChange = (filters: SupplierFilters) => {
    if (filters.activeFilters) {
      this._supplierCategoryConfig.Filters.forEach(filter => {
        this.filterForm.controls[filter.Path].setValue(filters.activeFilters[filter.Path]);
      });
    }
  }

  searchSuppliers(searchStr: string) {
    this.searchTermForSuppliers = searchStr;
    this.context.supplierFilters.searchBy(searchStr);
  }

  changePage(page: number): void {
    this.context.supplierFilters.toPage(page);
    window.scrollTo(0, null);
  }

  applyFilters() {
    const filters = {};
    this._supplierCategoryConfig.Filters.forEach(filter => {
      filters[filter.Path] = this.filterForm.value[filter.Path];
    });
    this.context.supplierFilters.filterByFields(filters);
    this.popover.close();
  }

  clearFilters() {
    this.context.supplierFilters.clearAllFilters();
  }

  openPopover() {
    this.popover.open();
  }

  closePopover() {
    this.popover.close();
  }
}
