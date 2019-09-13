import { Component, ElementRef, Input, OnInit, OnDestroy, ContentChildren, Directive, QueryList, ViewContainerRef } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

// Put this on a custom component within the <shared-model> tags if you want to reset it when the modal closes.
@Directive({ selector: '[ResetOnModalClose]' })
export class ResetDirective {
  constructor(private view: ViewContainerRef) {}

  get component() {
    return this.view['_data'].componentView.component;
  }
}

export interface IModalComponent {
  isOpen: boolean;
  open: () => void;
  close: () => void;
  onClose: (callback: () => void) => void;
}

@Component({
  selector: 'shared-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
})
export class ModalComponent implements OnInit, OnDestroy, IModalComponent {
  id: string;
  @Input() modalTitle: string;
  @ContentChildren(ResetDirective, { descendants: true })
  children: QueryList<ResetDirective>;
  isOpen = false;
  faTimes = faTimes;
  private onCloseCallback = () => {};

  constructor(private elementRef: ElementRef) {}

  ngOnInit(): void {
    // move element to bottom of page (just before </body>) so it can be displayed above everything else
    document.body.appendChild(this.elementRef.nativeElement);
  }

  // remove self when directive is destroyed
  ngOnDestroy(): void {
    this.close();
    this.elementRef.nativeElement.remove();
  }

  // open modal
  open(): void {
    this.isOpen = true;
    this.elementRef.nativeElement.style.display = 'block';
    document.body.classList.add('shared-modal--open');
  }

  // close modal
  close(): void {
    this.isOpen = false;
    // Only applies to components with the ResetDirective
    this.children.forEach((child) => child.component.ngOnInit());
    this.elementRef.nativeElement.style.display = 'none';
    document.body.classList.remove('shared-modal--open');
    this.onCloseCallback();
  }

  onClose(callBack: () => void) {
    this.onCloseCallback = callBack;
  }
}
