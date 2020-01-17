import { Directive, HostBinding, HostListener, Output, EventEmitter } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { MarketPlaceProductImage } from '../models/MarketPlaceProduct.interface';

export interface FileHandle {
  ExistingImage: MarketPlaceProductImage;
  File: File;
  Url: SafeUrl;
  Index?: number;
  Delete?: boolean;
}

@Directive({
  selector: '[appDrag]',
})
export class DragDirective {
  @Output()
  files = new EventEmitter<any>();

  @HostBinding('style.background-color')
  private background = '#f5fcff';
  @HostBinding('style.opacity')
  private opacity = '1';

  constructor(private sanitizer: DomSanitizer) {}

  //Dragover listener
  @HostListener('dragover', ['$event'])
  onDragOver(evt) {
    evt.preventDefault();
    evt.stopPropagation();
    this.background = '#9ecbec';
    this.opacity = '0.8';
  }

  //Dragleave listener
  @HostListener('dragleave', ['$event'])
  public onDragLeave(evt) {
    evt.preventDefault();
    evt.stopPropagation();
    this.background = '#f5fcff';
    this.opacity = '1';
  }

  //Drop listener
  @HostListener('drop', ['$event'])
  public onDrop(evt: DragEvent) {
    evt.preventDefault();
    evt.stopPropagation();
    this.background = '#eee';

    const files: FileHandle[] = [];
    Array.from(evt.dataTransfer.files).map((file) => {
      const File = file;
      const Url = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(File));
      files.push({ File, Url, ExistingImage: null });
    });
    if (files.length > 0) {
      this.files.emit(files);
    }
  }
}
