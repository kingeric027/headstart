import { Injectable, Inject } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { applicationConfiguration } from 'src/app/config/app.config';
import { AppConfig } from 'shopper-context-interface';
import { OcTokenService } from '@ordercloud/angular-sdk';

/**
 * automatically append token to the authorization header
 * required to interact with middleware layer
 */
@Injectable({
  providedIn: 'root',
})
export class AutoAppendTokenInterceptor implements HttpInterceptor {
  constructor(private ocTokenService: OcTokenService, @Inject(applicationConfiguration) private appConfig: AppConfig) {}
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
