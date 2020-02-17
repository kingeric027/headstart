import { TestBed, inject } from '@angular/core/testing';
import { InjectionToken } from '@angular/core';
import { HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController, } from '@angular/common/http/testing';
import { RefreshTokenInterceptor } from '@app-seller/auth/interceptors/refresh-token/refresh-token.interceptor';
import { applicationConfiguration, } from '@app-seller/config/app.config';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { CookieModule } from 'ngx-cookie';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { of, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';
describe('RefreshTokenInterceptor', function () {
    var mockRefreshToken = 'RefreshABC123';
    var tokenService = {
        GetAccess: jasmine
            .createSpy('GetAccess')
            .and.returnValue(of(mockRefreshToken)),
    };
    var refreshToken = new BehaviorSubject('');
    var appAuthService = {
        refreshToken: refreshToken,
        fetchingRefreshToken: false,
        failedRefreshAttempt: false,
        refresh: jasmine.createSpy('refresh').and.returnValue(of(mockRefreshToken)),
    };
    var httpClient;
    var httpMock;
    beforeEach(function () {
        TestBed.configureTestingModule({
            imports: [CookieModule.forRoot(), HttpClientTestingModule],
            providers: [
                { provide: AppAuthService, useValue: appAuthService },
                { provide: OcTokenService, useValue: tokenService },
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: RefreshTokenInterceptor,
                    multi: true,
                },
                {
                    provide: applicationConfiguration,
                    useValue: new InjectionToken('app.config'),
                },
            ],
        });
        httpClient = TestBed.get(HttpClient);
        httpMock = TestBed.get(HttpTestingController);
    });
    it('should be created', inject([RefreshTokenInterceptor], function (service) {
        expect(service).toBeTruthy();
    }));
    describe('making http calls', function () {
        var thrownError = '';
        it('should rethrow error on non-ordercloud http calls', function () {
            httpClient
                .get('/data')
                .pipe(catchError(function (ex) {
                thrownError = ex.error;
                return 'GoToSubscribe';
            }))
                .subscribe(function () {
                expect(thrownError).toBe('mockError');
            });
            var req = httpMock.expectOne('/data');
            req.flush('mockError', { status: 401, statusText: 'Unauthorized' });
            httpMock.verify();
        });
        it('should rethrow error if status code is not 401', function () {
            httpClient
                .get('https://api.ordercloud.io/v1/me')
                .pipe(catchError(function (ex) {
                thrownError = ex.error;
                return 'GoToSubscribe';
            }))
                .subscribe(function () {
                expect(thrownError).toBe('mockError');
            });
            var req = httpMock.expectOne('https://api.ordercloud.io/v1/me');
            req.flush('mockError', { status: 500, statusText: 'Unauthorized' });
            httpMock.verify();
        });
        describe('refresh operation', function () {
            var firstRequest;
            var secondRequest;
            var setupMockCalls = function () {
                // call http client
                httpClient.get('https://api.ordercloud.io/v1/me').subscribe();
                // first request that "fails" but is caught
                firstRequest = httpMock.expectOne('https://api.ordercloud.io/v1/me');
                firstRequest.flush('mockBody', {
                    status: 401,
                    statusText: 'Unauthorized',
                });
                // second request that goes out as a consequence of the refresh operation
                secondRequest = httpMock.expectOne('https://api.ordercloud.io/v1/me');
                secondRequest.flush('mockBody');
            };
            it('should call appAuthService.refresh', function () {
                setupMockCalls();
                expect(appAuthService.refresh).toHaveBeenCalled();
            });
            it('should set refresh token on second call', function () {
                setupMockCalls();
                expect(firstRequest.request.headers.get('Authorization')).toBe(null);
                expect(secondRequest.request.headers.get('Authorization')).toBe("Bearer " + mockRefreshToken);
                appAuthService.failedRefreshAttempt = true;
            });
            afterEach(function () {
                httpMock.verify();
            });
        });
    });
});
//# sourceMappingURL=refresh-token.interceptor.spec.js.map