import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'reports-processing-component',
  templateUrl: './reports-processing.component.html',
  styleUrls: ['./reports-processing.component.scss'],
})
export class ReportsProcessingComponent {
  @Input()
  selectedTemplateID: string;
  @Input()
  reportDownloading: boolean;
  @Input()
  fetchingPreview: boolean;
  @Output()
  handlePreviewReport = new EventEmitter<string>();
  @Output()
  handleDownloadReport = new EventEmitter<string>();

  constructor() {}

  previewReport(event: any): void {
    this.handlePreviewReport.emit(event);
  }

  downloadReport(event: any): void {
    this.handleDownloadReport.emit(event);
  }
}
