import { LineItemSpec } from 'ordercloud-javascript-sdk'

export interface LineItemToAdd {
  ProductID: string
  Quantity: number
  Specs: LineItemSpec[]
  Product: {
    // adding purely so i can use productNameWithSpecs pipe without modification
    Name: string
  }
  Price: number // adding for display purposes
  xp: {
    ImageUrl: string
    KitProductName: string
    KitProductImageUrl: string
    KitProductID: string
  }
}
