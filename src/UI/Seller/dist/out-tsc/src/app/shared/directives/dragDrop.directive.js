import { __decorate, __metadata } from "tslib";
import { Directive, HostBinding, HostListener, Output, EventEmitter } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
var DragDirective = /** @class */ (function () {
    function DragDirective(sanitizer) {
        this.sanitizer = sanitizer;
        this.files = new EventEmitter();
        this.background = '#f5fcff';
        this.opacity = '1';
    }
    //Dragover listener
    DragDirective.prototype.onDragOver = function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
        this.background = '#9ecbec';
        this.opacity = '0.8';
    };
    //Dragleave listener
    DragDirective.prototype.onDragLeave = function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
        this.background = '#f5fcff';
        this.opacity = '1';
    };
    //Drop listener
    DragDirective.prototype.onDrop = function (evt) {
        var _this = this;
        evt.preventDefault();
        evt.stopPropagation();
        this.background = '#eee';
        var files = [];
        Array.from(evt.dataTransfer.files).map(function (file) {
            var File = file;
            var URL = _this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(File));
            files.push({ File: File, URL: URL });
        });
        if (files.length > 0) {
            this.files.emit(files);
        }
    };
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], DragDirective.prototype, "files", void 0);
    __decorate([
        HostBinding('style.background-color'),
        __metadata("design:type", Object)
    ], DragDirective.prototype, "background", void 0);
    __decorate([
        HostBinding('style.opacity'),
        __metadata("design:type", Object)
    ], DragDirective.prototype, "opacity", void 0);
    __decorate([
        HostListener('dragover', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], DragDirective.prototype, "onDragOver", null);
    __decorate([
        HostListener('dragleave', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], DragDirective.prototype, "onDragLeave", null);
    __decorate([
        HostListener('drop', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [DragEvent]),
        __metadata("design:returntype", void 0)
    ], DragDirective.prototype, "onDrop", null);
    DragDirective = __decorate([
        Directive({
            selector: '[appDrag]',
        }),
        __metadata("design:paramtypes", [DomSanitizer])
    ], DragDirective);
    return DragDirective;
}());
export { DragDirective };
//# sourceMappingURL=dragDrop.directive.js.map