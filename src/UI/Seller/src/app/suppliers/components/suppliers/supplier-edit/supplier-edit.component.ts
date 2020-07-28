import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { SupportedRates, SupportedCurrencies } from '@app-seller/shared/models/supported-rates.interface';
import { OcIntegrationsAPIService } from '@app-seller/shared/services/oc-integrations-api/oc-integrations-api.service';
import { SupplierService } from '../supplier.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import {
  MiddlewareAPIService,
  SupplierFilterConfigDocument,
} from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ListPage } from '@ordercloud/headstart-sdk';
@Component({
  selector: 'app-supplier-edit',
  templateUrl: './supplier-edit.component.html',
  styleUrls: ['./supplier-edit.component.scss'],
})
export class SupplierEditComponent implements OnInit {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  @Input()
  supplierEditable;
  availableCurrencies: SupportedRates[] = [];
  isCreatingNew: boolean;
  isSupplierUser: boolean;
  countriesServicingOptions = [];
  constructor(
    public ocIntegrations: OcIntegrationsAPIService,
    public supplierService: SupplierService,
    private currentUserService: CurrentUserService
  ) {
    this.isCreatingNew = this.supplierService.checkIfCreatingNew();
  }

  async ngOnInit(): Promise<void> {
    this.availableCurrencies = await this.ocIntegrations.getAvailableCurrencies();
    this.availableCurrencies = this.availableCurrencies.filter(c =>
      Object.values(SupportedCurrencies).includes(SupportedCurrencies[c.Currency])
    );
    this.isSupplierUser = await this.currentUserService.isSupplierUser();
    this.setUpSupplierCountrySelectIfNeeded();
  }

  setUpSupplierCountrySelectIfNeeded(): void {
    const indexOfCountriesServicingConfig = this.filterConfig.Filters?.findIndex(
      s => s.Path === 'xp.CountriesServicing'
    );
    if (indexOfCountriesServicingConfig > -1) {
      this.countriesServicingOptions = this.filterConfig.Filters[indexOfCountriesServicingConfig].Items;
    }
  }

  getCountryCheckbox(country: string): string {
    const isIncluded = this.supplierEditable?.xp?.CountriesServicing?.includes(country);
    return isIncluded;
  }

  updateCountriesServicing(checked: boolean, country: string): void {
    let newCountriesSupported = this.supplierEditable.xp.CountriesServicing || [];
    const isCountryInExistingValue = newCountriesSupported.includes(country);
    if (checked && !isCountryInExistingValue) {
      newCountriesSupported = [...newCountriesSupported, country];
    } else if (!checked && isCountryInExistingValue) {
      newCountriesSupported = newCountriesSupported.filter(n => n !== country);
    }
    this.updateResource.emit({ value: newCountriesSupported, field: 'xp.CountriesServicing' });
  }

  updateResourceFromEvent(event: any, field: string): void {
    if (field.startsWith('xp.ProductTypes')) {
      const form = this.resourceForm.getRawValue();
      const valueToArray = [];
      Object.keys(form.ProductTypes).forEach(item => {
        if (form.ProductTypes[item]) valueToArray.push(item);
      });
      this.updateResource.emit({ field: 'xp.ProductTypes', value: valueToArray });
    } else {
      const value = ['Active', 'xp.SyncFreightPop'].includes(field) ? event.target.checked : event.target.value;
      this.updateResource.emit({ value, field });
    }
  }
}
