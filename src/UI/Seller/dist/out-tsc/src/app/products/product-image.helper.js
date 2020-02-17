import { __assign } from "tslib";
export var IMAGE_HOST_URL = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
export var PLACEHOLDER_URL = 'http://placehold.it/300x300';
export var PRODUCT_IMAGE_PATH_STRATEGY = 'PRODUCT_IMAGE_PATH_STRATEGY';
export function getProductMainImageUrlOrPlaceholder(product) {
    var imgUrls = getProductImageUrls(product);
    return imgUrls.length ? imgUrls[0] : PLACEHOLDER_URL;
}
export function ReplaceHostUrls(product) {
    var images = (product.xp && product.xp.Images) || [];
    return images.map(function (img) { return ReplaceHostUrl(img); });
}
function getProductImageUrls(product) {
    return ReplaceHostUrls(product)
        .map(function (image) { return image.URL; })
        .filter(function (url) { return url; });
}
function ReplaceHostUrl(img) {
    return __assign(__assign({}, img), { URL: img.URL.replace('{u}', IMAGE_HOST_URL) });
}
//# sourceMappingURL=product-image.helper.js.map