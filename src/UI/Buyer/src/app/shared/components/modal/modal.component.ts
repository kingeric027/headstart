import {
  Component,
  ElementRef,
  Input,
  OnInit,
  OnDestroy,
  ContentChildren,
  Directive,
  QueryList,
  ViewContainerRef,
  PLATFORM_ID,
  Inject,
} from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isPlatformBrowser } from '@angular/common';

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

  constructor(private elementRef: ElementRef, @Inject(PLATFORM_ID) private platformId: Object) {}

  ngOnInit(): void {
    // move element to bottom of page (just before </body>) so it can be displayed above everything else
    if (isPlatformBrowser(this.platformId)) {
      document.body.appendChild(this.elementRef.nativeElement);
    }
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
    if (isPlatformBrowser(this.platformId)) {
      document.body.classList.add('shared-modal--open');
    }
  }

  // close modal
  close(): void {
    this.isOpen = false;
    // Only applies to components with the ResetDirective
    this.children.forEach((child) => child.component.ngOnInit());
    this.elementRef.nativeElement.style.display = 'none';
    if (isPlatformBrowser(this.platformId)) {
      document.body.classList.remove('shared-modal--open');
    }
    this.onCloseCallback();
  }

  onClose(callBack: () => void) {
    this.onCloseCallback = callBack;
  }
}
