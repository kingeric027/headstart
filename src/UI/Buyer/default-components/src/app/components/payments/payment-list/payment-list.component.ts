import { Component, Input } from '@angular/core';
import { Payment } from 'ordercloud-javascript-sdk';

@Component({
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.scss'],
})
export class OCMPaymentList {
  @Input() payments: Payment;
}
