import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'reports-preview-component',
  templateUrl: './reports-preview.component.html',
  styleUrls: ['./reports-preview.component.scss'],
})
export class ReportsPreviewComponent implements OnChanges {
  @Input()
  reportData: object[];
  @Input()
  originalHeaders: string[];
  @Input()
  displayHeaders: string[];
  @Input()
  selectedTemplateID: string;
  pipeName: string;

  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.selectedTemplateID) {
      this.reportData = [];
    }
  }

  formatData(item: any, header: string): string {
    if (header.toLowerCase().includes('date')) {;
      return moment(item[header]).format('MM/DD/YYYY');
    } else if (header.includes('.')) {
      return (this.getNestedValue(item, header));
    } else return item[header];
  }

  requiresPipe(header: string): boolean {
    if (header.toLowerCase().includes('phone')) {
      this.pipeName = 'phone';
      return true;
    } else if (header.toLowerCase().includes('total') ||
              header.toLowerCase().includes('cost') ||
              header.toLowerCase().includes('discount')) 
    { this.pipeName = 'currency';
      return true;
    } else {
      return false;
    }
  }

  //TO-DO - Will need refactoring for future data values that are more deeply nested.
  getNestedValue(item: {}, header: string): string {
    const props = header.split('.');
    const first = item[props[0]];
    if (first) {
      return first[props[1]];
    }
  }
}
