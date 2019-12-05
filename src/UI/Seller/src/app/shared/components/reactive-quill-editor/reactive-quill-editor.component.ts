import { Component, Input, EventEmitter, Output } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { get as _get } from 'lodash';
import { ValidateRichTextDescription } from '@app-seller/validators/validators';

@Component({
  selector: 'reactive-quill-editor-component',
  templateUrl: './reactive-quill-editor.component.html',
  styleUrls: ['./reactive-quill-editor.component.scss'],
})
export class ReactiveQuillComponent {
  _updatedResource: any;
  _formControlForText: any;
  quillChangeSubscription: any;

  @Input()
  pathOnResource: string;
  @Output()
  resourceUpdated = new EventEmitter();
  @Input()
  set formControlForText(value: FormControl) {
    this._formControlForText = value;
    this.setQuillChangeEvent();
  }

  @Input()
  set resourceInSelection(resource: any) {
    this.setQuillChangeEvent();
  }

  setQuillChangeEvent() {
    if (this._formControlForText) {
      this.quillChangeSubscription = this._formControlForText.valueChanges.subscribe((newFormValue) => {
        this.resourceUpdated.emit({ field: this.pathOnResource, value: newFormValue });
      });
    }
  }
}
