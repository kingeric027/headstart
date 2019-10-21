import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { faCalendar, faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormControl } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { debounceTime, takeWhile } from 'rxjs/operators';
import { DateValidator } from '../../validators/validators';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './order-date-filter.component.html',
  styleUrls: ['./order-date-filter.component.scss'],
})
export class OCMOrderDateFilter implements OnInit, OnDestroy {
  private alive = true;
  faCalendar = faCalendar;
  faTimes = faTimes;
  form: FormGroup;
  @Output() selectedDate = new EventEmitter<string[]>();

  constructor(private datePipe: DatePipe) {}

  ngOnInit() {
    this.form = new FormGroup({
      fromDate: new FormControl(null as Date, DateValidator),
      toDate: new FormControl(null as Date, DateValidator),
    });
    this.onFormChanges();
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

  format(date: Date): string {
    return this.datePipe.transform(date, 'shortDate').replace(/\//g, '-');
  }

  private emitDate() {
    if (this.form.get('fromDate').invalid || this.form.get('toDate').invalid) {
      return;
    }

    const fromDate: Date = this.form.get('fromDate').value;
    const toDate: Date = this.form.get('toDate').value;
    const dateSubmitted: string[] = [];
    if (fromDate) {
      dateSubmitted.push(`>${this.format(fromDate)}`);
    }
    if (toDate) {
      // Add one day so the filter will be inclusive of the date selected
      toDate.setDate(toDate.getDate() + 1);
      dateSubmitted.push(`<${this.format(toDate)}`);
    }

    this.selectedDate.emit(dateSubmitted);
  }

  clearToDate() {
    this.form.patchValue({ toDate: null });
    this.emitDate();
  }

  clearFromDate() {
    this.form.patchValue({ fromDate: null });
    this.emitDate();
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
