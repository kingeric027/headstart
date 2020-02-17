import { Directive, Self, ElementRef, OnInit, Renderer2 } from '@angular/core';
import { NgControl } from '@angular/forms';
import { fromEvent } from 'rxjs';
import { ErrorDictionary } from '../validators/validators';

@Directive({
  selector: 'showErrors',
})
export class FormControlErrorDirective implements OnInit {
  errorSpan: HTMLElement;

  constructor(@Self() private control: NgControl, private el: ElementRef, private renderer: Renderer2) {}

  ngOnInit(): void {
    this.errorSpan = this.renderer.createElement(this.el.nativeElement.parentNode, 'span');
    this.renderer.setAttribute(this.errorSpan, 'class', 'error-message');
    (this.control as any).update.subscribe(this.displayErrorMsg);
    fromEvent(this.el.nativeElement.form, 'submit').subscribe(this.displayErrorMsg);
  }

  displayErrorMsg = (): void => {
    this.errorSpan.innerHTML = this.getErrorMsg(this.control);
  }

  getErrorMsg(control: NgControl): string {
    if (!control.errors) return '';
    let controlErrors = Object.keys(control.errors);
    if (control.value) controlErrors = controlErrors.filter(x => x !== 'required');
    if (controlErrors.length === 0) return '';
    return ErrorDictionary[controlErrors[0]];
  }
}
