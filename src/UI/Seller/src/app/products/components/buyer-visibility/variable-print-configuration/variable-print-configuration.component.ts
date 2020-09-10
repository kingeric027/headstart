import { Component, OnInit, Input, OnChanges } from '@angular/core';
import {
    Buyer,
    OcBuyerService
} from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';
import { MarketplaceProduct } from '@ordercloud/headstart-sdk';
import { ChiliDocuments, ChiliSpecs } from '../../../../shared/services/middleware-api/middleware-chili.service';

@Component({
    selector: 'variable-print-configuration-component',
    templateUrl: './variable-print-configuration.component.html',
    styleUrls: ['./variable-print-configuration.component.scss'],
})
export class VariablePrintConfiguration implements OnInit, OnChanges {
    @Input()
    product: MarketplaceProduct;
    buyers: Buyer[];
    faExclamationCircle = faExclamationCircle;
    chiliDocuments: ChiliDocuments;
    chiliSpecs: ChiliSpecs;
    showChiliDocuments = false;
    showChiliSpecs = false;

    constructor(
        private ocBuyerService: OcBuyerService,
        private productService: ProductService
    ) { }

    ngOnInit(): void {
        this.getBuyers();
        this.getChiliDocs();
    }

    ngOnChanges(): void {
        console.log("Something Changed.");
    }

    async getBuyers(): Promise<void> {
        const buyers = await this.ocBuyerService.List().toPromise();
        this.buyers = buyers.Items;
    }

    async getChiliDocs(): Promise<void> {
        //TODO Update Buyer xp to associate folder(s) available to search on.
        const docs = await this.productService.getChiliDocuments("four51");
        console.log(docs);
        this.chiliDocuments = docs;
        this.showChiliDocuments = true;
    }

    async getDocumentSpecs(event: any): Promise<void> {
        this.showChiliSpecs = false;
        const documentID = event.target.value;
        const specs = await this.productService.getChiliSpecs(documentID);
        this.chiliSpecs = specs;
        console.log(specs);
        this.showChiliSpecs = true;
    }
}
