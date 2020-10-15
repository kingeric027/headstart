import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject, Input, Output } from '@angular/core';
import { MonitoredProductFieldModifiedNotificationDocument, NotificationStatus } from '@app-seller/shared/models/monitored-product-field-modified-notification.interface';
import { JDocument, PriceBreak, SuperMarketplaceProduct } from '@ordercloud/headstart-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { EventEmitter } from 'protractor';

@Component({
  selector: 'notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent {
  @Input() notification: JDocument;
  @Output() onActionTaken = new EventEmitter;
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

  getJsonValues(jsonObject): any | PriceBreak[] {
    if (jsonObject !== Object(jsonObject)) {
      return String(jsonObject);
    }
    for (var key in jsonObject) {
      if (jsonObject[key] instanceof Object && jsonObject[key][0]?.Price >= 0) {
        return this.getPriceBreaks(jsonObject[key]);
      }
    }
  }

  getPriceBreaks(jsonObject): PriceBreak[] {
    this.hasPriceBreak = true;
    let priceBreakList: PriceBreak[] = [];
    jsonObject.forEach(element => {
      let priceBreak;
      priceBreak = { Quantity: element.Quantity, Price: element.Price };
      priceBreakList.push(priceBreak);
    });
    return priceBreakList;
  }
}
