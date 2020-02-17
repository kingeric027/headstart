import { async, TestBed } from '@angular/core/testing';
import { AddressFormComponent } from '@app-seller/shared/components/address-form/address-form.component';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { of } from 'rxjs';
import { OcMeService } from '@ordercloud/angular-sdk';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
describe('AddressFormComponent', function () {
    var component;
    var fixture;
    var meService = {
        CreateAddress: jasmine.createSpy('CreateAddress').and.returnValue(of(null)),
        PatchAddress: jasmine.createSpy('PatchAddress').and.returnValue(of(null)),
    };
    var formErrorService = {
        hasRequiredError: jasmine.createSpy('hasRequiredError'),
        hasInvalidIdError: jasmine.createSpy('hasInValidIdError'),
        displayFormErrors: jasmine.createSpy('displayFormErrors'),
        hasPatternError: jasmine.createSpy('hasPatternError'),
    };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [AddressFormComponent],
            imports: [ReactiveFormsModule],
            providers: [
                FormBuilder,
                { provide: AppFormErrorService, useValue: formErrorService },
                { provide: OcMeService, useValue: meService },
            ],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(AddressFormComponent);
        component = fixture.componentInstance;
        component.existingAddress = {
            ID: 'ID',
            AddressName: 'My Address',
            FirstName: 'Crhistian',
            LastName: 'Ramirez',
            Street1: '404 5th st sw',
            Street2: null,
            City: 'Minneapolis',
            State: 'MN',
            Zip: '56001',
            Phone: '555-555-5555',
            Country: 'US',
        };
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
            expect(component.addressForm.value).toEqual({
                ID: 'ID',
                AddressName: 'My Address',
                FirstName: 'Crhistian',
                LastName: 'Ramirez',
                Street1: '404 5th st sw',
                Street2: '',
                City: 'Minneapolis',
                State: 'MN',
                Zip: '56001',
                Phone: '555-555-5555',
                Country: 'US',
            });
        });
    });
    describe('onSubmit', function () {
        beforeEach(function () {
            spyOn(component.formSubmitted, 'emit');
        });
        it('should call displayFormErrors if form is invalid', function () {
            component.addressForm.setErrors({ test: true });
            component['onSubmit']();
            expect(formErrorService.displayFormErrors).toHaveBeenCalled();
        });
        it('should emit formSubmitted event', function () {
            component.addressForm.get('ID').setValue('newID');
            component['onSubmit']();
            expect(component.formSubmitted.emit).toHaveBeenCalledWith({
                address: {
                    ID: 'newID',
                    AddressName: 'My Address',
                    FirstName: 'Crhistian',
                    LastName: 'Ramirez',
                    Street1: '404 5th st sw',
                    Street2: '',
                    City: 'Minneapolis',
                    State: 'MN',
                    Zip: '56001',
                    Phone: '555-555-5555',
                    Country: 'US',
                },
                prevID: 'ID',
            });
        });
    });
    describe('hasRequiredError', function () {
        beforeEach(function () {
            component['hasRequiredError']('FirstName');
        });
        it('should call formErrorService.hasRequiredError', function () {
            expect(formErrorService.hasRequiredError).toHaveBeenCalledWith('FirstName', component.addressForm);
        });
    });
});
//# sourceMappingURL=address-form.component.spec.js.map