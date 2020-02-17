import { TestBed } from '@angular/core/testing';
import { HasTokenGuard } from '@app-seller/shared/guards/has-token/has-token.guard';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { Router } from '@angular/router';
import { AppAuthService } from '@app-seller/auth';
import { of } from 'rxjs';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { AppStateService } from '@app-seller/shared/services/app-state/app-state.service';
describe('HasTokenGuard', function () {
    var guard;
    var mockAccessToken = null;
    var rememberMe = false;
    // tslint:disable-next-line:max-line-length
    var validToken = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c3IiOiJhbm9uX3VzZXIiLCJjaWQiOiI4MDIxODkzNi0zNTBiLTQxMDUtYTFmYy05NjJhZjAyM2Q2NjYiLCJvcmRlcmlkIjoiSVlBSnFOWVVpRVdyTy1Lei1TalpqUSIsInVzcnR5cGUiOiJidXllciIsInJvbGUiOlsiQnV5ZXJSZWFkZXIiLCJNZUFkbWluIiwiTWVDcmVkaXRDYXJkQWRtaW4iLCJNZUFkZHJlc3NBZG1pbiIsIk1lWHBBZG1pbiIsIlBhc3N3b3JkUmVzZXQiLCJTaGlwbWVudFJlYWRlciIsIlNob3BwZXIiLCJBZGRyZXNzUmVhZGVyIl0sImlzcyI6Imh0dHBzOi8vYXV0aC5vcmRlcmNsb3VkLmlvIiwiYXVkIjoiaHR0cHM6Ly9hcGkub3JkZXJjbG91ZC5pbyIsImV4cCI6MTUyNzA5Nzc0MywibmJmIjoxNTI2NDkyOTQzfQ.MBV7dqBq8RXSZZ8vEGidcfH8vSwOR55yHzvAq1w2bLc';
    var mockRefreshToken = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx';
    var appConfig = { anonymousShoppingEnabled: true };
    var tokenService = {
        GetAccess: jasmine
            .createSpy('GetAccess')
            .and.callFake(function () { return mockAccessToken; }),
        GetRefresh: jasmine
            .createSpy('GetRefresh')
            .and.returnValue(of(mockRefreshToken)),
    };
    var router = { navigate: jasmine.createSpy('navigate') };
    var appAuthService = {
        authAnonymous: jasmine.createSpy('authAnonymous').and.returnValue(of(null)),
        getRememberStatus: jasmine
            .createSpy('getRememberStatus')
            .and.callFake(function () { return rememberMe; }),
        refresh: jasmine.createSpy('refresh').and.returnValue(of(null)),
    };
    var appStateService = {
        isLoggedIn: { next: jasmine.createSpy('next').and.returnValue(null) },
    };
    beforeEach(function () {
        TestBed.configureTestingModule({
            imports: [],
            providers: [
                { provide: applicationConfiguration, useValue: appConfig },
                { provide: AppAuthService, useValue: appAuthService },
                { provide: Router, useValue: router },
                { provide: OcTokenService, useValue: tokenService },
                { provide: AppStateService, useValue: appStateService },
            ],
        });
        guard = TestBed.get(HasTokenGuard);
    });
    // set Date.now for consistent test results
    var originalDateNow = Date.now;
    beforeAll(function () {
        var mockDateNow = function () { return 1526497620725; };
        Date.now = mockDateNow;
    });
    afterAll(function () {
        Date.now = originalDateNow;
    });
    it('should ...', function () {
        expect(guard).toBeTruthy();
    });
    describe('canActivate', function () {
        describe('user is logged in', function () {
            beforeEach(function () {
                appConfig.anonymousShoppingEnabled = false;
            });
            it('should return true if token is valid', function () {
                mockAccessToken = validToken;
                guard.canActivate().subscribe(function (isTokenValid) {
                    expect(appAuthService.authAnonymous).not.toHaveBeenCalled();
                    expect(appStateService.isLoggedIn.next).toHaveBeenCalledWith(true);
                    expect(isTokenValid).toBe(true);
                });
            });
            it('should return false if token is invalid', function () {
                mockAccessToken = null;
                guard.canActivate().subscribe(function (isTokenValid) {
                    expect(appAuthService.authAnonymous).not.toHaveBeenCalled();
                    expect(isTokenValid).toBe(false);
                });
            });
        });
        describe('access token timed out but user has refresh token', function () {
            it('should call refresh', function () {
                mockAccessToken = null;
                rememberMe = true;
                guard.canActivate().subscribe(function (isTokenValid) {
                    expect(appAuthService.refresh).toHaveBeenCalled();
                    expect(isTokenValid).toBe(true);
                });
            });
        });
    });
    describe('isTokenValid', function () {
        it('should return false if token does not exist', function () {
            mockAccessToken = null;
            var isTokenValid = guard['isTokenValid']();
            expect(isTokenValid).toBe(false);
        });
        it('it should return false if token can not be parsed', function () {
            mockAccessToken = 'cant_parse_this_hammertime';
            var isTokenValid = guard['isTokenValid']();
            expect(isTokenValid).toBe(false);
        });
        describe('expiration time', function () {
            it('should return false if expiresIn time is less than current time', function () {
                // decodedToken.exp set to 1526497620
                // tslint:disable-next-line:max-line-length
                var lessThanCurrentTime = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c3IiOiJhbm9uX3VzZXIiLCJjaWQiOiI4MDIxODkzNi0zNTBiLTQxMDUtYTFmYy05NjJhZjAyM2Q2NjYiLCJvcmRlcmlkIjoiSVlBSnFOWVVpRVdyTy1Lei1TalpqUSIsInVzcnR5cGUiOiJidXllciIsInJvbGUiOlsiQnV5ZXJSZWFkZXIiLCJNZUFkbWluIiwiTWVDcmVkaXRDYXJkQWRtaW4iLCJNZUFkZHJlc3NBZG1pbiIsIk1lWHBBZG1pbiIsIlBhc3N3b3JkUmVzZXQiLCJTaGlwbWVudFJlYWRlciIsIlNob3BwZXIiLCJBZGRyZXNzUmVhZGVyIl0sImlzcyI6Imh0dHBzOi8vYXV0aC5vcmRlcmNsb3VkLmlvIiwiYXVkIjoiaHR0cHM6Ly9hcGkub3JkZXJjbG91ZC5pbyIsImV4cCI6MTUyNjQ5NzYyMCwibmJmIjoxNTI2NDkyOTQzfQ.W1GyDrOUyRxs8GZSiW0jk__37Cv98t2A_u7AK2PaMtU';
                mockAccessToken = lessThanCurrentTime;
                var isTokenValid = guard['isTokenValid']();
                expect(isTokenValid).toBe(false);
            });
            it('should return true if expiresIn time is greater than current time', function () {
                // decodedToken.exp set to 1526497621
                // tslint:disable-next-line:max-line-length
                var greaterThanCurrentTime = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c3IiOiJhbm9uX3VzZXIiLCJjaWQiOiI4MDIxODkzNi0zNTBiLTQxMDUtYTFmYy05NjJhZjAyM2Q2NjYiLCJvcmRlcmlkIjoiSVlBSnFOWVVpRVdyTy1Lei1TalpqUSIsInVzcnR5cGUiOiJidXllciIsInJvbGUiOlsiQnV5ZXJSZWFkZXIiLCJNZUFkbWluIiwiTWVDcmVkaXRDYXJkQWRtaW4iLCJNZUFkZHJlc3NBZG1pbiIsIk1lWHBBZG1pbiIsIlBhc3N3b3JkUmVzZXQiLCJTaGlwbWVudFJlYWRlciIsIlNob3BwZXIiLCJBZGRyZXNzUmVhZGVyIl0sImlzcyI6Imh0dHBzOi8vYXV0aC5vcmRlcmNsb3VkLmlvIiwiYXVkIjoiaHR0cHM6Ly9hcGkub3JkZXJjbG91ZC5pbyIsImV4cCI6MTUyNjQ5NzYyMSwibmJmIjoxNTI2NDkyOTQzfQ.EQ587x_hiCLu0hW6zTp-XxcXDUZdJjB5wFYC_RYqsf0';
                mockAccessToken = greaterThanCurrentTime;
                var isTokenValid = guard['isTokenValid']();
                expect(isTokenValid).toBe(true);
            });
        });
    });
});
//# sourceMappingURL=has-token.guard.spec.js.map