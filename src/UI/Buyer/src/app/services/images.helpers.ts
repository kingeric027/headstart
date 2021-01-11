import { ocAppConfig } from '../config/app.config'
import { MarketplaceLineItem, MarketplaceMeProduct } from '@ordercloud/headstart-sdk'
import { CurrentUser } from '../models/profile.types'

export const getPrimaryImageUrl = (
  product: MarketplaceMeProduct,
  user: CurrentUser
): string => {
  return `${ocAppConfig.cmsUrl}/assets/${user.Seller.ID}/products/${product.ID}/thumbnail?size=M`
}

export const getPrimaryLineItemImage = (
  lineItemID: string,
  lineItems: MarketplaceLineItem[],
  user: CurrentUser
): string => {
  const li = lineItems.find((item) => item.ID === lineItemID)
  return getPrimaryImageUrl(li.Product, user)
}
