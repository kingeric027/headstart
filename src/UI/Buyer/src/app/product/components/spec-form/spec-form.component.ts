import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { FieldConfig } from './field-config.interface';

@Component({
  selector: 'product-spec-form',
  template: `
    <form [formGroup]="form" (submit)="handleSubmit($event)">
      <ng-container
        *ngFor="let field of config; let i = index"
        class="my-1"
        formArrayName="ctrls"
        [config]="field"
        [group]="form"
        [index]="i"
        ocSpecField
      ></ng-container>
    </form>
  `,
  styleUrls: ['./spec-form.component.scss'],
})
export class SpecFormComponent implements OnInit {
  @Input() config: FieldConfig[] = [];
  @Output() submit: EventEmitter<any> = new EventEmitter<any>();
  @Output() change: EventEmitter<any> = new EventEmitter<any>();

  form: FormGroup;

  get controls() {
    return this.config.filter(({ type }) => type !== 'button');
  }
  get changes() {
    return this.form.valueChanges;
  }
  get valid() {
    return this.form.valid;
  }
  get value() {
    return this.form.value;
  }

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.form = this.createGroup();
    this.form.valueChanges.subscribe((e: any) => {
      const values: any = {};
      for (const value in e) {
        if (value !== 'ctrls') {
          values[value] = e[value];
        }
      }
      this.change.emit({ event: 'OnChange', values: values });
    });
  }

  createGroup() {
    const group = this.fb.group({
      ctrls: this.fb.array([]),
    });
    this.config.forEach((control) => {
      const ctrl = this.createControl(control);
      group.addControl(control.name, ctrl);
      group.controls['ctrls']['push'](ctrl);
    });
    return group;
  }

  createControl(config: FieldConfig) {
    const { disabled, validation, value } = config;
    return this.fb.control({ disabled, value }, validation);
  }

  handleSubmit(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.submit.emit({ event: event, values: this.value });
  }

  setDisabled(name: string, disable: boolean) {
    if (this.form.controls[name]) {
      const method = disable ? 'disable' : 'enable';
      this.form.controls[name][method]();
      return;
    }

    this.config = this.config.map((item) => {
      if (item.name === name) {
        item.disabled = disable;
      }
      return item;
    });
  }

  setValue(name: string, value: any) {
    this.form.controls[name].setValue(value, { emitEvent: true });
  }
}
