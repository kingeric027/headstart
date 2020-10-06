import { isValidLength } from './../../../services/card-validation.helper';
import { SuperMarketplaceProduct, MarketplaceVariant } from '@ordercloud/headstart-sdk';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormBuilder, AbstractControl } from '@angular/forms';
import { FormGroup, Validators } from '@angular/forms';
import { map as _map, find as _find } from 'lodash';

import { FieldConfig } from './field-config.interface';
import { SpecOption, Spec, ListPage } from 'ordercloud-javascript-sdk';
import { SpecFormEvent } from './spec-form-values.interface';
import { ShopperContextService } from 'marketplace';

@Component({
  template: `
    <form *ngIf="form" [formGroup]="form">
      <ng-container
        *ngFor="let field of config; let i = index"
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
export class OCMSpecForm {
  _specs: ListPage<Spec>;
  isValidAvailability: boolean;
  @Output() specFormChange: EventEmitter<SpecFormEvent> = new EventEmitter<SpecFormEvent>();
  @Output() onSelectionInactive: EventEmitter<boolean> = new EventEmitter<boolean>();
  config: FieldConfig[] = [];
  form: FormGroup;
 
  @Input() currency: string;
  @Input() disabledVariants: MarketplaceVariant[]
  @Input() set specs(value: ListPage<Spec>) {
    this._specs = value;
    this.init();
  } 

  constructor(private fb: FormBuilder, private context: ShopperContextService) { }

  init(): void {
    this.config = this.createFieldConfig();
    this.form = this.createGroup();
    this.form.valueChanges.subscribe(() => {
      this.handleChange();
    });
    // trigger change to update referencing components form validity
    this.handleChange();
  }

  createGroup(): FormGroup {
    const group = this.fb.group({
      ctrls: this.fb.array([]),
    });
    this.config.forEach(control => {
      const ctrl = this.createControl(control);
      group.addControl(control.name, ctrl);
      // tslint:disable-next-line:no-string-literal
      (group.controls.ctrls as any).push(ctrl);
    });
    return group;
  }

  createFieldConfig(): FieldConfig[] {
    const c: FieldConfig[] = [];
    if (!this._specs || !this._specs.Items) return c;
    for (const spec of this._specs.Items) {
      if (spec?.xp?.control === 'checkbox') {
        c.push(this.createCheckboxField(spec));
      } else if (spec?.xp?.control === 'range') {
        c.push(this.createRangeField(spec));
      } else if (spec?.Options.length === 1) {
        c.unshift(this.createLabelField(spec));
      } else if (spec?.Options.length > 1) {
        c.push(this.createSelectField(spec));
      } else if (spec.AllowOpenText) {
        c.push(this.createInputField(spec));
      }
    }
    return c;
  }

  createControl(config: FieldConfig): FormControl {
    const { disabled, validation, value } = config;
    return new FormControl({ disabled, value }, validation);
  }

  handleChange(): void { 
    console.log(this.form);
    console.log(this.disabledVariants);
    this.validateChangeAvailability(this.form, this.disabledVariants);
    this.specFormChange.emit({
      type: 'Change',
      valid: this.form.valid,
      form: this.form.value,
    } as SpecFormEvent);
  }

  validateChangeAvailability(form: FormGroup, disabledVariants: MarketplaceVariant[]) {

    for(let disabledVariant of disabledVariants){
      if (this.isControlInactive(form.value.ctrls, disabledVariant)) {
        this.onSelectionInactive.emit(true);
      }
    }

    form.value.ctrls.forEach(control => {
      if (!this.isControlAvailable(control, disabledVariants)){
        //control isn't available.
      }
    });
    this.isValidAvailability = true;
    let matchingSpecCount: number;
    for (let control of form.value.ctrls){
       disabledVariants.forEach(variant => {
         matchingSpecCount = 0;
         for (let spec of variant.Specs) {
           if (spec.Value.toUpperCase() != control.toUpperCase()){
             break;
           }
         }
       })
    }
    // disabledVariants.forEach(variants => {
    //   for (let spec of variants.Specs){
    //     if (!form.value.ctrls.contains(spec.Value)) { break; }

    //   }
     
    //   if (form.value.ctrls.contains(variants.Specs)){

    //   }
    
  }
  isControlInactive(ctrls: string[], disabledVariant: MarketplaceVariant): boolean {
    let controlCount = 0;
   for (let variant of disabledVariant.Specs) {
     ctrlLoop:
     for (let controlValue of ctrls){
       if (variant.Value == controlValue) {
        console.log(`ctrls: ${controlValue} and variant: ${variant.Value} almost matching!`)
        controlCount = controlCount + 1;
        if (controlCount ==  ctrls.length){ console.log(`${controlValue} matches ${variant.Value} twice now!`); return true; }
        break ctrlLoop;
       }
     }
   }
   return false;
  }

  isControlAvailable(control: AbstractControl, disabledVariants: MarketplaceVariant[]): boolean {
    for (let variant of disabledVariants){
      let isAvailable = false;
      for (let spec of variant.Specs) {
       console.log(`spec: ${spec.Value} and control: ${control}`)
      }
    }
    disabledVariants.forEach(element => {

    });
    return true;
  }

  private createCheckboxField(spec: Spec): FieldConfig {
    return {
      type: 'checkbox',
      label: spec.Name,
      name: spec.Name.replace(/ /g, ''),
      value: spec.DefaultOptionID,
      options: _map(spec.Options, 'Value'),
      validation: [Validators.nullValidator],
    }
  }

  private createRangeField(spec: Spec): FieldConfig {
    return {
      type: 'range',
      label: spec.Name,
      name: spec.Name.replace(/ /g, ''),
      value: spec.DefaultValue,
      min: Math.min(..._map(spec.Options, (option: SpecOption) => +option.Value)),
      max: Math.max(..._map(spec.Options, (option: SpecOption) => +option.Value)),
      validation: [
        spec.Required ? Validators.required : Validators.nullValidator,
        Validators.min(Math.min(..._map(spec.Options, (option: SpecOption) => +option.Value))),
        Validators.max(Math.max(..._map(spec.Options, (option: SpecOption) => +option.Value))),
      ],
    }
  }

  private createSelectField(spec: Spec): FieldConfig {
    return {
      type: 'select',
      label: spec.Name,
      name: spec.Name.replace(/ /g, ''),
      value: spec.DefaultOptionID
        ? _find(spec.Options, option => {
          return option.ID === spec.DefaultOptionID;
        }).Value
        : null,
      options: _map(spec.Options),
      disabledVariants: this.disabledVariants,
      validation: [spec.Required ? Validators.required : Validators.nullValidator],
      currency: this.context.currentUser.get().Currency
    }
  }

  private createLabelField(spec: Spec): FieldConfig {
    return {
      type: 'label',
      label: spec.Name,
      name: spec.Name.replace(/ /g, ''),
      options: _map(spec.Options, 'Value'),
    }
  }

  private createInputField(spec: Spec): FieldConfig
  {
    return {
      type: 'input',
      label: spec.Name,
      name: spec.Name.replace(/ /g, ''),
      value: spec.DefaultValue,
      validation: [spec.Required ? Validators.required : Validators.nullValidator],
    }
  }
}
