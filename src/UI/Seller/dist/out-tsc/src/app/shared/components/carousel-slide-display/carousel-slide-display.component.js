import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { faSave, faTrashAlt, faUpload, } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { RegexService } from '@app-seller/shared/services/regex/regex.service';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
var CarouselSlideDisplayComponent = /** @class */ (function () {
    function CarouselSlideDisplayComponent(formBuilder, toastrService, formErrorService, regexService) {
        var _this = this;
        this.formBuilder = formBuilder;
        this.toastrService = toastrService;
        this.formErrorService = formErrorService;
        this.regexService = regexService;
        this.save = new EventEmitter();
        this.delete = new EventEmitter();
        this.faSave = faSave;
        this.faTrash = faTrashAlt;
        this.faUpload = faUpload;
        this.hasPatternError = function (controlName) {
            return _this.formErrorService.hasPatternError(controlName, _this.carouselForm);
        };
    }
    CarouselSlideDisplayComponent.prototype.ngOnInit = function () {
        this.carouselForm = this.formBuilder.group({
            URL: this.slide.URL || '',
            headerText: [
                this.slide.headerText || '',
                Validators.pattern(this.regexService.HundredChar),
            ],
            bodyText: [
                this.slide.bodyText || '',
                Validators.pattern(this.regexService.HundredChar),
            ],
        });
    };
    CarouselSlideDisplayComponent.prototype.fileChange = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            var file = fileList[0];
            // Make API call to image storage integration. API should return the url at which the file is stored.
            // Then, use commented out code below to save this URL in OrderCloud. Delete the toastr.
            // const url = 'http://example.com';
            // this.carouselForm.setValue({ URL: url});
            // this.textChanges();
            var message = 'File upload functionality requires an integration with file storage. Developers can find details at https://github.com/ordercloud-api/ngx-shopper/blob/development/src/UI/Seller/src/app/shared/components/carousel-slide-display/carousel-slide-display.component.ts';
            this.toastrService.warning(message, null, {
                disableTimeOut: true,
                closeButton: true,
                tapToDismiss: false,
            });
        }
    };
    CarouselSlideDisplayComponent.prototype.textChanges = function () {
        if (this.saveDisabled())
            return;
        this.save.emit({ prev: this.slide, new: this.carouselForm.value });
    };
    CarouselSlideDisplayComponent.prototype.deleteSlide = function () {
        this.delete.emit({ prev: this.slide });
    };
    CarouselSlideDisplayComponent.prototype.saveDisabled = function () {
        return ((this.slide.headerText === this.carouselForm.value.headerText &&
            this.slide.bodyText === this.carouselForm.value.bodyText) ||
            !this.carouselForm.valid);
    };
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], CarouselSlideDisplayComponent.prototype, "slide", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], CarouselSlideDisplayComponent.prototype, "save", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], CarouselSlideDisplayComponent.prototype, "delete", void 0);
    CarouselSlideDisplayComponent = __decorate([
        Component({
            selector: 'shared-carousel-slide-display',
            templateUrl: './carousel-slide-display.component.html',
            styleUrls: ['./carousel-slide-display.component.scss'],
        }),
        __metadata("design:paramtypes", [FormBuilder,
            ToastrService,
            AppFormErrorService,
            RegexService])
    ], CarouselSlideDisplayComponent);
    return CarouselSlideDisplayComponent;
}());
export { CarouselSlideDisplayComponent };
//# sourceMappingURL=carousel-slide-display.component.js.map