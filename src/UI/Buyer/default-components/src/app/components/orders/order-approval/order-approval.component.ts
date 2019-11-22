import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ModalState } from 'src/app/models/modal-state.class';
import { OCMComponent } from '../../base-component';

@Component({
  templateUrl: './order-approval.component.html',
  styleUrls: ['./order-approval.component.scss'],
})
export class OCMOrderApproval extends OCMComponent implements OnInit {
  approved: boolean;
  form: FormGroup;
  modalID = 'order-approval-comments';
  @Input() orderID: string;
  approveModal = ModalState.Closed;

  constructor(private toasterService: ToastrService) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({ comments: new FormControl('') });
  }

  ngOnContextSet() {}

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
