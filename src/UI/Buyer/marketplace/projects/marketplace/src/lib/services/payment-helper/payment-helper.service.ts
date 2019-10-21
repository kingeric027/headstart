import { Injectable } from '@angular/core';
import { ListPayment, OcPaymentService, OcMeService, Payment } from '@ordercloud/angular-sdk';

@Injectable({
  providedIn: 'root',
})
export class PaymentHelperService {
  constructor(private ocPaymentService: OcPaymentService, private ocMeService: OcMeService) {}

  async ListPaymentsOnOrder(orderID: string): Promise<ListPayment> {
    const payments = await this.ocPaymentService.List('outgoing', orderID).toPromise();
    const withDetails = payments.Items.map((payment) => this.setPaymentDetails(payment));
    const Items = await Promise.all(withDetails);
    return { Items, Meta: payments.Meta };
  }

  private async setPaymentDetails(payment: Payment): Promise<Payment> {
    const details = await this.getPaymentDetails(payment);
    (payment as any).Details = details;
    return payment;
  }

  private async getPaymentDetails(payment: Payment): Promise<any> {
    switch (payment.Type) {
      case 'CreditCard':
        return this.ocMeService.GetCreditCard(payment.CreditCardID).toPromise();
      case 'SpendingAccount':
        return this.ocMeService.GetSpendingAccount(payment.SpendingAccountID).toPromise();
      case 'PurchaseOrder':
        return Promise.resolve({ PONumber: payment.xp.PONumber });
    }
  }
}
