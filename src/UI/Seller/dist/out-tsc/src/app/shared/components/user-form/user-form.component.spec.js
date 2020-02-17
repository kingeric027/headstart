import { async, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared';
import { UserFormComponent } from '@app-seller/shared/components/user-form/user-form.component';
describe('UserFormComponent', function () {
    var component;
    var fixture;
    var mockUser = {
        ID: '1',
        Username: 'Products',
        FirstName: 'First',
        LastName: 'Second',
        Email: 'test@email.com',
        Phone: '123-456-7890',
        Active: true,
    };
    var formErrorService = {
        hasRequiredError: jasmine.createSpy('hasRequiredError'),
        displayFormErrors: jasmine.createSpy('displayFormErrors'),
        hasValidEmailError: jasmine.createSpy('hasValidEmailError'),
        hasPatternError: jasmine.createSpy('hasPatternError'),
    };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [UserFormComponent],
            imports: [ReactiveFormsModule],
            providers: [
                FormBuilder,
                { provide: AppFormErrorService, useValue: formErrorService },
            ],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(UserFormComponent);
        component = fixture.componentInstance;
        component.existingUser = mockUser;
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
            expect(component.userForm.value).toEqual({
                ID: '1',
                Username: 'Products',
                FirstName: 'First',
                LastName: 'Second',
                Email: 'test@email.com',
                Phone: '123-456-7890',
                Active: true,
            });
        });
    });
    describe('onSubmit', function () {
        beforeEach(function () {
            spyOn(component.formSubmitted, 'emit');
        });
        it('should call displayFormErrors if form is invalid', function () {
            component.userForm.setErrors({ test: true });
            component['onSubmit']();
            expect(formErrorService.displayFormErrors).toHaveBeenCalled();
        });
        it('should emit formSubmitted event', function () {
            component.userForm.setErrors(null);
            component.userForm.get('ID').setValue('newID');
            component['onSubmit']();
            expect(component.formSubmitted.emit).toHaveBeenCalledWith({
                user: {
                    ID: 'newID',
                    Username: 'Products',
                    FirstName: 'First',
                    LastName: 'Second',
                    Email: 'test@email.com',
                    Phone: '123-456-7890',
                    Active: true,
                },
                prevID: '1',
            });
        });
    });
    describe('hasRequiredError', function () {
        beforeEach(function () {
            component['hasRequiredError']('FirstName');
        });
        it('should call formErrorService.hasRequiredError', function () {
            expect(formErrorService.hasRequiredError).toHaveBeenCalledWith('FirstName', component.userForm);
        });
    });
});
//# sourceMappingURL=user-form.component.spec.js.map