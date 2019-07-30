import { Injectable } from '@angular/core';
import { OrderReorderResponse } from '@app-buyer/shared/services/reorder/reorder.interface';
import { OcMeService, BuyerProduct, LineItem } from '@ordercloud/angular-sdk';
import { forEach as _forEach, differenceBy as _differenceBy } from 'lodash';
import { CartService } from '@app-buyer/shared/services/cart/cart.service';

@Injectable({
  providedIn: 'root',
})
export class AppReorderService {
  constructor(
    private cartService: CartService,
    private meService: OcMeService
  ) {}

  public async order(orderID: string): Promise<OrderReorderResponse> {
    if (!orderID) throw new Error('Needs Order ID');
    const lineItems = await this.cartService.listAllItems(orderID);
    const productIds = lineItems.Items.map((item) => item.ProductID);
    const validProducts = await this.getValidProducts(productIds);
    const result = this.isProductInLiValid(validProducts, lineItems.Items);
    return this.hasInventory(result);
  }

  private async getValidProducts(
    productIds: string[],
    validProducts: BuyerProduct[] = []
  ): Promise<BuyerProduct[]> {
    validProducts = validProducts;
    const chunk = productIds.splice(0, 25);
    const productList = await this.meService
      .ListProducts({ filters: { ID: chunk.join('|') } })
      .toPromise();
    validProducts = validProducts.concat(productList.Items);
    if (productIds.length) {
      return this.getValidProducts(productIds, validProducts);
    } else {
      return Promise.resolve(validProducts);
    }
  }

  private isProductInLiValid(
    products: BuyerProduct[],
    lineItems: LineItem[]
  ): OrderReorderResponse {
    const validProductIDs = products.map((p) => p.ID);
    const validLi: LineItem[] = [];
    const invalidLi: LineItem[] = [];

    _forEach(lineItems, (li) => {
      if (validProductIDs.indexOf(li.ProductID) > -1) {
        const product = products.find((p) => p.ID === li.ProductID);
        li.Product = product;
        validLi.push(li);
      } else {
        invalidLi.push(li);
      }
    });
    return { ValidLi: validLi, InvalidLi: invalidLi };
  }

  private hasInventory(response: OrderReorderResponse): OrderReorderResponse {
    // compare new validLi with old validLi and push difference into the new invalid[] + old invalid array.
    const newValidLi = response.ValidLi.filter(isValidToOrder);
    let newInvalidLi = _differenceBy(response.ValidLi, newValidLi, 'ProductID');

    newInvalidLi = newInvalidLi.concat(response.InvalidLi);

    return { ValidLi: newValidLi, InvalidLi: newInvalidLi };

    function isValidToOrder(li) {
      const restrictedOrderQuantity =
        li.Product.PriceSchedule.RestrictedQuantity;
      let withinPriceBreak;

      if (!restrictedOrderQuantity) {
        return validOrderQuantity(li);
      } else {
        withinPriceBreak = li.Product.PriceSchedule.PriceBreaks.some(
          (pb) => pb.Quantity === li.Quantity
        );
        return withinPriceBreak && validOrderQuantity(li);
      }
    }
    function validOrderQuantity(li) {
      const inventory = li.Product.Inventory;
      if (!inventory || !inventory.Enabled || inventory.OrderCanExceed) {
        return true;
      }
      return inventory.QuantityAvailable >= li.Quantity;
    }
  }
}
