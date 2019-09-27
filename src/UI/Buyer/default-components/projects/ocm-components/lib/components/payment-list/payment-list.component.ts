import { Component, Input } from '@angular/core';
import { ListPayment } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.scss'],
})
export class OCMPaymentList extends OCMComponent {
  @Input() payments: ListPayment;
}
