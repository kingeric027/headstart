import { Component, OnInit, OnDestroy } from '@angular/core'
import { FormGroup, FormControl } from '@angular/forms'
import { takeWhile } from 'rxjs/operators'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'
import { OrderFilters, OrderStatus } from 'src/app/shopper-context'

@Component({
  templateUrl: './order-status-filter.component.html',
  styleUrls: ['./order-status-filter.component.scss'],
})
export class OCMOrderStatusFilter implements OnInit, OnDestroy {
  alive = true
  form: FormGroup
  statuses = []

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.statuses = Object.values(OrderStatus)
    this.form = new FormGroup({
      status: new FormControl(OrderStatus.AllSubmitted),
    })
    this.context.orderHistory.filters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe((filters: OrderFilters) => {
        this.form.setValue({ status: filters.status || '' })
      })
  }

  selectStatus(): void {
    const status = this.form.get('status').value
    this.context.orderHistory.filters.filterByStatus(status)
  }

  ngOnDestroy(): void {
    this.alive = false
  }
}