import { __decorate, __metadata } from "tslib";
import { Injectable } from '@angular/core';
/**
 * append headers to disable IE11's aggressive caching of GET requests
 * see here for more info on bug: https://medium.com/@tltjr/disabling-internet-explorer-caching-in-your-angular-5-application-3e148f4437ad
 *
 * we've augmented the suggested solution to check  first if the browser
 * is IE11 and to ensure the request is a 'GET' as those are the affected calls
 * we'd like to keep caching mechanisms for other browsers and request types because
 * some things should rightfully be cached
 */
var CacheInterceptor = /** @class */ (function () {
    function CacheInterceptor() {
    }
    CacheInterceptor.prototype.intercept = function (request, next) {
        var hasIE11 = window.navigator.userAgent.includes('Trident/');
        if (hasIE11 && request.method === 'GET') {
            request = request.clone({
                setHeaders: {
                    'Cache-Control': 'no-cache',
                    Pragma: 'no-cache',
                    Expires: 'Sat, 01 Jan 2000 00:00:00 GMT',
                },
            });
        }
        return next.handle(request);
    };
    CacheInterceptor = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [])
    ], CacheInterceptor);
    return CacheInterceptor;
}());
export { CacheInterceptor };
//# sourceMappingURL=cache-interceptor.js.map