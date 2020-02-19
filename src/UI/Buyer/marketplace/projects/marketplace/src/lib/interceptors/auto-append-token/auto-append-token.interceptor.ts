import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig } from '../../shopper-context';

/**
 * automatically append token to the authorization header
 * required to interact with middleware layer
 */
@Injectable({
  providedIn: 'root',
})
export class AutoAppendTokenInterceptor implements HttpInterceptor {
  constructor(private ocTokenService: OcTokenService, private appConfig: AppConfig) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.includes(this.appConfig.middlewareUrl)) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
        },
      });
    }
    return next.handle(request);
  }
}
