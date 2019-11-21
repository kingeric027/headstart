import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Supplier } from '@ordercloud/angular-sdk';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { FormControl, FormGroup } from '@angular/forms';
import { FilterDictionary } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { Router, ActivatedRoute } from '@angular/router';

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
    activatedroute: ActivatedRoute
  ) {
    super(changeDetectorRef, supplierService, router, activatedroute);
    this.router = router;
  }

  route = 'supplier';

  filterConfig = {
    id: 'SEB',
    timeStamp: '0001-01-01T00:00:00+00:00',
    MarketplaceName: 'Self Esteem Brands',
    Filters: [
      {
        Display: 'Vendor Level',
        Path: 'xp.Categories.VendorLevel',
        Values: ['PREFERRED', 'DESIGNATED', 'MANDATED', 'EXCLUSIVE DESIGNATED'],
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
      },
    ],
  };
}
