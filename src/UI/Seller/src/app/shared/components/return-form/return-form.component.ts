import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { LineItemStatus } from '@app-seller/shared/models/order-status.interface';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LineItem, OcLineItemService, OcOrderService, Order } from '@ordercloud/angular-sdk';

@Component({
    selector: 'return-form',
    templateUrl: './return-form.component.html',
})
export class ReturnForm implements OnInit {
    returnForm: FormGroup;
    isSaving = false;
    @Input() order: Order;
    @Input() lineItem: LineItem;
    @Input() orderDirection: string;
    constructor(private modalService: NgbModal, private ocOrderService: OcOrderService, private ocLineItemService: OcLineItemService,) { }

    ngOnInit() { this.setReturnForm(); }

    open(content) {
        this.modalService
            .open(content, { ariaLabelledBy: 'return-form' })
    }
    setReturnForm(): void {
        this.returnForm = new FormGroup({
            Comment: new FormControl(this.order.xp?.OrderReturnInfo?.Comment || ''),
            Complete: new FormControl(null),
        });
    }

    async onReturnFormSubmit(): Promise<void> {
        this.isSaving = true;
        const comment = this.returnForm.value.Comment;
        const status = this.returnForm.value.Complete ? LineItemStatus.Returned : LineItemStatus.ReturnRequested;
        const resolved = this.returnForm.value.Complete;
        await this.ocOrderService.Patch(this.orderDirection, this.order.ID, { xp: { OrderReturnInfo: { Comment: comment, Resolved: resolved } } }).toPromise();
        await this.ocLineItemService.Patch(this.orderDirection, this.order.ID, this.lineItem.ID, { xp: { LineItemReturnInfo: { Resolved: resolved }, LineItemStatus: status } }).toPromise();
        this.isSaving = false;
    }
}
