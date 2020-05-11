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
  Values: string[];
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
        Values: ['PREFERRED', 'DESIGNATED', 'MANDATED', 'EXCLUSIVE DESIGNATED'],
        Type: 'Dropdown',
      },
      {
        Display: 'Service Category',
        Path: 'xp.Categories.ServiceCategory',
        Values: [
          'Accounting Services and Software',
          'AEDs & Accessories',
          'Air Fragrance Systems',
          'Apparel AF Staff Uniforms',
          'Apparel and Promotional Products',
          'Billing',
          'Body Composition Solutions',
          'Cleaning Services',
          'Cleaning Supplies  & Equipment Wipes',
          'Club Design & Construction',
          'Club Management Software',
          'Club Rental Services',
          'Collection Agency',
          'Employment & Labor Law Toolkit and Resources',
          'Employment Services',
          'Facility Equipment',
          'Fans',
          'Financing/Lender',
          'First Aid & Safety',
          'Fitness Accessories',
          'Fitness Education',
          'Fitness Equipment',
          'Fitness Incentives Programs/Processor',
          'Free Weights',
          'Heart Zoning Solutions',
          'Insurance - Club',
          'Insurance ? Health Club Surety Bonds',
          'Lease Management & Audit',
          'Market Expansion Line/Call Answering Service',
          'Marketing - Digital/Social Media',
          'Marketing - Direct Mail',
          'Marketing - Email & CRM',
          'Marketing - Print Packages, Promotion Signage, Outdoor Events',
          'Merchant & Credit Card Services',
          'Music',
          'Mystery Shopping Services',
          'Non-Profit Fitness Association',
          'Office Supplies',
          'PT Sales Consulting',
          'Real Estate',
          'Resale Products',
          'Security & Hardware Installation',
          'Signage - Exterior',
          'Signage - Interior',
          'Supplements & Nutrition',
          'Tanning',
          'TV and Connection Solutions',
          'Vending Machines',
          'Insurance ? Club',
          'Resale',
          'Signage ? Interior',
          'Accounting Services',
          'Apparel & Promotional Products',
          'Business Texting Software',
          'Equipment',
          'Financial Management Software & Processing',
          'Financing',
          'First Aid Kits',
          'Floor Mat Service',
          'General Office Supplies',
          'Hand Dryers',
          'Hiring Management Software',
          'Insurance - Studio',
          'Linens',
          'Marketing  - Digital/Social Media',
          'Marketing - Email',
          'Marketing - Print Packages, Promotion signage, Outdoor Events',
          'On-Hold Messaging Service',
          'Pre-Employment Screening Services',
          'Security & Technology',
          'Signing',
          'Studio Design & Construction',
          'Studio Management Software',
          'Studio Rental Services',
          'Studio Surety Bonds',
          'Supplies: Color, Tools',
          'Uniforms',
          'Wax',
          'Wipes and Cleaning Supplies',
        ],
        Type: 'Dropdown',
      },
    ],
  };
}
