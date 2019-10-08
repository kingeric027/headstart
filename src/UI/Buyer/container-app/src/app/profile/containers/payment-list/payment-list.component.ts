import { Component, OnInit } from '@angular/core';
import { ListBuyerCreditCard, BuyerCreditCard, ListSpendingAccount } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';

import * as moment from 'moment';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { AuthNetCreditCard } from 'shopper-context-interface';
import { AuthNetCreditCardService } from 'src/app/shared';

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

  constructor(private context: ShopperContextService, private creditCardService: AuthNetCreditCardService) {}

  ngOnInit() {
    this.getCards();
    this.getAccounts();
  }

  getCards() {
    this.cards$ = this.context.myResources.ListCreditCards();
  }

  getAccounts() {
    const now = moment().format('YYYY-MM-DD');
    const dateFilter = { StartDate: `>${now}|!*`, EndDate: `<${now}|!*` };
    this.accounts$ = this.context.myResources.ListSpendingAccounts({
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

  async addCard(card: AuthNetCreditCard) {
    const response = await this.creditCardService.CreateSavedCard(card);
    if (response.ResponseHttpStatusCode >= 400) {
      throw new Error((response.ResponseBody as any).ExceptionMessage);
    } else {
      this.showCardForm = false;
      this.getCards();
    }
  }

  async deleteCard(cardId: string) {
    await this.creditCardService.DeleteSavedCard(cardId);
    this.getCards();
  }
}
