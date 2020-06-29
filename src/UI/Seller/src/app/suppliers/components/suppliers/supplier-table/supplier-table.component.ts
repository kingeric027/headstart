import { Component, ChangeDetectorRef, NgZone, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Supplier } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { get as _get } from 'lodash';
import {
  ValidateRichTextDescription,
  ValidateEmail,
  ValidatePhone,
  ValidateSupplierCategorySelection,
} from '@app-seller/validators/validators';
import { SupplierService } from '../supplier.service';
import { MarketplaceSupplier } from 'marketplace-javascript-sdk';

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
    Currency: new FormControl(_get(supplier, 'xp.Currency'), Validators.required)
  });
}

@Component({
  selector: 'app-supplier-table',
  templateUrl: './supplier-table.component.html',
  styleUrls: ['./supplier-table.component.scss'],
})
export class SupplierTableComponent extends ResourceCrudComponent<Supplier> {
  constructor(
    private supplierService: SupplierService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, supplierService, router, activatedroute, ngZone, createSupplierForm);
    this.router = router;
  }

  // these are custom filters for SEB that should be moved out of this code and into
  // external storage in the future, filters that apply to all of marketplace
  // can also be added to this before passing into the resource table
  filterConfig = {
    id: 'SEB',
    timeStamp: '0001-01-01T00:00:00+00:00',
    MarketplaceName: 'Self Esteem Brands',
    Filters: [
      {
        Display: 'Vendor Level',
        Path: 'xp.Categories.VendorLevel',
        Items: [
          {Text: 'PREFERRED', Value: 'PREFERRED'},
          {Text: 'DESIGNATED', Value: 'DESIGNATED'},
          {Text: 'MANDATED', Value: 'MANDATED'},
          {Text: 'EXCLUSIVE DESIGNATED', Value: 'EXCLUSIVE DESIGNATED'}],
        Type: 'Dropdown',
      },
      {
        Display: 'Service Category',
        Path: 'xp.Categories.ServiceCategory',
        Items: [
          {Text: 'Accounting Services and Software', Value: 'Accounting Services and Software'},
          {Text: 'AEDs & Accessories', Value: 'AEDs & Accessories'},
          {Text: 'Air Fragrance Systems', Value: 'Air Fragrance Systems'},
          {Text: 'Apparel AF Staff Uniforms', Value: 'Apparel AF Staff Uniforms'},
          {Text: 'Apparel and Promotional Products', Value: 'Apparel and Promotional Products'},
          {Text: 'Billing', Value: 'Billing'},
          {Text: 'Body Composition Solutions', Value: 'Body Composition Solutions'},
          {Text: 'Cleaning Services', Value: 'Cleaning Services'},
          {Text: 'Cleaning Supplies  & Equipment Wipes', Value: 'Cleaning Supplies  & Equipment Wipes'},
          {Text: 'Club Design & Construction', Value: 'Club Design & Construction'},
          {Text: 'Club Management Software', Value: 'Club Management Software'},
          {Text: 'Club Rental Services', Value: 'Club Rental Services'},
          {Text: 'Collection Agency', Value: 'Collection Agency'},
          {Text: 'Employment & Labor Law Toolkit and Resources', Value: 'Employment & Labor Law Toolkit and Resources'},
          {Text: 'Employment Services', Value: 'Employment Services'},
          {Text: 'Facility Equipment', Value: 'Facility Equipment'},
          {Text: 'Fans', Value: 'Fans'},
          {Text: 'Financing/Lender', Value: 'Financing/Lender'},
          {Text: 'First Aid & Safety', Value: 'First Aid & Safety'},
          {Text: 'Fitness Accessories', Value: 'Fitness Accessories'},
          {Text: 'Fitness Education', Value: 'Fitness Education'},
          {Text: 'Fitness Equipment', Value: 'Fitness Equipment'},
          {Text: 'Fitness Incentives Programs/Processor', Value: 'Fitness Incentives Programs/Processor'},
          {Text: 'Free Weights', Value: 'Free Weights'},
          {Text: 'Heart Zoning Solutions', Value: 'Heart Zoning Solutions'},
          {Text: 'Insurance - Club', Value: 'Insurance - Club'},
          {Text: 'Insurance ? Health Club Surety Bonds', Value: 'Insurance ? Health Club Surety Bonds'},
          {Text: 'Lease Management & Audit', Value: 'Lease Management & Audit'},
          {Text: 'Market Expansion Line/Call Answering Service', Value: 'Market Expansion Line/Call Answering Service'},
          {Text: 'Marketing - Digital/Social Media', Value: 'Marketing - Digital/Social Media'},
          {Text: 'Marketing - Direct Mail', Value: 'Marketing - Direct Mail'},
          {Text: 'Marketing - Email & CRM', Value: 'Marketing - Email & CRM'},
          {Text: 'Marketing - Print Packages, Promotion Signage, Outdoor Events', Value: 'Marketing - Print Packages, Promotion Signage, Outdoor Events'},
          {Text: 'Merchant & Credit Card Services', Value: 'Merchant & Credit Card Services'},
          {Text: 'Music', Value: 'Music'},
          {Text: 'Mystery Shopping Services', Value: 'Mystery Shopping Services'},
          {Text: 'Non-Profit Fitness Association', Value: 'Non-Profit Fitness Association'},
          {Text: 'Office Supplies', Value: 'Office Supplies'},
          {Text: 'PT Sales Consulting', Value: 'PT Sales Consulting'},
          {Text: 'Real Estate', Value: 'Real Estate'},
          {Text: 'Resale Products', Value: 'Resale Products'},
          {Text: 'Security & Hardware Installation', Value: 'Security & Hardware Installation'},
          {Text: 'Signage - Exterior', Value: 'Signage - Exterior'},
          {Text: 'Signage - Interior', Value: 'Signage - Interior'},
          {Text: 'Supplements & Nutrition', Value: 'Supplements & Nutrition'},
          {Text: 'Tanning', Value: 'Tanning'},
          {Text: 'TV and Connection Solutions', Value: 'TV and Connection Solutions'},
          {Text: 'Vending Machines', Value: 'Vending Machines'},
          {Text: 'Insurance ? Club', Value: 'Insurance ? Club'},
          {Text: 'Resale', Value: 'Resale'},
          {Text: 'Signage ? Interior', Value: 'Signage ? Interior'},
          {Text: 'Accounting Services', Value: 'Accounting Services'},
          {Text: 'Apparel & Promotional Products', Value: 'Apparel & Promotional Products'},
          {Text: 'Business Texting Software', Value: 'Business Texting Software'},
          {Text: 'Equipment', Value: 'Equipment'},
          {Text: 'Financial Management Software & Processing', Value: 'Financial Management Software & Processing'},
          {Text: 'Financing', Value: 'Financing'},
          {Text: 'First Aid Kits', Value: 'First Aid Kits'},
          {Text: 'Floor Mat Service', Value: 'Floor Mat Service'},
          {Text: 'General Office Supplies', Value: 'General Office Supplies'},
          {Text: 'Hand Dryers', Value: 'Hand Dryers'},
          {Text: 'Hiring Management Software', Value: 'Hiring Management Software'},
          {Text: 'Insurance - Studio', Value: 'Insurance - Studio'},
          {Text: 'Linens', Value: 'Linens'},
          {Text: 'Marketing  - Digital/Social Media', Value: 'Marketing  - Digital/Social Media'},
          {Text: 'Marketing - Email', Value: 'Marketing - Email'},
          {Text: 'Marketing - Print Packages, Promotion signage, Outdoor Events', Value: 'Marketing - Print Packages, Promotion signage, Outdoor Events'},
          {Text: 'On-Hold Messaging Service', Value: 'On-Hold Messaging Service'},
          {Text: 'Pre-Employment Screening Services', Value: 'Pre-Employment Screening Services'},
          {Text: 'Security & Technology', Value: 'Security & Technology'},
          {Text: 'Signing', Value: 'Signing'},
          {Text: 'Studio Design & Construction', Value: 'Studio Design & Construction'},
          {Text: 'Studio Management Software', Value: 'Studio Management Software'},
          {Text: 'Studio Rental Services', Value: 'Studio Rental Services'},
          {Text: 'Studio Surety Bonds', Value: 'Studio Surety Bonds'},
          {Text: 'Supplies: Color, Tools', Value: 'Supplies: Color, Tools'},
          {Text: 'Uniforms', Value: 'Uniforms'},
          {Text: 'Wax', Value: 'Wax'},
          {Text: 'Wipes and Cleaning Supplies', Value: 'Wipes and Cleaning Supplies'}
        ],
        Type: 'Dropdown',
      },
    ],
  };
}
