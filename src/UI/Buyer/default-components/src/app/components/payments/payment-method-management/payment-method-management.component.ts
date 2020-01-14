import { Component, OnInit } from '@angular/core';
import { ListBuyerCreditCard, BuyerCreditCard, ListSpendingAccount } from '@ordercloud/angular-sdk';
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import * as moment_ from 'moment';
const moment = moment_;
import { AuthNetCreditCard, ShopperContextService } from 'marketplace';
import { ModalState } from 'src/app/models/modal-state.class';

@Component({
  templateUrl: './payment-method-management.component.html',
  styleUrls: ['./payment-method-management.component.scss'],
})
export class OCMPaymentMethodManagement implements OnInit {
  alive = true;
  showCardForm = false;
  faPlus = faPlus;
  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  areYouSureModal = ModalState.Closed;
  cards: ListBuyerCreditCard;
  accounts: ListSpendingAccount;
  currentCard: BuyerCreditCard = null;

  constructor(private context: ShopperContextService) {}

  ngOnInit() {
    this.getCards();
    this.getAccounts();
  }

  async getCards() {
    this.cards = await this.context.myResources.ListCreditCards().toPromise();
  }

  async getAccounts() {
    const now = moment().format('YYYY-MM-DD');
    //TODO: Reconsider filter - it's not working.
    const dateFilter = { StartDate: `>${now}|!*`, EndDate: `<${now}|!*` };
    this.accounts = await this.context.myResources.ListSpendingAccounts({ filters: undefined }).toPromise();
  }

  showEdit(card: BuyerCreditCard) {
    this.showCardForm = true;
    this.currentCard = card;
  }

  showAdd() {
    this.showCardForm = true;
    this.currentCard = null;
  }

  showAreYouSure(card: BuyerCreditCard) {
    this.currentCard = card;
    this.areYouSureModal = ModalState.Open;
  }

  closeAreYouSure() {
    this.currentCard = null;
    this.areYouSureModal = ModalState.Closed;
  }

  dismissEditCardForm() {
    this.currentCard = null;
    this.showCardForm = false;
  }

  async addCard(card: AuthNetCreditCard) {
    const response = await this.context.creditCards.CreateSavedCard(card);
    if (!response.ResponseBody.ID && (response.ResponseBody as any).messages.resultCode === 'Error') {
      throw new Error((response.ResponseBody as any).messages.message[0].text);
    } else {
      this.showCardForm = false;
      this.getCards();
    }
  }

  async deleteCard(card: BuyerCreditCard) {
    this.areYouSureModal = ModalState.Closed;
    this.cards.Items = this.cards.Items.filter(c => c.ID !== card.ID);
    await this.context.creditCards.DeleteSavedCard(card.ID);
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }) {
    console.log(newOptions);
    this.requestOptions = Object.assign(this.requestOptions, newOptions);
    this.reloadCards();
  }

  private async reloadCards() {
    const cards = await this.context.myResources.ListCreditCards(this.requestOptions).toPromise();
    this.cards = cards;
  }
}
