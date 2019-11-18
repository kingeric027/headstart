import { Component, Input, OnChanges, OnInit, ViewChild } from '@angular/core';
import { OCMComponent } from '../base-component';
import { ListSupplier, Supplier } from '@ordercloud/angular-sdk';
import { faTimes, faFilter } from '@fortawesome/free-solid-svg-icons';
import { FormControl, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile } from 'rxjs/operators';
import { SupplierFilters } from 'marketplace';
// import SupplierCategoryConfig from './SEB_SupplierCategoryConfig';

@Component({
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss'],
})
export class OCMSupplierList extends OCMComponent implements OnInit, OnChanges {
  @Input() suppliers: ListSupplier;
  @Input() supplierCategoryConfig: any;
  @ViewChild('popover', { static: false }) public popover: NgbPopover;
  searchTermForSuppliers: string = null;
  filterForm: FormGroup;
  faTimes = faTimes;
  faFilter = faFilter;
  serviceCategory = '';
  activeFilters = {};
  serviceCategoryOptions = [];
  vendorLevelOptions = [];
  activeFilterCount: number = 0;

  ngOnContextSet() {
    this.activeFilterCount = Object.keys(this.context.supplierFilters.activeFiltersSubject.value.activeFilters).length;
    this.context.supplierFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  ngOnInit() {
    this.setForm();
    // this.serviceCategoryOptions = SupplierCategoryConfig.ServiceCategories;
    // this.vendorLevelOptions = SupplierCategoryConfig.VendorLevels;
  }
  showSCC() {
    console.log(this.supplierCategoryConfig);
  }
  ngOnChanges() {
    this.activeFilterCount = Object.keys(this.context.supplierFilters.activeFiltersSubject.value.activeFilters).length;
  }

  setForm() {
    this.filterForm = new FormGroup({
      serviceCategory: new FormControl(''),
      vendorLevel: new FormControl(''),
    });
  }

  private handleFiltersChange = (filters: SupplierFilters) => {
    if (filters.activeFilters) {
      this.filterForm.controls.serviceCategory.setValue(filters.activeFilters['Categories.ServiceCategory']);
      this.filterForm.controls.vendorLevel.setValue(filters.activeFilters['Categories.VendorLevel']);
    }
  };

  searchSuppliers(searchStr: string) {
    this.searchTermForSuppliers = searchStr;
    this.context.supplierFilters.searchBy(searchStr);
  }

  changePage(page: number): void {
    this.context.supplierFilters.toPage(page);
    window.scrollTo(0, null);
  }

  applyFilters() {
    const { serviceCategory, vendorLevel } = this.filterForm.value;
    const filters = {};
    if (serviceCategory) filters['Categories.ServiceCategory'] = serviceCategory;
    if (vendorLevel) filters['Categories.VendorLevel'] = vendorLevel;
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
