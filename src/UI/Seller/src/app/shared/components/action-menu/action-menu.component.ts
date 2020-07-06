import { Component, Input, Output, EventEmitter } from '@angular/core';
import { getSupportedInputTypes } from '@angular/cdk/platform';

@Component({
  selector: 'action-menu-component',
  templateUrl: './action-menu.component.html',
  styleUrls: ['./action-menu.component.scss'],
})
export class ActionMenuComponent {
  @Input()
  areChanges = false;
  @Input()
  allowDiscard = false;
  @Input()
  allowDelete = false;
  @Input()
  isDeletable = false;
  @Input()
  saveTextOverride = '';
  @Input()
  resourceName = '';
  @Input()
  requireConfirmation = false;
  @Input()
  disableSave = false;
  @Input()
  confirmText = '';
  @Output()
  executeSaveAction = new EventEmitter<void>();
  @Output()
  executeDiscardAction = new EventEmitter<void>();

  showConfirm = false;

  requestUserConfirmation(): void {
    this.showConfirm = true;
  }

  emitSave(): void {
    this.executeSaveAction.emit();
  }

  handleSavePressed(): void {
    if (this.showConfirm) {
      this.requestUserConfirmation();
    } else {
      this.emitSave();
    }
  }

  handleDiscard(): void {
    this.executeDiscardAction.emit();
  }
}
