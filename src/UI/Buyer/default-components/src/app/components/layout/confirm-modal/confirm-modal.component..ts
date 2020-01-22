import { Component, Output, EventEmitter, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    templateUrl: './confirm-modal.component.html',
    styleUrls: ['./confirm-modal.component.scss'],
})
export class ConfirmModal {
    @Input()
    modalTitle: string;
    @Input()
    description: string;
}