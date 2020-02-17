import { async, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { HttpClientModule } from '@angular/common/http';
import { InjectionToken } from '@angular/core';
import { ForgotPasswordComponent } from '@app-seller/auth/containers/forgot-password/forgot-password.component';
import { applicationConfiguration, } from '@app-seller/config/app.config';
import { OcPasswordResetService, } from '@ordercloud/angular-sdk';
import { CookieModule } from 'ngx-cookie';
import { ToastrService } from 'ngx-toastr';
describe('ForgotPasswordComponent', function () {
    var component;
    var fixture;
    var router = { navigateByUrl: jasmine.createSpy('navigateByUrl') };
    var ocPasswordService = {
        SendVerificationCode: jasmine
            .createSpy('SendVerificationCode')
            .and.returnValue(of(true)),
    };
    var toastrService = { success: jasmine.createSpy('success') };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [ForgotPasswordComponent],
            imports: [ReactiveFormsModule, CookieModule.forRoot(), HttpClientModule],
            providers: [
                { provide: Router, useValue: router },
                { provide: OcPasswordResetService, useValue: ocPasswordService },
                { provide: ToastrService, useValue: toastrService },
                {
                    provide: applicationConfiguration,
                    useValue: new InjectionToken('app.config'),
                },
            ],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(ForgotPasswordComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('ngOnInit', function () {
        var formbuilder = new FormBuilder();
        beforeEach(function () {
            component.ngOnInit();
        });
        it('should set the form values to empty strings', function () {
            expect(component.resetEmailForm.value).toEqual({
                email: '',
            });
        });
    });
    describe('onSubmit', function () {
        beforeEach(function () {
            component['appConfig'].clientID = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx';
            component.onSubmit();
        });
        it('should call the PasswordService SendVerificationCode method, Toaster success method, and route to login', function () {
            expect(ocPasswordService.SendVerificationCode).toHaveBeenCalledWith({
                Email: '',
                ClientID: 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx',
                URL: 'http://localhost:9876',
            });
            expect(toastrService.success).toHaveBeenCalledWith('Password Reset Email Sent!');
            expect(router.navigateByUrl).toHaveBeenCalledWith('/login');
        });
    });
});
//# sourceMappingURL=forgot-password.component.spec.js.map