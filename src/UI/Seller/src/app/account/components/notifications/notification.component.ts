import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { JDocument, PriceBreak } from '@ordercloud/headstart-sdk';

@Component({
  selector: 'notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent {
  @Input()
  notification: JDocument;
  constructor(private ref: ChangeDetectorRef) {}

  hasPriceBreak = false;
  notificationAccept(action: string) {
    console.log(action);
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
