import { Component, Input } from '@angular/core';
import { JDocument } from '@ordercloud/headstart-sdk';

@Component({
  selector: 'notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent {
    @Input() notification: JDocument;  
  constructor() { }

  notificationAccept(action: string) {
      console.log(action);
  }
}