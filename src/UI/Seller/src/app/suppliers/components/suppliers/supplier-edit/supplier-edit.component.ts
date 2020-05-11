import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { Rates } from '@app-seller/shared/models/exchange-rates.interface';
import { OcIntegrationsAPIService } from '@app-seller/shared/services/oc-integrations-api/oc-integrations-api.service';
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
  availableCurrencies: Rates[] = [];
  constructor(public ocIntegrations: OcIntegrationsAPIService) {}

  async ngOnInit(): Promise<void> {
    this.availableCurrencies = (await this.ocIntegrations.getAvailableCurrencies('USD')).Rates;
  }

  updateResourceFromEvent(event: any, field: string): void {
    const value = field === 'Active' || field === 'xp.SyncFreightPop' ? event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
  }
}
