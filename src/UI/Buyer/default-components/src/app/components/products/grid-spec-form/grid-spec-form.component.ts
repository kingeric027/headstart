import { Component, Input } from '@angular/core'
import { FormGroup } from '@angular/forms'
import {
  PriceSchedule,
  SuperMarketplaceProduct,
} from '@ordercloud/headstart-sdk'
import { MarketplaceMeProduct, ShopperContextService } from 'marketplace'
import { PriceBreak, Spec } from 'ordercloud-javascript-sdk'
import { ProductDetailService } from '../product-details/product-detail.service'
import { QtyChangeEvent } from '../quantity-input/quantity-input.component'
import { GridSpecOption, SpecFormService } from '../spec-form/spec-form.service'
import { minBy as _minBy } from 'lodash'

@Component({
  templateUrl: `./grid-spec-form.component.html`,
})
export class OCMGridSpecForm {
  _specs: Spec[]
  _specForm: FormGroup
  _superProduct: SuperMarketplaceProduct
  product: MarketplaceMeProduct
  specOptions: string[]
  lineItems: any[] = []
  lineTotals: number[] = []
  unitPrices: number[] = []
  totalPrice = 0
  isAddingToCart = false
  priceSchedule: PriceSchedule
  priceBreaks: PriceBreak[]
  price: number
  percentSavings: number
  totalQty: number
  qtyValid = false
  errorMsg = ''
  constructor(
    private specFormService: SpecFormService,
    private context: ShopperContextService,
    private productDetailService: ProductDetailService
  ) {}

  @Input() set superProduct(value: SuperMarketplaceProduct) {
    this._superProduct = value
    this.product = this._superProduct.Product
    this.priceBreaks = this._superProduct.PriceSchedule.PriceBreaks
    this.priceSchedule = this._superProduct.PriceSchedule
  }
  @Input() set specs(value: Spec[]) {
    this._specs = value
    this.getSpecOptions(value)
  }
  @Input() set specForm(value: FormGroup) {
    this._specForm = value
  }

  getSpecOptions(specs: Spec[]): void {
    // creates an object with each spec option and its values
    const obj = {}
    for (const spec of specs) {
      for (const option of spec.Options) {
        const name = spec.Name.replace(/ /g, '')
        if (obj[name]) obj[name].push(option.Value)
        else obj[name] = [option.Value]
      }
    }
    this.specOptions = this.getAllSpecCombinations(obj)
  }

  getAllSpecCombinations(specOptions: object): string[] {
    // returns an array containing every combination of spec values
    const arr = []
    for (const specName in specOptions) {
      if (specOptions.hasOwnProperty(specName)) {
        arr.push(specOptions[specName])
      }
    }
    if (arr.length === 1) return arr[0]
    else {
      const result = []
      const combinations = this.getAllSpecCombinations(arr.slice(1))
      for (const combination of combinations) {
        for (const optionValue of arr[0]) {
          result.push(optionValue + ', ' + combination)
        }
      }
      result.forEach(() => {
        this.lineTotals.push(0)
        this.unitPrices.push(0)
      })
      return result
    }
  }

  changeQuantity(specs: string, event: QtyChangeEvent): void {
    const indexOfSpec = this.specOptions.indexOf(specs)
    let specArray = specs.split(',')
    specArray = specArray.map((x) => x.replace(/\s/g, ''))
    const item = {
      Quantity: event.qty,
      Product: this.product,
      ProductID: this.product.ID,
      Specs: this.specFormService.getGridLineItemSpecs(this._specs, specArray),
      xp: {
        ImageUrl: this.specFormService.getGridLineItemImageUrl(
          this._superProduct.Images,
          this._specs,
          specArray
        ),
      },
    }
    const i = this.lineItems.findIndex(
      (li) => JSON.stringify(li.Specs) === JSON.stringify(item.Specs)
    )
    if (i === -1) this.lineItems.push(item)
    else this.lineItems[i] = item
    const liQuantities = []
    this.lineItems.forEach((li) => liQuantities.push(li.Quantity))
    this.totalQty = liQuantities.reduce((acc, curr) => {
      return acc + curr
    })
    this.qtyValid = this.validateQuantity(this.lineItems)
    this.lineTotals[indexOfSpec] = this.getLineTotal(event.qty, item.Specs[0])
    this.unitPrices[indexOfSpec] = this.getUnitPrice(event.qty, item.Specs[0])
    this.totalPrice = this.getTotalPrice()
  }

  getErrorMsg(event: any) {
    this.errorMsg = event
    console.log(event)
  }

  validateQuantity(lineItems: any): boolean {
    return this.totalQty >= lineItems[0].Product.PriceSchedule.MinQty &&
      lineItems[0].Product.PriceSchedule.MaxQuantity !== null
      ? this.totalQty <= lineItems[0].Product.PriceSchedule.MaxQuantity
      : this.totalQty !== 0
  }

  getUnitPrice(qty: number, specs: GridSpecOption): number {
    if (!this.priceBreaks?.length) return
    const startingBreak = _minBy(this.priceBreaks, 'Quantity')
    const selectedBreak = this.priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= qty
        ? candidate
        : current
    }, startingBreak)
    return specs.Markup
      ? selectedBreak.Price + specs.Markup
      : selectedBreak.Price
  }

  getLineTotal(qty: number, specs: GridSpecOption): number {
    if (qty > 0) {
      if (this.priceBreaks?.length) {
        const basePrice = qty * this.priceBreaks[0].Price
        this.percentSavings = this.productDetailService.getPercentSavings(
          this.price,
          basePrice
        )
      }
      return this.productDetailService.getGridLineItemPrice(
        this.priceBreaks,
        specs,
        qty
      )
    }
    return 0
  }

  async addToCart(): Promise<void> {
    const lineItems = this.lineItems.filter((li) => li.Quantity > 0)
    try {
      this.isAddingToCart = true
      await this.context.order.cart.addMany(lineItems)
    } catch (ex) {
      this.isAddingToCart = false
      throw ex
    }
    this.isAddingToCart = false
    this.lineItems = []
  }

  getTotalPrice(): number {
    return this.lineTotals.reduce((acc, curr) => {
      return acc + curr
    })
  }
}
