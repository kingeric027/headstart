import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { faCalendar, faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormControl } from '@angular/forms';
import { debounceTime, takeWhile } from 'rxjs/operators';
import { DateValidator } from '../../validators/validators';
import { OCMComponent } from '../base-component';
import { OrderFilters } from 'marketplace';
import { DatePipe } from '@angular/common';

@Component({
  templateUrl: './order-date-filter.component.html',
  styleUrls: ['./order-date-filter.component.scss'],
})
export class OCMOrderDateFilter extends OCMComponent implements OnInit, OnDestroy {
  private alive = true;
  faCalendar = faCalendar;
  faTimes = faTimes;
  form: FormGroup;

  constructor(private datePipe: DatePipe) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      fromDate: new FormControl(null as Date, DateValidator),
      toDate: new FormControl(null as Date, DateValidator),
    });
    this.onFormChanges();
  }

  ngOnContextSet() {
    this.context.orderHistory.filters.onFiltersChange(this.handlefiltersChange);
  }

  handlefiltersChange = (filters: OrderFilters) => {
    this.form.setValue({
      fromDate: this.inverseFormatDate(filters.fromDate),
      toDate: this.inverseFormatDate(filters.toDate)
    });
  }

  private onFormChanges() {
    this.form.valueChanges
      .pipe(
        debounceTime(500),
        takeWhile(() => this.alive)
      )
      .subscribe(() => {
        this.emitDate();
      });
  }

  private emitDate() {
    if (this.form.get('fromDate').invalid || this.form.get('toDate').invalid) {
      return;
    }
    const fromDate: Date = this.form.get('fromDate').value;
    const toDate: Date = this.form.get('toDate').value;
    if (fromDate) {
      this.context.orderHistory.filters.filterByFromDate(this.formatDate(fromDate));
    }
    if (toDate) {
      toDate.setDate(toDate.getDate() + 1);
      this.context.orderHistory.filters.filterByToDate(this.formatDate(toDate));
    }
  }

  clearToDate() {
    this.context.orderHistory.filters.filterByToDate(undefined);
  }

  clearFromDate() {
    this.context.orderHistory.filters.filterByFromDate(undefined);
  }

  ngOnDestroy() {
    this.alive = false;
  }

  private formatDate(date: Date): string {
    return this.datePipe.transform(date, 'shortDate').replace(/\//g, '-');
  }

  private inverseFormatDate(date: string): Date {
    return date ? new Date(date.substr(1)) : null;
  }
}
