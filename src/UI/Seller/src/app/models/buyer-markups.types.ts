import { MarketplaceBuyer } from '@ordercloud/headstart-sdk'

export interface HSBuyerPriceMarkup {
  Buyer: MarketplaceBuyer
  Markup: BuyerMarkup
}

interface BuyerMarkup {
  Percent: number
}
