import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { SupportedRates, SupportedCurrencies } from '@app-seller/shared/models/supported-rates.interface';
import { OcIntegrationsAPIService } from '@app-seller/shared/services/oc-integrations-api/oc-integrations-api.service';
import { SupplierService } from '../supplier.service';
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
  constructor(public ocIntegrations: OcIntegrationsAPIService, public supplierService: SupplierService) {
    this.isCreatingNew = this.supplierService.checkIfCreatingNew();
  }

  async ngOnInit(): Promise<void> {
    this.availableCurrencies = await this.ocIntegrations.getAvailableCurrencies();
    this.availableCurrencies = this.availableCurrencies.filter(c =>
      Object.values(SupportedCurrencies).includes(SupportedCurrencies[c.Currency])
    );
  }

  updateResourceFromEvent(event: any, field: string): void {
    const value = field === 'Active' || field === 'xp.SyncFreightPop' ? event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
  }
}
