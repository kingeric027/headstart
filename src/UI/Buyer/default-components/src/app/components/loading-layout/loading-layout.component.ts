import { Component, Input, OnChanges } from '@angular/core';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './loading-layout.component.html',
  styleUrls: ['./loading-layout.component.scss'],
})
export class OCMLoadingLayout extends OCMComponent {
  @Input() blockCount: string;
  @Input() columns: string;
  @Input() resource: any;
  @Input() height: string;
  @Input() width: string;
  @Input() mb: string;
  columnsToRender: Array<any> = [];
  divsToRender: Array<any> = [];
  bootstrapColumns: number;

  ngOnContextSet() {
    this.columnsToRender = new Array(this.columns);
    this.divsToRender = new Array(this.blockCount);
    this.bootstrapColumns = Math.round(12 / this.columnsToRender.length);
  }
}
