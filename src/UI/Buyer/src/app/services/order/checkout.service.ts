import { MarketplaceBuyerCreditCard } from '../../shopper-context'
import {
  Payment,
  Orders,
  Payments,
  BuyerAddress,
  Me,
  OrderWorksheet,
  IntegrationEvents,
  ShipMethodSelection,
  LineItems,
} from 'ordercloud-javascript-sdk'
import { Injectable } from '@angular/core'
import { PaymentHelperService } from '../payment-helper/payment-helper.service'
import { OrderStateService } from './order-state.service'
import {
  HeadStartSDK,
  Address,
  OrderCloudIntegrationsCreditCardToken,
  MarketplaceOrder,
  ListPage,
  MarketplaceLineItem,
  MarketplaceAddressBuyer,
  OrderCloudIntegrationsCreditCardPayment,
} from '@ordercloud/headstart-sdk'
import { max } from 'lodash'

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  constructor(
    private paymentHelper: PaymentHelperService,
    private state: OrderStateService
  ) {}

  async appendPaymentMethodToOrderXp(
    orderID: string,
    ccPayment?: OrderCloudIntegrationsCreditCardPayment
  ): Promise<void> {
    const paymentMethod = ccPayment?.CreditCardID
      ? 'Credit Card'
      : 'Purchase Order'
    await Orders.Patch('Outgoing', orderID, {
      xp: { PaymentMethod: paymentMethod },
    })
  }

  async addComment(comment: string): Promise<MarketplaceOrder> {
    return await this.patch({ Comments: comment })
  }

  async setShippingAddress(address: BuyerAddress): Promise<MarketplaceOrder> {
    // If a saved address (with an ID) is changed by the user it is attached to an order as a one time address.
    // However, order.ShippingAddressID (or BillingAddressID) still points to the unmodified address. The ID should be cleared.
    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
    ;(address as any).ID = null
    this.order = await HeadStartSDK.ValidatedAddresses.SetShippingAddress(
      'Outgoing',
      this.order.ID,
      address as Address
    )
    return this.order
  }

  async setShippingAddressByID(
    address: MarketplaceAddressBuyer
  ): Promise<MarketplaceOrder> {
    try {
      await Orders.Patch('Outgoing', this.order.ID, {
        xp: { ShippingAddress: address },
      })
      return await this.patch({ ShippingAddressID: address.ID })
    } catch (ex) {
      if (ex?.errors?.Errors?.[0]?.ErrorCode === 'NotFound') {
        if (ex?.errors?.Errors?.[0]?.Data.ObjectType === 'Order') {
          throw Error('You no longer have access to this order')
        }
        throw Error(
          'You no longer have access to this saved address. Please enter or select a different one.'
        )
      }
      throw ex
    }
  }

  async setBuyerLocationByID(
    buyerLocationID: string
  ): Promise<MarketplaceOrder> {
    const patch = {
      BillingAddressID: buyerLocationID,
      xp: { ApprovalNeeded: '' },
    }
    const isApprovalNeeded = await this.isApprovalNeeded(buyerLocationID)
    if (isApprovalNeeded) patch.xp.ApprovalNeeded = buyerLocationID
    try {
      this.order = await this.patch(patch as MarketplaceOrder)
      return this.order
    } catch (ex) {
      if (ex.errors.Errors[0].ErrorCode === 'NotFound') {
        if (ex?.errors?.Errors?.[0]?.Data.ObjectType === 'Order') {
          throw Error('You no longer have access to this order')
        }
        throw Error(
          'You no longer have access to this buyer location. Please enter or select a different one.'
        )
      }
    }
  }

  async isApprovalNeeded(locationID: string): Promise<boolean> {
    const userGroups = await Me.ListUserGroups({
      searchOn: 'ID',
      search: locationID,
    })
    return userGroups.Items.some((u) => u.ID === `${locationID}-NeedsApproval`)
  }

  async listPayments(): Promise<ListPage<Payment>> {
    return await this.paymentHelper.ListPaymentsOnOrder(this.order.ID)
  }

  async createSavedCCPayment(
    card: MarketplaceBuyerCreditCard,
    amount: number
  ): Promise<Payment> {
    return await this.createCCPayment(
      card?.PartialAccountNumber,
      card.CardType,
      card.ID,
      amount
    )
  }

  async createOneTimeCCPayment(
    card: OrderCloudIntegrationsCreditCardToken,
    amount: number
  ): Promise<Payment> {
    // This slice() is sooo crucial. Otherwise we would be storing creditcard numbers in xp.
    // Which would be really really bad.
    const partialAccountNum = card.AccountNumber.slice(-4)
    return await this.createCCPayment(
      partialAccountNum,
      card.CardType,
      null,
      amount
    )
  }

  // Integration Methods
  // order cloud sandbox service methods, to be replaced by updated sdk in the future
  async estimateShipping(): Promise<OrderWorksheet> {
    return await IntegrationEvents.EstimateShipping('Outgoing', this.order.ID)
  }

  async cleanLineItemIDs(
    orderID: string,
    lineItems: MarketplaceLineItem[]
  ): Promise<void> {
    /* line item ids are significant for suppliers creating a relationship
     * between their shipments and line items in ordercloud
     * we are sequentially labeling these ids for ease of shipping */
    const patchQueue: Promise<any>[] = [] //This is an array of operations we will add to.
    const unIndexedLi = lineItems.filter((li) => li.ID.length != 4)
    let indexedLi = lineItems.filter((li) => li.ID.length === 4)
    lineItems.forEach((li, index) => {
      if (
        !lineItems.map((li) => li.ID).includes(this.createIDFromIndex(index))
      ) {
        //first check if there are any that have not been idexed at all
        if (unIndexedLi.length) {
          const liToPatch = unIndexedLi[0]
          patchQueue.push(
            LineItems.Patch('Outgoing', orderID, liToPatch.ID, {
              ID: this.createIDFromIndex(index),
            })
          )
          unIndexedLi.shift() //now remove that li from the unIndexed array
        } else {
          //otherwise grab the highest indexed line item and patch that to fill the hole.
          const maxIndex = max(
            indexedLi.map((li) => Number(li.ID.substring(1)))
          )
          const indextoMatch = this.createIDFromIndex(maxIndex - 1)
          const liToPatch = indexedLi.find((li) => {
            return li.ID === indextoMatch
          })
          patchQueue.push(
            LineItems.Patch('Outgoing', orderID, liToPatch.ID, {
              ID: this.createIDFromIndex(index),
            })
          )
          indexedLi = indexedLi.filter((li) => li.ID !== indextoMatch) // now remove that from indexedLi array so we dont patch it again
        }
      }
    })
    await Promise.all(patchQueue)
    await this.state.resetLineItems()
  }

  createIDFromIndex(index: number): string {
    /* X was choosen as a prefix for the lineItem ID so that it is easy to
     * direct suppliers where to look for the ID. L and I are sometimes indistinguishable
     * from the number 1 so I avoided those. X is also difficult to confuse with other
     * letters when verbally pronounced */
    const countInList = index + 1
    const paddedCount = countInList.toString().padStart(3, '0')
    return 'X' + paddedCount
  }

  async selectShipMethods(
    selections: ShipMethodSelection[]
  ): Promise<OrderWorksheet> {
    const orderWorksheet = await IntegrationEvents.SelectShipmethods(
      'Outgoing',
      this.order.ID,
      {
        ShipMethodSelections: selections,
      }
    )
    this.order = orderWorksheet.Order
    return orderWorksheet
  }

  async calculateOrder(): Promise<MarketplaceOrder> {
    const orderCalculation = await IntegrationEvents.Calculate(
      'Outgoing',
      this.order.ID
    )
    this.order = orderCalculation.Order
    return this.order
  }

  async createPurchaseOrderPayment(amount: number): Promise<Payment> {
    const payment: Payment = {
      Amount: amount,
      DateCreated: new Date().toDateString(),
      Type: 'PurchaseOrder',
    }
    return await Payments.Create('Outgoing', this.order.ID, payment)
  }

  async deleteExistingPayments(): Promise<any[]> {
    const payments = await Payments.List('Outgoing', this.order.ID)
    const deleteAll = payments.Items.map((payment) =>
      Payments.Delete('Outgoing', this.order.ID, payment.ID)
    )
    return Promise.all(deleteAll)
  }

  private async createCCPayment(
    partialAccountNum: string,
    cardType: string,
    creditCardID: string,
    amount: number
  ): Promise<Payment> {
    const payment: Payment = {
      Amount: amount,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: creditCardID,
      xp: {
        partialAccountNumber: partialAccountNum,
        cardType,
      },
    }
    return await Payments.Create('Outgoing', this.order.ID, payment)
  }

  private async patch(order: MarketplaceOrder): Promise<MarketplaceOrder> {
    this.order = (await Orders.Patch(
      'Outgoing',
      this.order.ID,
      order
    )) as MarketplaceOrder
    return this.order
  }

  private get order(): MarketplaceOrder {
    return this.state.order
  }

  private set order(value: MarketplaceOrder) {
    this.state.order = value
  }
}
