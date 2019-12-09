import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { takeWhile } from 'rxjs/operators';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './sort-suppliers.component.html',
  styleUrls: ['./sort-suppliers.component.scss'],
})
export class OCMSupplierSort implements OnInit, OnDestroy {
  alive = true;
  form: FormGroup;
  options = [{ value: 'Name', label: 'A to Z' }, { value: '!Name', label: 'Z to A' }];
  @Output() closePopoverEvent = new EventEmitter();

  constructor(private context: ShopperContextService) {}

  ngOnInit() {
    this.form = new FormGroup({ sortBy: new FormControl(null) });
    this.context.supplierFilters.activeFiltersSubject.pipe(takeWhile(() => this.alive)).subscribe(filters => {
      this.setForm(filters.sortBy);
    });
  }

  private setForm(sortBy: string) {
    sortBy = sortBy || null;
    this.form.setValue({ sortBy });
  }

  sortStrategyChanged() {
    const sortValue = this.form.get('sortBy').value;
    this.context.supplierFilters.sortBy(sortValue);
    this.closePopoverEvent.emit();
  }

  cancelFilters() {
    this.closePopoverEvent.emit();
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
