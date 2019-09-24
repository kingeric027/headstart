import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { OcOrderService } from '@ordercloud/angular-sdk';
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

  constructor(
    private formBuilder: FormBuilder,
    private ocOrderService: OcOrderService,
    private toasterService: ToastrService,
    private context: ShopperContextService
  ) {}

  ngOnInit() {
    this.form = this.formBuilder.group({ comments: '' });
  }

  openModal(approved: boolean) {
    this.approved = approved;
    this.approveModalOpen = true;
  }

  submitReview() {
    const comments = this.form.value.comments;
    const request = this.approved
      ? this.ocOrderService.Approve('outgoing', this.orderID, {
          Comments: comments,
          AllowResubmit: false,
        })
      : this.ocOrderService.Decline('outgoing', this.orderID, {
          Comments: comments,
          AllowResubmit: false,
        });

    request.subscribe(() => {
      this.toasterService.success(`Order ${this.orderID} was ${this.approved ? 'Approved' : 'Declined'}`);
      this.approveModalOpen = false;
      this.context.routeActions.toOrdersToApprove();
    });
  }
}
