import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ModalState } from 'src/app/shared/models/modal-state.class';

@Component({
  selector: 'ocm-order-approval',
  templateUrl: './order-approval.component.html',
  styleUrls: ['./order-approval.component.scss'],
})
export class OrderApprovalComponent implements OnInit {
  approved: boolean;
  form: FormGroup;
  modalID = 'order-approval-comments';
  @Input() orderID: string;
  approveModal = ModalState.Closed;

  constructor(private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit() {
    this.form = new FormGroup({ comments: new FormControl('') });
  }

  openModal(approved: boolean) {
    this.approved = approved;
    this.approveModal = ModalState.Open;
  }

  async submitReview() {
    const comments = this.form.value.comments;
    if (this.approved) {
      await this.context.orderHistory.approveOrder(this.orderID, comments);
    } else {
      await this.context.orderHistory.declineOrder(this.orderID, comments);
    }

    this.toasterService.success(`Order ${this.orderID} was ${this.approved ? 'Approved' : 'Declined'}`);
    this.approveModal = ModalState.Closed;
    this.context.router.toOrdersToApprove();
  }
}
