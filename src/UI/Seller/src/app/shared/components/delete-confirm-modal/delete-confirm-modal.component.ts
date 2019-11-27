import { Component, Output, EventEmitter } from '@angular/core';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'delete-confirm-modal-component',
  templateUrl: './delete-confirm-modal.component.html',
})
export class DeleteConfirmModal {
  closeResult: string;
  @Output()
  deleteConfirmed = new EventEmitter();
  constructor(private modalService: NgbModal) {}

  open(content) {
    this.modalService
      .open(content, { ariaLabelledBy: 'delete-confirm-modal' })
      .result.then((result) => {
        this.deleteConfirmed.emit(null);
      })
      .catch(() => {});
  }
}
