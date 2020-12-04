import { FormGroup } from '@angular/forms'
import { MeProductInKit } from '@ordercloud/headstart-sdk'

export interface ProductSelectionEvent {
  productKitDetails: MeProductInKit
  variantSelection?: KitVariantSelection
}

export interface KitVariantSelection {
  productID: string
  specForm: FormGroup
  quantity: number
}
