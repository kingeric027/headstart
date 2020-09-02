import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { MiddlewareKitService } from '../shared/services/middleware-api/middleware-kit.service';
import { MarketplaceKitProduct } from '@ordercloud/headstart-sdk';
@Injectable({
    providedIn: 'root',
})
export class KitService extends ResourceCrudService<MarketplaceKitProduct> {
    emptyResource = {
        ID: null,
        Name: null,
        Product: {
            OwnerID: '',
            DefaultPriceScheduleID: '',
            AutoForward: false,
            Active: false,
            ID: null,
            Name: null,
            Description: null,
            QuantityMultiplier: 1,
            ShipWeight: null,
            ShipHeight: null,
            ShipWidth: null,
            ShipLength: null,
            ShipFromAddressID: null,
            Inventory: null,
            DefaultSupplierID: null,
            xp: {
                IntegrationData: null,
                IsResale: false,
                Facets: {},
                Images: [],
                Status: null,
                HasVariants: false,
                Note: '',
                Tax: {
                    Category: 'P0000000', // SEB-827 default tax category to TPP
                    Code: null,
                    Description: null,
                },
                UnitOfMeasure: {
                    Unit: null,
                    Qty: null,
                },
                ProductType: null,
                StaticContent: null,
            },
        },
        Images: null,
        Attachments: null,
        ProductAssignments: {
            ProductsInKit: [
                {
                    ID: null,
                    MinQty: null,
                    MaxQty: null,
                    Static: false,
                    Variants: [],
                    SpecCombo: '',
                },
            ]
        },
    };

    constructor(
        router: Router,
        activatedRoute: ActivatedRoute,
        middlewareKitService: MiddlewareKitService,
        currentUserService: CurrentUserService
    ) {
        super(router, activatedRoute, middlewareKitService, currentUserService, '/kitproducts', 'kitproducts');
    }
}
