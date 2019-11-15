import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OCMComponent } from '../base-component';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './sort-suppliers.component.html',
  styleUrls: ['./sort-suppliers.component.scss'],
})
export class OCMSupplierSort extends OCMComponent implements OnInit {
  form: FormGroup;
  options = [{ value: 'Name', label: 'A to Z' }, { value: '!Name', label: 'Z to A' }];
  @Output() closePopoverEvent = new EventEmitter();

  ngOnInit() {
    this.form = new FormGroup({ sortBy: new FormControl(null) });
  }

  ngOnContextSet() {
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
}
