import { OrderCloudIntegrationsCreditCardToken } from "@ordercloud/headstart-sdk";
import { Address, BuyerCreditCard } from "ordercloud-javascript-sdk";

export interface CreditCardFormOutput {
    card: OrderCloudIntegrationsCreditCardToken
    cvv: string
}

export interface SelectedCreditCard {
  SavedCard?: MarketplaceBuyerCreditCard
  NewCard?: OrderCloudIntegrationsCreditCardToken
  CVV: string
}

export interface CreditCard {
  token: string
  name: string
  month: string
  year: string
  street: string
  state: string
  city: string
  zip: string
  country: string
  cvv: string
}

export type MarketplaceBuyerCreditCard = BuyerCreditCard<CreditCardXP>

export interface CreditCardXP {
  CCBillingAddress: Address
}