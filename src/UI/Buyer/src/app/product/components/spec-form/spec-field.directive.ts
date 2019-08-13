import {
  ComponentFactoryResolver,
  Directive,
  Input,
  ViewContainerRef,
  OnInit,
  ComponentRef,
  OnChanges,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { SpecFormInputComponent } from './spec-form-input/spec-form-input.component';
import { SpecFormSelectComponent } from './spec-form-select/spec-form-select.component';
import { SpecFormButtonComponent } from './spec-form-button/spec-form-button.component';
import { SpecFormNumberComponent } from './spec-form-number/spec-form-number.component';
import { SpecFormTextAreaComponent } from './spec-form-textarea/spec-form-textarea.component';
import { SpecFormAddToCartComponent } from './spec-form-add-to-cart/spec-form-add-to-cart.component';
import { FieldConfig } from './field-config.interface';
import { Field } from './field.interface';
import { SpecFormCheckboxComponent } from './spec-form-checkbox/spec-form-checkbox.component';

const components = {
  button: SpecFormButtonComponent,
  input: SpecFormInputComponent,
  number: SpecFormNumberComponent,
  select: SpecFormSelectComponent,
  textarea: SpecFormTextAreaComponent,
  checkbox: SpecFormCheckboxComponent,
  addtocart: SpecFormAddToCartComponent,
};

@Directive({
  selector: '[ocSpecField]',
})
export class SpecFieldDirective implements Field, OnChanges, OnInit {
  @Input() config: FieldConfig;
  @Input() group: FormGroup;
  @Input() index: number;
  component: ComponentRef<Field>;

  constructor(
    private resolver: ComponentFactoryResolver,
    private container: ViewContainerRef
  ) {}

  ngOnChanges() {
    if (this.component) {
      this.component.instance.config = this.config;
      this.component.instance.group = this.group;
      this.component.instance.index = this.index;
    }
  }

  ngOnInit() {
    if (!components[this.config.type]) {
      const supportedTypes = Object.keys(components).join(', ');
      throw new Error(
        `Trying to use an unsupported type (${this.config.type}).
        Supported types: ${supportedTypes}`
      );
    }
    const component = this.resolver.resolveComponentFactory<Field>(
      components[this.config.type]
    );
    this.component = this.container.createComponent(component);
    this.component.instance.config = this.config;
    this.component.instance.group = this.group;
    this.component.instance.index = this.index;
  }
}
