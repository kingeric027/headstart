import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { fromEvent } from 'rxjs';
import { Asset } from '@ordercloud/headstart-sdk';

@Component({
  templateUrl: './image-gallery.component.html',
  styleUrls: ['./image-gallery.component.scss'],
})
export class OCMImageGallery implements OnInit, OnChanges {
  @Input() imgUrls: string[] = [];
  @Input() imgs: Asset[] = [];
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

  ngOnChanges(changes: SimpleChanges): void {
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

  isSelected(image: Asset): boolean {
    return this.imgUrls.indexOf(image.Url) === this.selectedIndex;
  }

  isImageMatchingSpecs(image: Asset): boolean {
    // Examine all specs, and find the image tag that matches all specs, removing spaces where needed on the spec to find that match.
    return this.specs.every(spec => image.Tags.find(tag => tag?.split('-').includes(spec.replace(/\s/g, ''))));
  }

  onSpecsChange(): void {
    let image;
    if (this.imgs.length === 1) {
      image = this.imgs[0];
    } else {
      image = this.imgs.find(img => this.isImageMatchingSpecs(img));
    }
    if (image) {
      this.select(image.Url);
    } else {
      // If no specs/tags match, grab primary image, or placeholder if no images exist
      this.imgs[0] ? this.select(this.imgs[0].Url) : this.select('http://placehold.it/500x500');
    }
  }

  getGallery(): Asset[] {
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
