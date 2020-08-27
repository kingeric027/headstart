import { Component, OnInit } from '@angular/core';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ReportsTemplateService } from '@app-seller/shared/services/middleware-api/reports-template.service';
import { reportTypes } from '../../reports-template/models/headers';
import { ReportTemplate } from '@ordercloud/headstart-sdk';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss'],
})
export class ReportsComponent implements OnInit {
  reportSelectionForm: FormGroup;
  reportTypes = reportTypes;
  reportTemplates: ReportTemplate[] = [];
  selectedTemplateID: string;
  selectedTemplate: ReportTemplate;
  reportData: object[];
  displayHeaders: string[];
  fetchingPreview = false;
  reportDownloading = false;

  constructor(private currentUserService: CurrentUserService, private reportsTemplateService: ReportsTemplateService) {}

  async ngOnInit(): Promise<void> {
    this.createReportSelectionForm();
  }

  createReportSelectionForm(): void {
    this.reportSelectionForm = new FormGroup({
      ReportType: new FormControl(null, Validators.required),
      ReportTemplate: new FormControl(null, Validators.required),
    });
  }

  async handleReportTypeSelection(event: string): Promise<void> {
    this.reportSelectionForm.controls['ReportType'].setValue(event);
    this.reportTemplates = await this.reportsTemplateService.listReportTemplatesByReportType(event);
  }

  handleReportTemplateSelection(event: string): void {
    this.selectedTemplateID = event;
    this.selectedTemplate = this.reportTemplates.find(template => template.TemplateID === event);
    const headers = this.selectedTemplate.Headers;
    this.displayHeaders = headers.map(header => this.humanizeHeader(header));
    this.reportSelectionForm.controls['ReportTemplate'].setValue(event);
  }

  async handlePreviewReport(event: string): Promise<void> {
    this.fetchingPreview = true;
    this.reportData = await this.reportsTemplateService.previewReport(this.selectedTemplate);
    this.fetchingPreview = false;
  }

  async handleDownloadReport(event: string): Promise<void> {
    this.reportDownloading = true;
    await this.reportsTemplateService.downloadReport(this.selectedTemplate);
    this.reportDownloading = false;
  }

  humanizeHeader(header: string): string {
    let newHeader = header.includes('xp.') ? header.split('.')[1] : header;
    newHeader = newHeader.replace(/([a-z](?=[0-9A-Z])|[A-Z](?=[A-Z][a-z]))/, '$1 ');
    return newHeader;
  }
}
