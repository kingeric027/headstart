import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { Rates } from '@app-seller/shared/models/exchange-rates.interface';
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
  constructor(public middleware: MiddlewareAPIService) {}

  async ngOnInit(): Promise<void> {
    this.availableCurrencies = (await this.middleware.getAvailableCurrencies('USD')).Rates;
  }

  updateResourceFromEvent(event: any, field: string): void {
    const value = field === 'Active' || field === 'xp.SyncFreightPop' ? event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
  }
}
