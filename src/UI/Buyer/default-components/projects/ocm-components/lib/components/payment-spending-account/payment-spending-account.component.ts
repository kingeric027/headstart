import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SpendingAccount, ListSpendingAccount, Payment, Order } from '@ordercloud/angular-sdk';
import * as moment_ from 'moment';
const moment = moment_;
import { ModalState } from '../../models/modal-state.class';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './payment-spending-account.component.html',
  styleUrls: ['./payment-spending-account.component.scss'],
})
export class OCMPaymentSpendingAccount extends OCMComponent {
  order: Order;
  @Input() payment: Payment;
  @Output() continue = new EventEmitter();
  spendingAccounts: ListSpendingAccount;
  selectedSpendingAccount: SpendingAccount = null;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  resultsPerPage = 6;
  spendingAccountModal = ModalState.Closed;

  async ngOnContextSet() {
    this.order = this.context.currentOrder.get();
    this.spendingAccounts = await this.listSpendingAccounts();
    this.selectedSpendingAccount = this.getSavedSpendingAccount(this.spendingAccounts);
    if (!this.selectedSpendingAccount) {
      this.spendingAccountModal = ModalState.Open;
    }
  }

  async listSpendingAccounts(): Promise<ListSpendingAccount> {
    const now = moment().format('YYYY-MM-DD');
    const filters = { StartDate: `<${now}|!*`, EndDate: `>${now}|!*` };
    const options = { filters, ...this.requestOptions, pageSize: this.resultsPerPage };
    return await this.context.myResources.ListSpendingAccounts(options).toPromise();
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
    this.spendingAccountModal = ModalState.Closed;
    this.selectedSpendingAccount = account;
    this.payment = {
      Type: 'SpendingAccount',
      SpendingAccountID: account.ID,
      Accepted: true,
    };
    this.context.currentOrder.createPayment(this.payment);
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

  async updateRequestOptions(options: { search?: string; page?: number }) {
    Object.assign(this.requestOptions, options);
    this.spendingAccounts = await this.listSpendingAccounts();
  }

  openModal() {
    this.spendingAccountModal = ModalState.Open;
  }
}
