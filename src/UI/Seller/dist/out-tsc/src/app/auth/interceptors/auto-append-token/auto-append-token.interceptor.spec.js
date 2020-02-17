import { TestBed, inject } from '@angular/core/testing';
import { HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController, } from '@angular/common/http/testing';
import { AutoAppendTokenInterceptor } from '@app-seller/auth/interceptors/auto-append-token/auto-append-token.interceptor';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { CookieModule } from 'ngx-cookie';
describe('AutoAppendTokenInterceptor', function () {
    var mockToken = 'ABC123';
    var mockMiddlewareUrl = 'my-integration-path/api';
    var tokenService = {
        GetAccess: jasmine.createSpy('GetAccess').and.returnValue(mockToken),
    };
    var appConfig = { middlewareUrl: mockMiddlewareUrl };
    var httpClient;
    var httpMock;
    beforeEach(function () {
        TestBed.configureTestingModule({
            imports: [CookieModule.forRoot(), HttpClientTestingModule],
            providers: [
                { provide: OcTokenService, useValue: tokenService },
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: AutoAppendTokenInterceptor,
                    multi: true,
                },
                { provide: applicationConfiguration, useValue: appConfig },
            ],
        });
        httpClient = TestBed.get(HttpClient);
        httpMock = TestBed.get(HttpTestingController);
    });
    it('should be created', inject([AutoAppendTokenInterceptor], function (service) {
        expect(service).toBeTruthy();
    }));
    describe('making http calls', function () {
        it('should add authorization headers to integration calls', function () {
            httpClient.get(mockMiddlewareUrl + "/data").subscribe(function (response) {
                expect(response).toBeTruthy();
            });
            var req = httpMock.expectOne(mockMiddlewareUrl + "/data");
            expect(req.request.headers.get('Authorization')).toEqual("Bearer " + mockToken);
            req.flush({ hello: 'World' });
            httpMock.verify();
        });
        it('should not add authorization headers if call is not to integrations url', function () {
            httpClient.get('/data').subscribe(function (response) {
                expect(response).toBeTruthy();
            });
            var req = httpMock.expectOne('/data');
            expect(req.request.headers.get('Authorization')).toBe(null);
            req.flush({ hello: 'World' });
            httpMock.verify();
        });
    });
});
//# sourceMappingURL=auto-append-token.interceptor.spec.js.map