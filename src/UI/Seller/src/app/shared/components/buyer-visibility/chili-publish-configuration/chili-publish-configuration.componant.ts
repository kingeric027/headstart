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
    selector: 'chili-publish-configuration-component',
    templateUrl: './chili-publish-configuration.componant.html',
    styleUrls: ['./chili-publish-configuration.componant.scss'],
})
export class ChiliPublishConfiguration implements OnInit, OnChanges {
    @Input()
    product: MarketplaceProduct;
    faExclamationCircle = faExclamationCircle;

    showChiliConfigs = false;
    showChiliDocuments = false;
    showChiliSpecs = false;
    showAvailableCategories = false;
    showAssignmentButton = false;

    _buyerID = '';
    _productID = '';
    _documentID = '';
    _documentName = '';
    _catalogID = '';

    tecraDocuments: TecraDocument[] = [];
    tecraSpecs: TecraSpec[] = [];
    catalogs: UserGroup[] = [];
    availableCatalogs: UserGroup[] = [];
    catalogAssignments: ProductAssignment[] = [];
    chiliConfigs: ChiliConfig[] = [];


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
        value.forEach(val => val.forEach(v => this.catalogAssignments.push(v)));
    }

    constructor(
        private ocBuyerService: OcBuyerService,
        private productService: ProductService
    ) { }

    ngOnInit(): void {
        this.getChiliDocs();
        this.listChiliConfigs();
    }

    ngOnChanges(): void {
        console.log("Something Changed.");
    }

    isAssigned(userGroupID: string): boolean {
        return this.catalogAssignments.some(c => c.UserGroupID === userGroupID);
    }

    async getChiliDocs(): Promise<void> {
        //TODO Update Buyer xp to associate folder(s) available to search on.
        const docs = await this.productService.getTecraDocuments("4511001");
        this.tecraDocuments = docs;
        this.showChiliDocuments = true;
    }

    async listChiliConfigs(): Promise<void> {
        //TODO - Update to only get configs assosociated to this buyer and product
        this.showChiliConfigs = false;
        this.chiliConfigs = [];
        const configs = await this.productService.listChiliConfigs();
        if (configs.Items.length > 0) {
            configs.Items.forEach(item => {
                item.ReadOnly = true;
                this.chiliConfigs.push(item);
            });
            this.showChiliConfigs = true;
        }
    }

    async selectChiliTemplate(event: any): Promise<void> {
        this.showAvailableCategories = false;
        this._documentID = event.target.value;
        this._documentName = this.tecraDocuments.find(x => x.id === this._documentID).name;

        //TODO - Update to only get configs assosociated to this buyer and product
        const specs = await this.productService.getTecraSpecs(this._documentID, "4511001");
        this.tecraSpecs = specs;

        console.log(this.tecraSpecs);
        this.showAvailableCategories = true;
    }

    async setCatalogSelection(event: any): Promise<void> {
        this.showAssignmentButton = false;
        this._catalogID = event.target.value;
        this.showAssignmentButton = true;
    }

    async executeChiliConfigAssignmentRequest(): Promise<void> {
        const requests = this.tecraSpecs.map((spec, index) => {
            const csui: ChiliSpecUI = {
                ControlType: (spec.dataType === "string") ? "Text" : "DropDown"
            }
            const csxp: ChiliSpecXp = {
                UI: csui
            }
            const tempSpec: ChiliSpec = {
                ListOrder: index + 1,
                Name: spec.name,
                DefaultValue: spec.displayValue,
                Required: (spec.required.toLowerCase() == "true") ? true : false,
                AllowOpenText: false,
                DefaultOptionID: "",
                xp: csxp
            };

            return this.productService.saveChiliSpec(tempSpec);
        });

        const chiliSpecs = await Promise.all(requests);
        const chilSpecIds = chiliSpecs.map(cspec => cspec.ID);

        //TODO - UPdate the Headstart SDK to use the BuyerID and CatalogID that I manually updated locally.
        const config: ChiliConfig =
        {
            SupplierProductID: this._productID,
            ChiliTemplateID: this._documentID,
            ChiliTemplateName: this._documentName,
            Specs: chilSpecIds,
            ID: "",
            BuyerID: this._buyerID,
            CatalogID: this._catalogID
        };

        debugger;
        await this.productService.saveChiliConfig(config);
        this.listChiliConfigs();
    }
    async deleteChiliConfig(config: ChiliConfig): Promise<void> {
        //Delete all associated specs
        const requests = config.Specs.map(cspec => {
            return this.productService.deleteChiliSpec(cspec);
        });
        await Promise.all(requests);

        //Delete Config after all specs are deleted
        await this.productService.deleteChiliConfig(config.ID);

        //Reset View
        this.listChiliConfigs();
    }

    async editChiliConfig(event: any): Promise<void> {
        event.ReadOnly = false;
    }
    async cancelChiliConfigEdit(event: any): Promise<void> {
        event.ReadOnly = true;
    }
    async updateChiliConfig(event: any): Promise<void> {
        event.ReadOnly = true;
        await this.productService.saveChiliConfig(event);
        this.listChiliConfigs();
    }
}
