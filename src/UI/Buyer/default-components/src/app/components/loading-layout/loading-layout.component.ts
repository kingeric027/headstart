import { Component, Input } from '@angular/core';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './loading-layout.component.html',
  styleUrls: ['./loading-layout.component.scss'],
})
export class OCMLoadingLayout extends OCMComponent {
  @Input() resource: any; // which resource you're basing loading off of (e.g. suppliers, products, etc)
  @Input() height: string; // height of individual loading block
  @Input() width: string; // width of individual loading block
  @Input() mb: string; // bottom margin of individual loading block
  @Input() columns: string; // number of columns for loading layout
  @Input() rows: string; // number of rows to display in loading layout
  columnsToRender: Array<any> = []; // used to loop over in html to generate x number of rows
  divsToRender: Array<any> = []; // used to loop over in html to generate x number of divs (rows)
  bootstrapColumns: number; // used to calculate the proper html class for bootstrap columns

  ngOnContextSet() {
    this.columnsToRender = new Array(this.columns);
    this.divsToRender = new Array(this.rows);
    this.bootstrapColumns = Math.round(12 / this.columnsToRender.length);
  }
}
