import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ModalState } from 'src/app/models/modal-state.class';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './order-approval.component.html',
  styleUrls: ['./order-approval.component.scss'],
})
export class OCMOrderApproval  implements OnInit {
  approved: boolean;
  form: FormGroup;
  modalID = 'order-approval-comments';
  @Input() orderID: string;
  approveModal = ModalState.Closed;

  constructor(private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit(): void {
    this.form = new FormGroup({ comments: new FormControl('') });
  }

  openModal(approved: boolean): void {
    this.approved = approved;
    this.approveModal = ModalState.Open;
  }

  async submitReview(): Promise<void> {
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
