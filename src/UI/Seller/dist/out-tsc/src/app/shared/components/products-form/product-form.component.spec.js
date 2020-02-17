import { __assign } from "tslib";
import { async, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserModule } from '@angular/platform-browser';
import { ProductFormComponent } from '@app-seller/shared/components/products-form/product-form.component';
describe('ProductsFormComponent', function () {
    var component;
    var fixture;
    var mockProduct = {
        ID: '1',
        Name: 'Products',
        Description: 'Description',
        Active: true,
        xp: { Featured: false },
    };
    var formErrorService = {
        hasRequiredError: jasmine.createSpy('hasRequiredError'),
        displayFormErrors: jasmine.createSpy('displayFormErrors'),
        hasInvalidIdError: jasmine.createSpy('hasInvalidIdError'),
        hasPatternError: jasmine.createSpy('hasPatternError'),
    };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [ProductFormComponent],
            imports: [ReactiveFormsModule, RouterTestingModule, BrowserModule],
            providers: [
                FormBuilder,
                { provide: AppFormErrorService, useValue: formErrorService },
            ],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(ProductFormComponent);
        component = fixture.componentInstance;
        component.existingProduct = mockProduct;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('ngOnInit', function () {
        beforeEach(function () {
            component.ngOnInit();
        });
        it('should initialize form correctly', function () {
            expect(component.productForm.value).toEqual({
                ID: '1',
                Name: 'Products',
                Description: 'Description',
                Active: true,
                Featured: false,
            });
        });
    });
    describe('onSubmit', function () {
        beforeEach(function () {
            spyOn(component.formSubmitted, 'emit');
        });
        it('should call displayFormErrors if form is invalid', function () {
            component.productForm.setErrors({ test: true });
            component['onSubmit']();
            expect(formErrorService.displayFormErrors).toHaveBeenCalled();
        });
        it('should emit formSubmitted event', function () {
            component['onSubmit']();
            expect(component.formSubmitted.emit).toHaveBeenCalledWith(__assign(__assign({}, mockProduct), { Featured: false }));
        });
    });
    describe('hasRequiredError', function () {
        beforeEach(function () {
            component['hasRequiredError']('FirstName');
        });
        it('should call formErrorService.hasRequiredError', function () {
            expect(formErrorService.hasRequiredError).toHaveBeenCalledWith('FirstName', component.productForm);
        });
    });
});
//# sourceMappingURL=product-form.component.spec.js.map