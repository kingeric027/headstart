import { TestBed, inject } from '@angular/core/testing';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { OcAuthService, OcTokenService, Configuration, } from '@ordercloud/angular-sdk';
import { CookieModule, CookieService } from 'ngx-cookie';
import { RouterTestingModule } from '@angular/router/testing';
import { AppErrorHandler } from '@app-seller/config/error-handling.config';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AppStateService } from '@app-seller/shared';
describe('AppAuthService', function () {
    var mockCookieResponse = {
        'mgr-test_cookieOne': 'cookieone',
        'mgr-test_cookieTwo': 'cookietwo',
        otherCookie: 'othercookie',
    };
    var mockRefreshToken = 'mock refresh token';
    var mockToken = 'mock token';
    var mockClientID = 'mockClientID';
    var mockAppName = 'mgr-test';
    var router = { navigate: jasmine.createSpy('navigate') };
    var cookieService = {
        getObject: jasmine.createSpy('getObject').and.returnValue({ status: true }),
        putObject: jasmine.createSpy('putObject'),
        getAll: jasmine.createSpy('getAll').and.returnValue(mockCookieResponse),
        remove: jasmine.createSpy('remove'),
    };
    var appAuthService;
    var authService;
    var tokenService;
    var appConfig = {
        appname: mockAppName,
        clientID: mockClientID,
        anonymousShoppingEnabled: true,
        scope: ['FullAccess'],
    };
    var appErrorHandler = { displayError: jasmine.createSpy('displayError') };
    var appStateService = {
        isLoggedIn: { next: jasmine.createSpy('next').and.returnValue(null) },
    };
    beforeEach(function () {
        TestBed.configureTestingModule({
            imports: [
                RouterTestingModule,
                HttpClientTestingModule,
                CookieModule.forRoot(),
            ],
            providers: [
                { provide: Router, useValue: router },
                { provide: CookieService, useValue: cookieService },
                { provide: AppErrorHandler, useValue: appErrorHandler },
                { provide: Configuration, useValue: new Configuration() },
                { provide: applicationConfiguration, useValue: appConfig },
                { provide: AppStateService, useValue: appStateService },
            ],
        });
        appConfig = TestBed.get(applicationConfiguration);
        tokenService = TestBed.get(OcTokenService);
        appAuthService = TestBed.get(AppAuthService);
        authService = TestBed.get(OcAuthService);
    });
    it('should be created', inject([AppAuthService], function (service) {
        expect(service).toBeTruthy();
    }));
    describe('refresh', function () {
        describe('on success', function () {
            beforeEach(function () {
                spyOn(appAuthService, 'fetchRefreshToken').and.returnValue(of(mockToken));
                spyOn(appAuthService, 'logout').and.returnValue(of(null));
                spyOn(tokenService, 'SetAccess');
                appAuthService.refresh().subscribe();
            });
            it('should call fetchRefreshToken', function () {
                expect(appAuthService.fetchRefreshToken).toHaveBeenCalled();
            });
            it('should save the retrieved refresh token', function () {
                expect(tokenService.SetAccess).toHaveBeenCalledWith(mockToken);
            });
            it('should set isLoggedIn to true', function () {
                expect(appStateService.isLoggedIn.next).toHaveBeenCalledWith(true);
            });
        });
        describe('on error', function () {
            beforeEach(function () {
                spyOn(tokenService, 'GetAccess').and.returnValue(mockToken);
                spyOn(appAuthService, 'logout');
                spyOn(appAuthService, 'fetchRefreshToken').and.returnValue(throwError('Token refresh attempt not possible'));
                appAuthService.refresh().subscribe();
            });
            it('should set failedRefreshAttempt to true', function () {
                expect(appAuthService.failedRefreshAttempt).toBe(true);
            });
            it('should log user out', function () {
                expect(appAuthService.logout).toHaveBeenCalled();
            });
        });
    });
    describe('fetchToken', function () {
        beforeEach(function () {
            spyOn(appAuthService, 'fetchRefreshToken');
        });
        it('should return access token if it is available', function () {
            spyOn(tokenService, 'GetAccess').and.returnValue('mockToken');
            appAuthService.fetchToken();
            expect(appAuthService.fetchRefreshToken).not.toHaveBeenCalled();
        });
        it('should call fetchRefresh token if no access token is available', function () {
            spyOn(tokenService, 'GetAccess').and.returnValue(null);
            appAuthService.fetchToken();
            expect(appAuthService.fetchRefreshToken).toHaveBeenCalled();
        });
    });
    describe('fetchRefreshToken', function () {
        describe('and has refresh token', function () {
            beforeEach(function () {
                spyOn(tokenService, 'GetRefresh').and.returnValue(mockRefreshToken);
            });
            it('should call authService.RefreshToken', function () {
                spyOn(authService, 'RefreshToken').and.returnValue(of({ access_token: mockToken }));
                appAuthService.fetchRefreshToken();
                expect(authService.RefreshToken).toHaveBeenCalledWith(mockRefreshToken, mockClientID);
            });
        });
        describe('logout', function () {
            beforeEach(function () {
                appAuthService.logout();
            });
            it('should clear out cookies that start with appname', function () {
                expect(cookieService.getAll).toHaveBeenCalled();
                expect(cookieService.remove).toHaveBeenCalledWith('mgr-test_cookieOne');
                expect(cookieService.remove).toHaveBeenCalledWith('mgr-test_cookieTwo');
                expect(cookieService.remove).not.toHaveBeenCalledWith('otherCookie');
            });
            it('should navigate user to login page', function () {
                expect(router.navigate).toHaveBeenCalledWith(['/login']);
            });
            it('should set isLoggedIn to false', function () {
                expect(appStateService.isLoggedIn.next).toHaveBeenCalledWith(false);
            });
        });
        describe('setRememberStatus', function () {
            var statusTrue = true;
            beforeEach(function () {
                appAuthService.setRememberStatus(statusTrue);
            });
            it('should store status in cookies', function () {
                expect(cookieService.putObject).toHaveBeenCalledWith('mgr-test_rememberMe', { status: statusTrue });
            });
        });
        describe('getRememberStatus', function () {
            it('should return status stored in cookies', function () {
                var rememberMeStatus = appAuthService.getRememberStatus();
                expect(rememberMeStatus).toBe(true);
            });
        });
    });
});
//# sourceMappingURL=app-auth.service.spec.js.map