import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { get as _get } from 'lodash';
import { ValidateRichTextDescription, ValidateEmail, } from '@app-seller/validators/validators';
function createSupplierForm(supplier) {
    return new FormGroup({
        ID: new FormControl({ value: supplier.ID, disabled: !this.isCreatingNew }),
        Name: new FormControl(supplier.Name, Validators.required),
        LogoUrl: new FormControl(_get(supplier, 'xp.Images') && _get(supplier, 'xp.Images')[0].URL),
        Description: new FormControl(_get(supplier, 'xp.Description'), ValidateRichTextDescription),
        // need to figure out strucure of free string array
        // StaticContentLinks: new FormControl(_get(supplier, 'xp.StaticContentLinks'), Validators.required),
        SupportContactName: new FormControl((_get(supplier, 'xp.SupportContact') && _get(supplier, 'xp.SupportContact.Name')) || ''),
        SupportContactEmail: new FormControl((_get(supplier, 'xp.SupportContact') && _get(supplier, 'xp.SupportContact.Email')) || '', ValidateEmail),
        SupportContactPhone: new FormControl((_get(supplier, 'xp.SupportContact') && _get(supplier, 'xp.SupportContact.Phone')) || ''),
        Active: new FormControl(supplier.Active),
    });
}
var SupplierTableComponent = /** @class */ (function (_super) {
    __extends(SupplierTableComponent, _super);
    function SupplierTableComponent(supplierService, changeDetectorRef, router, activatedroute, ngZone) {
        var _this = _super.call(this, changeDetectorRef, supplierService, router, activatedroute, ngZone, createSupplierForm) || this;
        _this.supplierService = supplierService;
        // these are custom filters for SEB that should be moved out of this code and into
        // external storage in the future, filters that apply to all of marketplace
        // can also be added to this before passing into the resource table
        _this.filterConfig = {
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
        _this.router = router;
        return _this;
    }
    var _a;
    SupplierTableComponent = __decorate([
        Component({
            selector: 'app-supplier-table',
            templateUrl: './supplier-table.component.html',
            styleUrls: ['./supplier-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof SupplierService !== "undefined" && SupplierService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone])
    ], SupplierTableComponent);
    return SupplierTableComponent;
}(ResourceCrudComponent));
export { SupplierTableComponent };
//# sourceMappingURL=supplier-table.component.js.map