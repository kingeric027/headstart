import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OcOrderService, Order } from '@ordercloud/angular-sdk';
import { ToastrService } from 'ngx-toastr';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'order-approval',
  templateUrl: './order-approval.component.html',
  styleUrls: ['./order-approval.component.scss'],
})
export class OrderApprovalComponent implements OnInit {
  approved: boolean;
  form: FormGroup;
  modalID = 'order-approval-comments';
  @Input() orderID: string;
  approveModalOpen = false;

  constructor(private ocOrderService: OcOrderService, private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit() {
    this.form = new FormGroup({ comments: new FormControl('') });
  }

  openModal(approved: boolean) {
    this.approved = approved;
    this.approveModalOpen = true;
  }

  async approveOrder(orderID: string, comments: string): Promise<Order> {
    return await this.ocOrderService
      .Approve('outgoing', orderID, {
        Comments: comments,
        AllowResubmit: false,
      })
      .toPromise();
  }

  async declineOrder(orderID: string, comments: string): Promise<Order> {
    return await this.ocOrderService
      .Decline('outgoing', orderID, {
        Comments: comments,
        AllowResubmit: false,
      })
      .toPromise();
  }

  async submitReview() {
    const comments = this.form.value.comments;
    if (this.approved) {
      await this.approveOrder(this.orderID, comments);
    } else {
      await this.declineOrder(this.orderID, comments);
    }

    this.toasterService.success(`Order ${this.orderID} was ${this.approved ? 'Approved' : 'Declined'}`);
    this.approveModalOpen = false;
    this.context.routeActions.toOrdersToApprove();
  }
}
