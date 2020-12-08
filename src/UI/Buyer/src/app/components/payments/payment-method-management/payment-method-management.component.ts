import { Component, OnInit } from '@angular/core'
import {
  BuyerCreditCard,
  SpendingAccount,
  ListPage,
} from 'ordercloud-javascript-sdk'
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons'
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons'
import { ModalState } from 'src/app/models/modal-state.class'
import { CreditCardFormOutput } from '../credit-card-form/credit-card-form.component'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'

@Component({
  templateUrl: './payment-method-management.component.html',
  styleUrls: ['./payment-method-management.component.scss'],
})
export class OCMPaymentMethodManagement implements OnInit {
  alive = true
  showCardForm = false
  faPlus = faPlus
  faArrowLeft = faArrowLeft
  faTrashAlt = faTrashAlt
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  }
  areYouSureModal = ModalState.Closed
  cards: ListPage<BuyerCreditCard>
  accounts: ListPage<SpendingAccount>
  currentCard: BuyerCreditCard = null

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.listCards()
  }

  showAdd(): void {
    this.showCardForm = true
    this.currentCard = null
  }

  showAreYouSure(card: BuyerCreditCard): void {
    this.currentCard = card
    this.areYouSureModal = ModalState.Open
  }

  closeAreYouSure(): void {
    this.currentCard = null
    this.areYouSureModal = ModalState.Closed
  }

  dismissAddCardForm(): void {
    this.currentCard = null
    this.showCardForm = false
  }

  async addCard(output: CreditCardFormOutput): Promise<void> {
    await this.context.currentUser.cards.Save(output.card)
    this.showCardForm = false
    this.listCards()
  }

  async deleteCard(card: BuyerCreditCard): Promise<void> {
    this.areYouSureModal = ModalState.Closed
    this.cards.Items = this.cards.Items.filter((c) => c.ID !== card.ID)
    await this.context.currentUser.cards.Delete(card.ID)
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }): void {
    this.requestOptions = Object.assign(this.requestOptions, newOptions)
    this.listCards()
  }

  private async listCards(): Promise<void> {
    this.cards = await this.context.currentUser.cards.List()
  }
}