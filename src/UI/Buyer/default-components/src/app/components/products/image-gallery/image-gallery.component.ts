import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { fromEvent } from 'rxjs';

@Component({
  templateUrl: './image-gallery.component.html',
  styleUrls: ['./image-gallery.component.scss'],
})
export class OCMImageGallery implements OnInit, OnChanges {
  @Input() imgUrls: string[] = [];
  @Input() imgs: any[] = [];
  @Input() specs: any[] = [];

  // gallerySize can be changed and the component logic + behavior will all work. However, the UI may look wonky.
  readonly gallerySize = 5;
  selectedIndex = 0;
  startIndex = 0;
  endIndex = this.gallerySize - 1;
  faAngleLeft = faAngleLeft;
  faAngleRight = faAngleRight;
  isResponsiveView: boolean;

  constructor() {
    this.onResize();
  }

  ngOnInit(): void {
    fromEvent(window, 'resize').subscribe(() => this.onResize());
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.specs) {
      this.onSpecsChange();
    }
  }

  onResize(): void {
    // this.isResponsiveView = window.innerWidth > 900;
    this.isResponsiveView = true;
  }

  select(url: string): void {
    this.selectedIndex = this.imgUrls.indexOf(url);
  }

  isSelected(image): boolean {
    return this.imgUrls.indexOf(image.Url) === this.selectedIndex;
  }

  isImageMatchingSpecs(image): boolean {
    return this.specs.every(spec => image.Tags.includes(spec));
  }

  onSpecsChange() {
    let image;
    if (this.imgs.length === 1) {
     image = this.imgs[0];
    } else {
     image = this.imgs.find(img => (this.isImageMatchingSpecs(img)));
    }
    if (image) {
      this.select(image.Url);
    }
   }

  getGallery(): string[] {
    return this.imgs.slice(this.startIndex, this.endIndex + 1);
  }

  forward(): void {
    this.selectedIndex++;
    if (this.selectedIndex > Math.min(this.endIndex, this.imgUrls.length - 1)) {
      // move images over one
      this.startIndex++;
      this.endIndex++;
      if (this.selectedIndex === this.imgUrls.length) {
        // cycle to the beginning
        this.selectedIndex = 0;
        this.startIndex = 0;
        this.endIndex = this.gallerySize - 1;
      }
    }
  }

  backward(): void {
    this.selectedIndex--;
    if (this.selectedIndex < this.startIndex) {
      // move images over one
      this.startIndex--;
      this.endIndex--;
      if (this.selectedIndex === -1) {
        // cycle to the end
        this.selectedIndex = this.imgUrls.length - 1;
        this.endIndex = this.imgUrls.length - 1;
        this.startIndex = Math.max(this.imgUrls.length - this.gallerySize, 0);
      }
    }
  }
}
