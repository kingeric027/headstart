import {
  Component,
  Input,
  ViewEncapsulation,
  ChangeDetectorRef,
} from '@angular/core'
import { getPrimaryImageUrl } from 'src/app/services/images.helpers'
import { PriceSchedule } from 'ordercloud-javascript-sdk'
import { MarketplaceMeProduct } from 'src/app/shopper-context'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'
@Component({
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OCMProductCard {
  _isFavorite = false
  _product: MarketplaceMeProduct = {
    PriceSchedule: {} as PriceSchedule,
  }
  _price: number
  _userCurrency: string
  quantity: number
  qtyValid = true
  shouldDisplayAddToCart = false
  isViewOnlyProduct = true
  hasSpecs = false
  isAddingToCart = false
  productImageHeight: any

  constructor(
    private cdr: ChangeDetectorRef,
    private context: ShopperContextService
  ) {}

  @Input() set product(value: MarketplaceMeProduct) {
    this._product = value
    this._price = value.PriceSchedule?.PriceBreaks[0]?.Price
    this.isViewOnlyProduct = !value.PriceSchedule
    this.hasSpecs = value.SpecCount > 0
    this._userCurrency = this.context.currentUser.get().Currency
  }

  @Input() set isFavorite(value: boolean) {
    this._isFavorite = value
    this.cdr.detectChanges() // TODO - remove. Solve another way.
  }

  async addToCart(): Promise<void> {
    this.isAddingToCart = true
    try {
      await this.context.order.cart.add({
        ProductID: this._product.ID,
        Quantity: this.quantity,
        xp: {
          ImageUrl: getPrimaryImageUrl(
            this._product,
            this.context.currentUser.get()
          ),
        },
      })
      this.isAddingToCart = false
    } catch (ex) {
      this.isAddingToCart = false
      throw ex
    }
  }

  ngOnInit(): void {
    setTimeout(() => {
      this.applyAspectRatioChange()
    }, 0)
  }

  applyAspectRatioChange(): void {
    const productImages = document.getElementsByClassName('product-card-image')
    Array.from(productImages).forEach((productImage: any) => {
      const aspectRatio = productImage.naturalHeight / productImage.naturalWidth
      if (aspectRatio === 1) {
        productImage.classList.add('object-fit-cover')
      }
    })
  }

  getImageUrl(): string {
    return getPrimaryImageUrl(this._product, this.context.currentUser.get())
  }

  toDetails(): void {
    this.context.router.toProductDetails(this._product.ID)
  }

  setIsFavorite(isFavorite: boolean): void {
    this.context.currentUser.setIsFavoriteProduct(isFavorite, this._product.ID)
  }

  showAddToCart(): boolean {
    return (
      !this.isViewOnlyProduct &&
      !this.hasSpecs &&
      this._product.xp.ProductType !== 'Quote'
    )
  }

  setQuantity(event: any): void {
    this.quantity = event.qty
    this.qtyValid = event.valid
  }
}