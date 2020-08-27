import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

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

  requiresPipe(header: string): boolean {
    if (header.toLowerCase().includes('phone')) {
      this.pipeName = 'phone';
      return true;
    } else {
      return false;
    }
  }

  //TO-DO - Will need refactoring for future data values that are more deeply nested.
  getNestedValue(item: {}, header: string): string {
    const props = header.split('.');
    return item[props[0]][props[1]];
  }
}
