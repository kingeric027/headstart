import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Order, ListLineItem, ListPromotion, OcOrderService, ListPayment, OrderApproval } from '@ordercloud/angular-sdk';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { AppPaymentService } from 'src/app/shared/services/app-payment/app-payment.service';
import { uniqBy as _uniqBy } from 'lodash';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'order-details',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OrderDetailsComponent implements OnInit {
  order$: Observable<Order>;
  lineItems$: Observable<ListLineItem>;
  promotions: ListPromotion;
  payments: ListPayment;
  approvals: OrderApproval[] = [];

  constructor(
    protected activatedRoute: ActivatedRoute,
    protected ocOrderService: OcOrderService,
    protected appPaymentService: AppPaymentService,
    public context: ShopperContextService //used in template
  ) {}

  ngOnInit() {
    this.order$ = this.activatedRoute.data.pipe(map(({ orderResolve }) => orderResolve.order));
    this.lineItems$ = this.activatedRoute.data.pipe(map(({ orderResolve }) => orderResolve.lineItems));
    this.activatedRoute.paramMap.subscribe(async (params: ParamMap) => {
      const orderID = params.get('orderID');
      this.promotions = await this.getPromotions(orderID);
      this.payments = await this.getPayments(orderID);
      this.approvals = await this.getApprovals(orderID);
    });
  }

  protected async getPromotions(orderID: string): Promise<ListPromotion> {
    return this.ocOrderService.ListPromotions('outgoing', orderID).toPromise();
  }

  protected async getPayments(orderID: string): Promise<ListPayment> {
    return this.appPaymentService.ListPaymentsOnOrder(orderID);
  }

  protected async getApprovals(orderID: string): Promise<OrderApproval[]> {
    const approvals = await this.ocOrderService.ListApprovals('outgoing', orderID).toPromise();
    approvals.Items = approvals.Items.filter((x) => x.Approver);
    return _uniqBy(approvals.Items, (x) => x.Comments);
  }
}
