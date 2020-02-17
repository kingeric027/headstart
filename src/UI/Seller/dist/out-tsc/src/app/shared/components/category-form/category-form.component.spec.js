import { async, TestBed } from '@angular/core/testing';
import { CategoryFormComponent } from './category-form.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
describe('CategoryFormComponent', function () {
    var component;
    var fixture;
    var formErrorService = {
        hasRequiredError: jasmine.createSpy('hasRequiredError'),
        hasInvalidIdError: jasmine.createSpy('hasInValidIdError'),
        displayFormErrors: jasmine.createSpy('displayFormErrors'),
        hasPatternError: jasmine.createSpy('hasPatternError'),
    };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [CategoryFormComponent],
            imports: [ReactiveFormsModule],
            providers: [
                FormBuilder,
                { provide: AppFormErrorService, useValue: formErrorService },
            ],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(CategoryFormComponent);
        component = fixture.componentInstance;
        component.existingCategory = {
            ID: '1',
            Name: 'Category',
            Description: 'Description',
            Active: true,
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
            expect(component.categoryForm.value).toEqual({
                ID: '1',
                Name: 'Category',
                Description: 'Description',
                Active: true,
            });
        });
    });
    describe('onSubmit', function () {
        beforeEach(function () {
            spyOn(component.formSubmitted, 'emit');
        });
        it('should call displayFormErrors if form is invalid', function () {
            component.categoryForm.setErrors({ test: true });
            component['onSubmit']();
            expect(formErrorService.displayFormErrors).toHaveBeenCalled();
        });
        it('should emit formSubmitted event', function () {
            component['onSubmit']();
            expect(component.formSubmitted.emit).toHaveBeenCalledWith({
                ID: '1',
                Name: 'Category',
                Description: 'Description',
                Active: true,
            });
        });
    });
    describe('hasRequiredError', function () {
        beforeEach(function () {
            component['hasRequiredError']('FirstName');
        });
        it('should call formErrorService.hasRequiredError', function () {
            expect(formErrorService.hasRequiredError).toHaveBeenCalledWith('FirstName', component.categoryForm);
        });
    });
});
//# sourceMappingURL=category-form.component.spec.js.map