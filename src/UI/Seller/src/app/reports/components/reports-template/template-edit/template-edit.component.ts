import { Component, ChangeDetectorRef, NgZone, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { ReportsTemplateService, ReportTemplate } from '@app-seller/shared/services/middleware-api/reports-template.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup } from '@angular/forms';
import { buyerLocation as buyerLocationHeaders } from '../models/headers';
import { buyerLocation as buyerLocationFilters, Filter } from '../models/filters';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { OcBuyerService } from '@ordercloud/angular-sdk';
import { cloneDeep } from 'lodash';
import { SupportedCountries, GeographyConfig } from '@app-seller/shared/models/supported-countries.interface';
import { AppGeographyService } from '@app-seller/shared';

@Component({
  selector: 'template-edit-component',
  templateUrl: './template-edit.component.html',
  styleUrls: ['./template-edit.component.scss'],
})
export class TemplateEditComponent implements OnChanges {
  @Input()
  resourceForm: FormGroup;
  @Input()
  reportType: string;
  @Input()
  set resourceInSelection (template: ReportTemplate) {
    this.reportTemplate = template;
    this.setHeadersAndFilters(this.reportType);
    if (this.reportsTemplateService.checkIfCreatingNew()) {
      this.isCreatingNew = true;
      this.handleSelectAllHeaders();
    } 
  }
  @Input()
  updatedResource: ReportTemplate;
  @Output()
  updateResource = new EventEmitter<any>();
  reportTemplate: ReportTemplate;
  isCreatingNew: boolean;
  reportTemplateEditable: ReportTemplate;
  headers: any[];
  filters: Filter[];
  filterChipsToDisplay: Filter[] = [];
  faCheckCircle = faCheckCircle;
  countryOptions: SupportedCountries[]

  constructor(
      private reportsTemplateService: ReportsTemplateService,
      private geographyService: AppGeographyService,
      private ocBuyerService: OcBuyerService,
      changeDetectorRef: ChangeDetectorRef,
      router: Router,
      activatedRoute: ActivatedRoute,
      ngZone: NgZone,
      ) {}

  ngOnChanges (changes: SimpleChanges) {
    if (changes.resourceInSelection?.currentValue !== changes.resourceInSelection?.previousValue) {
      if(!this.reportsTemplateService.checkIfCreatingNew()) {
        this.filters?.forEach(filter => {
          if (this.updatedResource.Filters && this.updatedResource.Filters[filter.path]?.length) {
            this.filterChipsToDisplay.push(filter);
          }
        })
      }
    }
  }

  handleUpdateReportTemplate(event: any, field: string): void {
    const value = ['AvailableToSuppliers'].includes(field) ? event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
  }

  setHeadersAndFilters(reportType: string): void {
    switch(reportType) {
      case 'BuyerLocation':
        this.headers = buyerLocationHeaders;
        this.filters = buyerLocationFilters;
        break;
    }
    if (this.filters?.length) {
      this.populateFilters();
    }
  }

  async populateFilters(): Promise<void> {
    for (let filter of this.filters) {
      let data;
      switch (filter.sourceType) {
        case 'oc':
          data = await this[filter.source].List().toPromise();
          filter.filterValues = data.Items;
          break;
        case 'model':
          if (filter.name === 'Country') {
            data = this.countryOptions = GeographyConfig.getCountries();
            filter.filterValues = data;
          }
          if (filter.name === 'State') {
            //For now, filtering is setup only for US states.  Will eventually need a way to get all states relevant to Seller organization.
            data = this.geographyService.getStates('US');
            filter.filterValues = data;
        }
      }
    }
  }

  isHeaderSelected(header: string): boolean {
    return this.updatedResource.Headers?.includes(header);
  }

  isFilterValueSelected(filter: Filter, filterValue: any): boolean {
    return this.updatedResource.Filters[filter?.path]?.includes(filterValue[filter.dataKey]);
  }

  toggleHeader(selectedHeader: string): void {
    let headers = [...this.updatedResource.Headers];
    const i = headers?.indexOf(selectedHeader);
    if (i > -1) {
      headers?.splice(i, 1);
    } else {
      headers?.push(selectedHeader);
    }
    const headersToCompare = this.headers.map(header => header.path);
    headers?.sort((a, b) => headersToCompare.indexOf(a) - headersToCompare.indexOf(b));
    this.resourceForm.controls['Headers'].setValue(headers);
    this.updateResource.emit({ value: headers, field: 'Headers'});
  }

  toggleFilter(filter: Filter, filterValue: any): void {
    let filters = cloneDeep(this.updatedResource.Filters);
    var selectedFilterValues = filters[filter.path];
    if (!selectedFilterValues || !selectedFilterValues.length) {
      filters[filter.path] = [filterValue[filter.dataKey]];
    } else {
      const i = selectedFilterValues.indexOf(filterValue[filter.dataKey]);
      if (i > -1) {
        selectedFilterValues.splice(i, 1);
      } else {
        selectedFilterValues.push(filterValue[filter.dataKey]);
      }
    }
    this.resourceForm.controls['Filters'].setValue(filters);
    this.updateResource.emit({ value: filters, field: 'Filters'});
  }

  toggleIncludeAllValues(includeAll: boolean, filter: Filter): void {
    if (includeAll) {
      const i = this.filterChipsToDisplay.indexOf(filter);
      this.filterChipsToDisplay.splice(i, 1);
      let filterValues = this.filters.find(f => f.path === filter.path).filterValues;
      this.updatedResource.Filters[filter.path] = filterValues.map(value => value[filter.dataKey]);
      this.resourceForm.controls['Filters'].setValue(this.updatedResource.Filters);
      this.updateResource.emit({ value: this.updatedResource.Filters, field: 'Filters'});
    } else {
      this.filterChipsToDisplay.push(filter);
      this.updatedResource.Filters[filter.path] = [];
      this.resourceForm.controls['Filters'].setValue(this.updatedResource.Filters);
      this.updateResource.emit({ value: this.updatedResource.Filters, field: 'Filters'});
    }
  }

  checkForChipDisplay(filter: Filter): boolean {
    return this.filterChipsToDisplay.includes(filter);
  }

  handleSelectAllHeaders(): void {
    let headersPaths = this.headers.map(header => header.path);
    this.resourceForm.controls['Headers'].setValue(headersPaths);
    this.updateResource.emit({ value: headersPaths, field: 'Headers'});
  }

  handleUnselectAllHeaders(): void {
    this.updatedResource.Headers = [];
    this.resourceForm.controls['Headers'].setValue(this.updatedResource.Headers);
    this.updateResource.emit({ value: this.updatedResource.Headers, field: 'Headers'});
  }
}