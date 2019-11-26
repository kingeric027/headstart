import { Component, OnInit, OnDestroy } from '@angular/core';
import { faCalendar, faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormControl } from '@angular/forms';
import { debounceTime, takeWhile } from 'rxjs/operators';
import { DateValidator } from '../../../validators/validators';
import { OCMComponent } from '../../base-component';
import { OrderFilters } from 'marketplace';
import { DatePipe } from '@angular/common';

@Component({
  templateUrl: './order-date-filter.component.html',
  styleUrls: ['./order-date-filter.component.scss'],
})
export class OCMOrderDateFilter extends OCMComponent implements OnInit, OnDestroy {
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
    this.context.orderHistory.filters.activeFiltersSubject.pipe(takeWhile(() => this.alive)).subscribe(this.handlefiltersChange);
  }

  handlefiltersChange = (filters: OrderFilters) => {
    const fromDate = this.inverseFormatDate(filters.fromDate);
    const toDate = this.inverseFormatDate(filters.toDate);
    this.form.setValue({ fromDate, toDate });
  }

  private onFormChanges() {
    this.form.valueChanges
      .pipe(
        debounceTime(500),
        takeWhile(() => this.alive)
      )
      .subscribe(() => {
        this.doFilter();
      });
  }

  private doFilter() {
    if (this.form.get('fromDate').invalid || this.form.get('toDate').invalid) {
      return;
    }
    const fromDate: Date = this.form.get('fromDate').value;
    const toDate: Date = this.form.get('toDate').value;
    this.context.orderHistory.filters.filterByDateSubmitted(this.formatDate(fromDate), this.formatDate(toDate));
  }

  clearToDate() {
    this.form.get('toDate').setValue(null);
    this.doFilter();
  }

  clearFromDate() {
    this.form.get('fromDate').setValue(null);
    this.doFilter();
  }

  private formatDate(date: Date): string {
    return date ? this.datePipe.transform(date, 'shortDate').replace(/\//g, '-') : null;
  }

  private inverseFormatDate(date: string): Date {
    return date ? new Date(date) : null;
  }
}
