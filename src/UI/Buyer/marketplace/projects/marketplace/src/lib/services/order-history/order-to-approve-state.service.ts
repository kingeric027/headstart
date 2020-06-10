import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { OcMeService } from '@ordercloud/angular-sdk';

@Injectable({
  providedIn: 'root',
})
export class OrdersToApproveStateService {
  public showAlert = new BehaviorSubject<number>(0);
  public numberOfOrdersToApprove = new BehaviorSubject<number>(0);

  constructor(private ocMeService: OcMeService) {}

  async reset(): Promise<void> {
    const ordersToApproverResponse = await this.ocMeService.ListApprovableOrders().toPromise();
    this.numberOfOrdersToApprove.next(ordersToApproverResponse.Meta.TotalCount);
  }

  async alertIfOrdersToApprove(): Promise<void> {
    await this.reset();
    if (this.numberOfOrdersToApprove.value) {
      this.showAlert.next(this.numberOfOrdersToApprove.value);
    }
  }
}
