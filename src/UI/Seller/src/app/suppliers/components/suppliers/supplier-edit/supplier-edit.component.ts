import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { SupportedRates, SupportedCurrencies } from '@app-seller/shared/models/supported-rates.interface';
import { OcIntegrationsAPIService } from '@app-seller/shared/services/oc-integrations-api/oc-integrations-api.service';
import { SupplierService } from '../supplier.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
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
  availableCurrencies: SupportedRates[] = [];
  isCreatingNew: boolean;
  isSupplierUser: boolean;
  constructor(public ocIntegrations: OcIntegrationsAPIService, 
    public supplierService: SupplierService,
    private currentUserService: CurrentUserService,) {
    this.isCreatingNew = this.supplierService.checkIfCreatingNew();
  }

  async ngOnInit(): Promise<void> {
    this.availableCurrencies = await this.ocIntegrations.getAvailableCurrencies();
    this.availableCurrencies = this.availableCurrencies.filter(c =>
      Object.values(SupportedCurrencies).includes(SupportedCurrencies[c.Currency])
    );
    this.isSupplierUser = await this.currentUserService.isSupplierUser();
  }

  updateResourceFromEvent(event: any, field: string): void {
    if(field.startsWith('xp.ProductTypes')) {
      const form = this.resourceForm.getRawValue();
      const valueToArray = [];
      Object.keys(form.ProductTypes).forEach(item => {
        if(form.ProductTypes[item]) valueToArray.push(item);
      })
      this.updateResource.emit({field: 'xp.ProductTypes', value: valueToArray}) 
    } else {
      const value = ['Active', 'xp.SyncFreightPop'].includes(field) ?
      event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
    }
  }
}
