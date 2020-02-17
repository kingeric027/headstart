import { async, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InjectionToken } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { of, BehaviorSubject } from 'rxjs';
import { LoginComponent } from '@app-seller/auth/containers/login/login.component';
import { applicationConfiguration, } from '@app-seller/config/app.config';
import { OcAuthService, OcTokenService } from '@ordercloud/angular-sdk';
import { CookieModule } from 'ngx-cookie';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { AppStateService } from '@app-seller/shared';
describe('LoginComponent', function () {
    var component;
    var fixture;
    var debugElement;
    var router = { navigateByUrl: jasmine.createSpy('navigateByUrl') };
    var ocTokenService = {
        SetAccess: jasmine.createSpy('SetAccess'),
        SetRefresh: jasmine.createSpy('Refresh'),
    };
    var response = { access_token: '123456', refresh_token: 'refresh123456' };
    var ocAuthService = {
        Login: jasmine.createSpy('Login').and.returnValue(of(response)),
    };
    var appAuthService = {
        setRememberStatus: jasmine.createSpy('setRememberStatus'),
    };
    var appStateService = { isLoggedIn: new BehaviorSubject(false) };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [LoginComponent],
            imports: [ReactiveFormsModule, CookieModule.forRoot(), HttpClientModule],
            providers: [
                { provide: AppStateService, useValue: appStateService },
                { provide: AppAuthService, useValue: appAuthService },
                { provide: Router, useValue: router },
                { provide: OcTokenService, useValue: ocTokenService },
                { provide: OcAuthService, useValue: ocAuthService },
                {
                    provide: applicationConfiguration,
                    useValue: new InjectionToken('app.config'),
                },
            ],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(LoginComponent);
        component = fixture.componentInstance;
        debugElement = fixture.debugElement;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('ngOnInit', function () {
        beforeEach(function () {
            component.ngOnInit();
        });
        it('should set the form values to empty strings', function () {
            expect(component.form.value).toEqual({
                username: '',
                password: '',
                rememberMe: false,
            });
        });
    });
    describe('onSubmit', function () {
        beforeEach(function () {
            component['appConfig'].clientID = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx';
            component['appConfig'].scope = ['testScope'];
        });
        it('should call the OcAuthService Login method, OcTokenService SetAccess method, and route to home', function () {
            component.onSubmit();
            expect(ocAuthService.Login).toHaveBeenCalledWith('', '', 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx', ['testScope']);
            expect(ocTokenService.SetAccess).toHaveBeenCalledWith(response.access_token);
            expect(router.navigateByUrl).toHaveBeenCalledWith('/home');
        });
        it('should call set refresh token and set rememberStatus if rememberMe is true', function () {
            component.form.controls['rememberMe'].setValue(true);
            component.onSubmit();
            expect(ocTokenService.SetRefresh).toHaveBeenCalledWith('refresh123456');
            expect(appAuthService.setRememberStatus).toHaveBeenCalledWith(true);
        });
    });
});
//# sourceMappingURL=login.component.spec.js.map