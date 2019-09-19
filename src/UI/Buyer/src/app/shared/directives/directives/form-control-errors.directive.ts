import { Directive, Self, ElementRef, OnInit, Renderer } from '@angular/core';
import { NgControl } from '@angular/forms';
import { ErrorDictionary } from 'src/app/ocm-default-components/validators/validators';
import { fromEvent } from 'rxjs';

@Directive({
  selector: '[showErrors]',
})
export class FormControlErrorDirective implements OnInit {
  constructor(@Self() private control: NgControl, private el: ElementRef, private renderer: Renderer) {}

  errorSpan: HTMLElement;

  ngOnInit() {
    this.errorSpan = this.renderer.createElement(this.el.nativeElement.parentNode, 'span');
    this.renderer.setElementAttribute(this.errorSpan, 'class', 'error-message');
    (this.control as any).update.subscribe(this.displayErrorMsg);
    fromEvent(this.el.nativeElement.form, 'submit').subscribe(this.displayErrorMsg);
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
