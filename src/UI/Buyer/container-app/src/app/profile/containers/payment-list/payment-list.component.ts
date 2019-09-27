import { Component, OnInit } from '@angular/core';
import { OcMeService, ListBuyerCreditCard, BuyerCreditCard, ListSpendingAccount } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { AuthorizeNetService } from 'src/app/shared';
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';

import * as moment from 'moment';
import { CreateCard } from 'shopper-context-interface';

@Component({
  selector: 'profile-payment-list',
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.scss'],
})
export class PaymentListComponent implements OnInit {
  alive = true;
  showCardForm = false;
  faPlus = faPlus;
  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;

  cards$: Observable<ListBuyerCreditCard>;
  accounts$: Observable<ListSpendingAccount>;
  currentCard: BuyerCreditCard = null;

  constructor(private ocMeService: OcMeService, private authorizeNetSerivce: AuthorizeNetService) {}

  ngOnInit() {
    this.getCards();
    this.getAccounts();
  }

  getCards() {
    this.cards$ = this.ocMeService.ListCreditCards();
  }

  getAccounts() {
    const now = moment().format('YYYY-MM-DD');
    const dateFilter = { StartDate: `>${now}|!*`, EndDate: `<${now}|!*` };
    this.accounts$ = this.ocMeService.ListSpendingAccounts({
      filters: dateFilter,
    });
  }

  showEdit(card: BuyerCreditCard) {
    this.showCardForm = true;
    this.currentCard = card;
  }

  showAdd() {
    this.showCardForm = true;
    this.currentCard = null;
  }

  async addCard(card: CreateCard) {
    const response = await this.authorizeNetSerivce.CreateCreditCard(card);
    if (response.ResponseHttpStatusCode >= 400) {
      throw new Error((response.ResponseBody as any).ExceptionMessage);
    } else {
      this.showCardForm = false;
      this.getCards();
    }
  }

  async deleteCard(cardId: string) {
    await this.authorizeNetSerivce.DeleteCreditCard(cardId);
    this.getCards();
  }
}
