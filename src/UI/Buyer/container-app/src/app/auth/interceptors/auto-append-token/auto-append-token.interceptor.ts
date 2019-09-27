import { Injectable, Inject } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { applicationConfiguration, AppConfig } from 'src/app/config/app.config';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

/**
 * automatically append token to the authorization header
 * required to interact with middleware layer
 */
@Injectable({
  providedIn: 'root',
})
export class AutoAppendTokenInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService, @Inject(applicationConfiguration) private appConfig: AppConfig) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.includes(this.appConfig.middlewareUrl)) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.authService.getOrderCloudToken()}`,
        },
      });
    }
    return next.handle(request);
  }
}
