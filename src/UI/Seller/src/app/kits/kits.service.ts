import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcPromotionService, Product } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';

@Injectable({
    providedIn: 'root',
})
export class KitService extends ResourceCrudService<Product> {
    emptyResource = {
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
        Images: [],
        Attachments: [],
        ProductAssignments: {
            ProductsInKit: [
                {
                    ID: null,
                    Required: false,
                    MinQty: null,
                    MaxQty: null
                },
                {
                    ID: null,
                    Required: false,
                    MinQty: null,
                    MaxQty: null
                }
            ]
        },
    };

    constructor(
        router: Router,
        activatedRoute: ActivatedRoute,
        ocPromotionService: OcPromotionService,
        currentUserService: CurrentUserService
    ) {
        super(router, activatedRoute, ocPromotionService, currentUserService, '/kitproducts', 'kitproducts');
    }
}
