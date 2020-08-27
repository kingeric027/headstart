import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ReportTemplate } from '@ordercloud/headstart-sdk';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'reports-selection-component',
  templateUrl: './reports-selection.component.html',
  styleUrls: ['./reports-selection.component.scss'],
})
export class ReportsSelectionComponent {
  @Input()
  reportTypes: any[];
  @Input()
  reportTemplates: ReportTemplate[];
  @Input()
  reportSelectionForm: FormGroup;
  @Input()
  displayHeaders: string[];
  @Output()
  handleReportTypeSelection = new EventEmitter<string>();
  @Output()
  handleReportTemplateSelection = new EventEmitter<string>();
  selectedTemplate: ReportTemplate = {};
  showDetails = false;
  filterEntries: string[][];

  constructor() {}

  updateReportType(event): void {
    this.handleReportTypeSelection.emit(event);
  }

  updateReportTemplate(event): void {
    this.handleReportTemplateSelection.emit(event);
    this.selectedTemplate = this.reportTemplates.find(template => template.id === event);
    this.filterEntries = Object.entries(this.selectedTemplate.Filters);
  }

  toggleShowDetails(): void {
    this.showDetails = !this.showDetails;
  }

  getDetailsDisplayVerb(): string {
    return this.showDetails ? 'Hide' : 'Show';
  }
}
