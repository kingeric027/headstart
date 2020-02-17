import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { faTrashAlt, faUpload, faCrown, } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
var ProductImagesComponent = /** @class */ (function () {
    function ProductImagesComponent(toastrService) {
        this.toastrService = toastrService;
        this.update = new EventEmitter();
        this.faTrash = faTrashAlt;
        this.faUpload = faUpload;
        this.faCrown = faCrown;
    }
    ProductImagesComponent.prototype.deleteImage = function (index) {
        this.product.xp.imageURLs.splice(index, 1);
        this.update.emit(this.product);
    };
    ProductImagesComponent.prototype.addImage = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            var file = fileList[0];
            // Make API call to image storage integration. API should return the url at which the file is stored.
            // Then, use commented out code below to save this URL in OrderCloud. Delete the toastr.
            // const url = 'http://example.com';
            // this.product.xp.imageURLs.push(url)
            // this.update.emit(this.product);
            var message = 'File upload functionality requires an integration with file storage. Developers can find details at https://github.com/ordercloud-api/ngx-shopper/blob/development/src/UI/Seller/src/app/product-management/components/product-images/product-images.component.ts';
            this.toastrService.warning(message, null, {
                disableTimeOut: true,
                closeButton: true,
                tapToDismiss: false,
            });
        }
    };
    // The image that appears on a buyer's product list is the first element of the imageURLs array.
    ProductImagesComponent.prototype.setPrimaryImage = function (url, index) {
        this.product.xp.imageURLs.splice(index, 1);
        this.product.xp.imageURLs.unshift(url);
        this.update.emit(this.product);
    };
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductImagesComponent.prototype, "product", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductImagesComponent.prototype, "update", void 0);
    ProductImagesComponent = __decorate([
        Component({
            selector: 'product-images',
            templateUrl: './product-images.component.html',
            styleUrls: ['./product-images.component.scss'],
        }),
        __metadata("design:paramtypes", [ToastrService])
    ], ProductImagesComponent);
    return ProductImagesComponent;
}());
export { ProductImagesComponent };
//# sourceMappingURL=product-images.component.js.map