import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OrderStatus, OrderFilters, ShopperContextService, OrderContext, OrderFlowStatus } from 'marketplace';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './order-status-filter.component.html',
  styleUrls: ['./order-status-filter.component.scss'],
})
export class OCMOrderStatusFilter implements OnInit, OnDestroy {
  alive = true;
  form: FormGroup;
  statuses = [OrderFlowStatus.Submitted, OrderFlowStatus.Shipped];

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    const needsApproval = this.context.currentUser.hasRoles('MPNeedsApproval');
    if (needsApproval) {
      this.statuses = [...this.statuses, OrderFlowStatus.AwaitingApproval, OrderFlowStatus.AwaitingChanges];
    }
    const orderContext = this.context.router.getOrderContext();
    // const defaultFilter = orderContext === OrderContext.
    this.form = new FormGroup({
      status: new FormControl(OrderStatus.AllSubmitted),
    });
    this.context.orderHistory.filters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe((filters: OrderFilters) => {
        this.form.setValue({ status: filters.status || '' });
      });
  }

  selectStatus(): void {
    const status = this.form.get('status').value;
    this.context.orderHistory.filters.filterByStatus(status);
  }

  ngOnDestroy(): void {
    this.alive = false;
  }
}
