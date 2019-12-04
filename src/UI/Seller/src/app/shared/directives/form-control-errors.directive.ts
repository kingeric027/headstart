import { Directive, Self, ElementRef, OnInit, Renderer, Input, ChangeDetectorRef } from '@angular/core';
import { NgControl, FormGroup } from '@angular/forms';
import { ErrorDictionary } from '../../../app/validators/validators';

@Directive({
  selector: '[showErrors]',
})
export class FormControlErrorDirective implements OnInit {
  constructor(@Self() private control: NgControl, private el: ElementRef, private renderer: Renderer) {}

  @Input()
  formControlName: string;

  // resourceForm needs to be passed in to remove error messages when resetting the form
  // without changing the inputs, could be a better way
  @Input()
  set resourceForm(value: FormGroup) {
    // need this to remove the error when the selected resource is changed
    if (this.errorSpan) {
      this.errorSpan.innerHTML = '';
    }
  }

  errorSpan: HTMLElement;

  ngOnInit() {
    this.initializeSubscriptions();
  }

  initializeSubscriptions() {
    this.errorSpan = this.renderer.createElement(this.el.nativeElement.parentNode, 'span');
    this.renderer.setElementAttribute(this.errorSpan, 'class', 'error-message');
    (this.control as any).update.subscribe(this.displayErrorMsg);
    this.control.valueChanges.subscribe(this.displayErrorMsg);
    // submit button might not exist within the form directly or may not be on the page, might need to find another way to show erros on form submit
    // fromEvent(this.el.nativeElement.form, 'submit').subscribe(this.displayErrorMsg);
  }

  displayErrorMsg = () => {
    this.errorSpan.innerHTML = this.getErrorMsg(this.control);
  };

  getErrorMsg(control: NgControl) {
    if (!control.errors) return '';
    let controlErrors = Object.keys(control.errors);
    if (control.value) controlErrors = controlErrors.filter((x) => x !== 'required');
    if (controlErrors.length === 0) return '';
    return ErrorDictionary[controlErrors[0]];
  }
}
