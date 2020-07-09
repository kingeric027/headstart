import { Component, ChangeDetectorRef, NgZone, OnInit, AfterViewInit, ViewChild, Inject } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Supplier } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators, ValidatorFn } from '@angular/forms';
import { get as _get } from 'lodash';
import {
  ValidateRichTextDescription,
  ValidateEmail,
  RequireCheckboxesToBeChecked,
  ValidatePhone,
  ValidateSupplierCategorySelection,
} from '@app-seller/validators/validators';
import { SupplierService } from '../supplier.service';
import { MarketplaceSupplier } from 'marketplace-javascript-sdk';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
export interface SupplierCategoryConfigFilters {
  Display: string;
  Path: string;
  Items: any[];
}
export interface SupplierCategoryConfig {
  id: string;
  timeStamp: string;
  MarketplaceName: string;
  Filters: Array<SupplierCategoryConfigFilters>;
}

function createSupplierForm(supplier: MarketplaceSupplier) {
  return new FormGroup({
    ID: new FormControl({ value: supplier.ID, disabled: !this.isCreatingNew }),
    Name: new FormControl(supplier.Name, Validators.required),
    LogoUrl: new FormControl(_get(supplier, 'xp.Images') && _get(supplier, 'xp.Images')[0]?.URL),
    Description: new FormControl(_get(supplier, 'xp.Description'), ValidateRichTextDescription),
    // need to figure out strucure of free string array
    // StaticContentLinks: new FormControl(_get(supplier, 'xp.StaticContentLinks'), Validators.required),
    SupportContactName: new FormControl(
      (_get(supplier, 'xp.SupportContact') && _get(supplier, 'xp.SupportContact.Name')) || ''
    ),
    SupportContactEmail: new FormControl(
      (_get(supplier, 'xp.SupportContact') && _get(supplier, 'xp.SupportContact.Email')) || '',
      ValidateEmail
    ),
    SupportContactPhone: new FormControl(
      (_get(supplier, 'xp.SupportContact') && _get(supplier, 'xp.SupportContact.Phone')) || ''
    ),
    Active: new FormControl(supplier.Active),
    SyncFreightPop: new FormControl(supplier.xp?.SyncFreightPop || false),
    Currency: new FormControl(_get(supplier, 'xp.Currency'), Validators.required),
    ProductType: new FormGroup({
      StandardProducts: new FormControl(supplier.xp?.StandardProducts || false),
      QuoteProducts: new FormControl(supplier.xp?.QuoteProducts || false),
      POProducts: new FormControl(supplier.xp?.StandardProducts || false)
    }, RequireCheckboxesToBeChecked())

  });
}

@Component({
  selector: 'app-supplier-table',
  templateUrl: './supplier-table.component.html',
  styleUrls: ['./supplier-table.component.scss'],
})
export class SupplierTableComponent extends ResourceCrudComponent<Supplier> {
  filterConfig: {};
  constructor(
    private supplierService: SupplierService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    ngZone: NgZone,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    super(changeDetectorRef, supplierService, router, activatedroute, ngZone, createSupplierForm);
    this.router = router;
    this.buildFilterConfig()
  }

  async buildFilterConfig(): Promise<void> {
    this.filterConfig = await MarketplaceSDK.SupplierCategoryConfigs.Get(this.appConfig.marketplaceID);
  }
}
