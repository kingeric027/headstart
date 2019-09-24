import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { SpendingAccount, ListSpendingAccount, OcMeService, Payment, Order } from '@ordercloud/angular-sdk';
import * as moment from 'moment';

@Component({
  selector: 'checkout-payment-spending-account',
  templateUrl: './payment-spending-account.component.html',
  styleUrls: ['./payment-spending-account.component.scss'],
})
export class PaymentSpendingAccountComponent implements OnInit {
  @Input() order: Order;
  @Input() payment: Payment;
  @Output() paymentCreated = new EventEmitter<Payment>();
  @Output() continue = new EventEmitter();
  spendingAccounts: ListSpendingAccount;
  selectedSpendingAccount: SpendingAccount = null;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  resultsPerPage = 6;
  spendingAccountModalOpen = false;

  constructor(private ocMeService: OcMeService) {}

  ngOnInit() {
    this.listSpendingAccounts().subscribe((accounts) => {
      this.spendingAccounts = accounts;
      this.selectedSpendingAccount = this.getSavedSpendingAccount(accounts);
      if (!this.selectedSpendingAccount) {
        this.spendingAccountModalOpen = true;
      }
    });
  }

  listSpendingAccounts(): Observable<ListSpendingAccount> {
    const now = moment().format('YYYY-MM-DD');
    const filters = { StartDate: `<${now}|!*`, EndDate: `>${now}|!*` };
    return this.ocMeService.ListSpendingAccounts({
      filters,
      ...this.requestOptions,
      pageSize: this.resultsPerPage,
    });
  }

  getSavedSpendingAccount(accounts: ListSpendingAccount): SpendingAccount {
    if (this.payment && this.payment.SpendingAccountID) {
      const saved = accounts.Items.filter((x) => x.ID === this.payment.SpendingAccountID);
      if (saved.length > 0) {
        return saved[0];
      }
    }
    return null;
  }

  accountSelected(account: SpendingAccount): void {
    this.spendingAccountModalOpen = false;
    this.selectedSpendingAccount = account;
    const payment: Payment = {
      Type: 'SpendingAccount',
      SpendingAccountID: account.ID,
      Accepted: true,
    };
    this.paymentCreated.emit(payment);
  }

  validateAndContinue() {
    if (!this.selectedSpendingAccount) {
      throw Error('Please select a spending account');
    }
    if (this.selectedSpendingAccount.Balance < this.order.Total) {
      throw Error('This spending account has insuficient funds');
    }
    if (!this.selectedSpendingAccount.AllowAsPaymentMethod) {
      throw Error('This spending account is not an allowed payment method.');
    }
    this.continue.emit();
  }

  updateRequestOptions(options: { search?: string; page?: number }) {
    Object.assign(this.requestOptions, options);
    this.listSpendingAccounts().subscribe((x) => (this.spendingAccounts = x));
  }
}
