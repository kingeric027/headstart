import { InjectionToken } from '@angular/core';
import { environment } from '../../environments/environment';
export var ocAppConfig = {
    appname: environment.appname,
    marketplaceID: environment.marketplaceID,
    cmsUrl: environment.cmsUrl,
    clientID: environment.clientID,
    middlewareUrl: environment.middlewareUrl,
    // sellerName is being hard-coded until this is available to store in OrderCloud
    sellerName: 'SEB Seller',
    scope: [
        // 'AdminAddressReader' is just for reading admin addresses as a seller user on product create/edti
        // Will need to be updated to 'AdminAddressAdmin' when seller address create is implemented
        'AdminAddressReader',
        'MeAddressAdmin',
        'MeAdmin',
        'BuyerUserAdmin',
        'UserGroupAdmin',
        'MeCreditCardAdmin',
        'MeXpAdmin',
        'Shopper',
        'CategoryReader',
        'ProductAdmin',
        // adding this for product editing and creation on the front end
        // this logic may be moved to the backend in the future and this might not be required
        'PriceScheduleAdmin',
        'SupplierReader',
        'SupplierAddressReader',
        'BuyerAdmin',
        'OverrideUnitPrice',
        'OrderAdmin',
        'OverrideTax',
        'OverrideShipping',
        'BuyerImpersonation',
        'AddressAdmin',
        'CategoryAdmin',
        'CatalogAdmin',
        'PromotionAdmin',
        'ApprovalRuleAdmin',
        'CreditCardAdmin',
        'SupplierAdmin',
        'SupplierUserAdmin',
        'SupplierAddressAdmin',
        'AdminUserAdmin',
        // custom roles used to conditionally display ui
        'MPMeProductAdmin',
        'MPMeProductReader',
        'MPProductAdmin',
        'MPProductReader',
        'MPPromotionAdmin',
        'MPPromotionReader',
        'MPCategoryAdmin',
        'MPCategoryReader',
        'MPOrderAdmin',
        'MPOrderReader',
        'MPShipmentAdmin',
        'MPBuyerAdmin',
        'MPBuyerReader',
        'MPSellerAdmin',
        'MPReportReader',
        'MPSupplierAdmin',
        'MPMeSupplierAdmin',
        'MPMeSupplierAddressAdmin',
        'MPMeSupplierUserAdmin',
    ],
};
export var applicationConfiguration = new InjectionToken('app.config', {
    providedIn: 'root',
    factory: function () { return ocAppConfig; },
});
//# sourceMappingURL=app.config.js.map