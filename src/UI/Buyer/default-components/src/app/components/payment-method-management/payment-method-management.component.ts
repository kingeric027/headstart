import { Component } from '@angular/core';
import { ListBuyerCreditCard, BuyerCreditCard, ListSpendingAccount } from '@ordercloud/angular-sdk';
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import * as moment_ from 'moment';
const moment = moment_;
import { OCMComponent } from '../base-component';
import { AuthNetCreditCard } from 'marketplace';

@Component({
  templateUrl: './payment-method-management.component.html',
  styleUrls: ['./payment-method-management.component.scss'],
})
export class OCMPaymentMethodManagement extends OCMComponent {
  alive = true;
  showCardForm = false;
  faPlus = faPlus;
  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;

  cards: ListBuyerCreditCard;
  accounts: ListSpendingAccount;
  currentCard: BuyerCreditCard = null;

  ngOnContextSet() {
    this.getCards();
    this.getAccounts();
  }

  async getCards() {
    this.cards = await this.context.myResources.ListCreditCards().toPromise();
  }

  async getAccounts() {
    const now = moment().format('YYYY-MM-DD');
    const dateFilter = { StartDate: `>${now}|!*`, EndDate: `<${now}|!*` };
    this.accounts = await this.context.myResources.ListSpendingAccounts({ filters: dateFilter }).toPromise();
  }

  showEdit(card: BuyerCreditCard) {
    this.showCardForm = true;
    this.currentCard = card;
  }

  showAdd() {
    this.showCardForm = true;
    this.currentCard = null;
  }

  async addCard(card: AuthNetCreditCard) {
    const response = await this.context.creditCards.CreateSavedCard(card);
    if (response.ResponseHttpStatusCode >= 400) {
      throw new Error((response.ResponseBody as any).ExceptionMessage);
    } else {
      this.showCardForm = false;
      this.getCards();
    }
  }

  async deleteCard(cardId: string) {
    await this.context.creditCards.DeleteSavedCard(cardId);
    this.getCards();
  }
}
