import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { FormControl } from '@angular/forms';
import {
    Buyer,
    OcBuyerService,
    UserGroup,
    ProductAssignment
} from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';
import { MarketplaceProduct, ChiliConfig, ChiliSpec, ChiliSpecXp, ChiliSpecUI } from '@ordercloud/headstart-sdk';
import { TecraDocument, TecraSpec } from '../../../../shared/services/middleware-api/middleware-chili.service';

@Component({
    selector: 'variable-print-configuration-component',
    templateUrl: './variable-print-configuration.component.html',
    styleUrls: ['./variable-print-configuration.component.scss'],
})
export class VariablePrintConfiguration implements OnInit {
    @Input()
    product: MarketplaceProduct;
    faExclamationCircle = faExclamationCircle;

    _buyerID = '';
    _productID = '';
    catalogs: UserGroup[] = [];
    catalogAssignments: ProductAssignment[][] = [];


    @Input()
    set productID(value: string) {
        this._productID = value;
    }

    @Input()
    set catalogID(value: string) {
        this._buyerID = value;
    }

    @Input()
    set availableCatelogs(value: UserGroup[]) {
        this.catalogs = value;
    }

    @Input()
    set productAssignments(value: ProductAssignment[][]) {
        this.catalogAssignments = value;      
    }

    constructor(
        private ocBuyerService: OcBuyerService,
        private productService: ProductService
    ) { }

    ngOnInit(): void {
        
    }
}
