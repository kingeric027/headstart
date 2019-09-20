import { Component, ElementRef, Input, OnInit, OnDestroy, Inject, Output, EventEmitter, OnChanges } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { DOCUMENT } from '@angular/common';

@Component({
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
})
export class OCMModal implements OnInit, OnChanges, OnDestroy {
  @Input() modalTitle: string;
  @Input() isOpen = false;
  @Output() close = new EventEmitter();
  faTimes = faTimes;
  wasOpen = false;

  constructor(private elementRef: ElementRef, @Inject(DOCUMENT) private document: any) {}

  ngOnInit(): void {
    // move element to bottom of page (just before </body>) so it can be displayed above everything else
    this.document.body.appendChild(this.elementRef.nativeElement);
  }

  ngOnChanges() {
    if (this.isOpen) {
      this.openModal();
    } else if (!this.isOpen) {
      this.closeModal();
    }
  }

  // remove self when directive is destroyed
  ngOnDestroy(): void {
    this.closeModal();
    this.elementRef.nativeElement.remove();
  }

  openModal(): void {
    this.elementRef.nativeElement.style.display = 'block';
    this.document.body.classList.add('shared-modal--open');
  }

  closeModal(): void {
    this.elementRef.nativeElement.style.display = 'none';
    this.document.body.classList.remove('shared-modal--open');
    this.close.emit();
    this.isOpen = false;
  }
}
