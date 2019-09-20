import { Component, ElementRef, Input, OnInit, OnDestroy, Inject } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'shared-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
})
export class ModalComponent implements OnInit, OnDestroy {
  id: string;
  @Input() modalTitle: string;
  isOpen = false;
  faTimes = faTimes;
  private onCloseCallback = () => {};

  constructor(private elementRef: ElementRef, @Inject(DOCUMENT) private document: any) {}

  ngOnInit(): void {
    // move element to bottom of page (just before </body>) so it can be displayed above everything else
    this.document.body.appendChild(this.elementRef.nativeElement);
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
    this.document.body.classList.add('shared-modal--open');
  }

  // close modal
  close(): void {
    this.isOpen = false;
    // Only applies to components with the ResetDirective
    this.elementRef.nativeElement.style.display = 'none';
    this.document.body.classList.remove('shared-modal--open');
    this.onCloseCallback();
  }

  onClose(callBack: () => void) {
    this.onCloseCallback = callBack;
  }
}
