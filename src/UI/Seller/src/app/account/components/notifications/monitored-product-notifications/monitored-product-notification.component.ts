import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { MonitoredProductFieldModifiedNotificationDocument, NotificationStatus } from '@app-seller/shared/models/monitored-product-field-modified-notification.interface';
import { JDocument, PriceBreak, SuperMarketplaceProduct } from '@ordercloud/headstart-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { OcTokenService } from '@ordercloud/angular-sdk';

@Component({
  selector: 'monitored-product-notification',
  templateUrl: './monitored-product-notification.component.html',
  styleUrls: ['./monitored-product-notification.component.scss'],
})
export class MonitoredProductNotificationComponent {
  @Input() notification: MonitoredProductFieldModifiedNotificationDocument;
  @Output() onActionTaken = new EventEmitter<string>();
  @Output() goToProductPage = new EventEmitter<string>();
  hasPriceBreak = false;

  constructor(
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private currentUserService: CurrentUserService,
    private ocTokenService: OcTokenService
) {}

async reviewMonitoredFieldChange(status: NotificationStatus, notification: MonitoredProductFieldModifiedNotificationDocument): Promise<void> {
    const myContext = await this.currentUserService.getUserContext();
    notification.Doc.Status = status;
    notification.Doc.History.ReviewedBy = { ID: myContext?.Me?.ID, Name: `${myContext?.Me?.FirstName} ${myContext?.Me?.LastName}`};
    notification.Doc.History.DateReviewed = new Date().toISOString();
    const headers = {
      headers: new HttpHeaders({
          Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
      }),
    };
    // TODO: Replace with the SDK
    const superProduct = await this.http.put<SuperMarketplaceProduct>(`${this.appConfig.middlewareUrl}/notifications/monitored-product-field-modified/${notification.ID}`, notification, headers).toPromise()
    this.onActionTaken.emit("ACCEPTED");
  }
  navigateToProductPage(productId: string) {
    this.goToProductPage.emit(productId);
  }
}
