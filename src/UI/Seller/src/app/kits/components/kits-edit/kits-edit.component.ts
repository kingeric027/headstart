import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AppAuthService } from '@app-seller/auth';
import { KitService } from '@app-seller/kits/kits.service';
import { MarketplaceKitProduct, MiddlewareKitService, ProductInKit } from '@app-seller/shared/services/middleware-api/middleware-kit.service';
import { faCircle, faHeart, faTimes, faTrash } from '@fortawesome/free-solid-svg-icons';
import { ListAddress, Product } from '@ordercloud/angular-sdk';
import { HeadStartSDK, SuperMarketplaceProduct } from '@ordercloud/headstart-sdk';
import { Router } from '@angular/router';
import { display } from 'html2canvas/dist/types/css/property-descriptors/display';
@Component({
    selector: 'app-kits-edit',
    templateUrl: './kits-edit.component.html',
    styleUrls: ['./kits-edit.component.scss'],
})
export class KitsEditComponent implements OnInit {
    kitProductEditable: MarketplaceKitProduct;
    kitProductStatic: MarketplaceKitProduct;
    kitProductForm: FormGroup;
    productsInKitForm: FormGroup[];
    @Input()
    set selectedKitProduct(product: MarketplaceKitProduct) {
        if (product.Product) this.handleSelectedProductChange(product);
        else {
            this.kitProductEditable = this.kitService.emptyResource;
            this.kitProductStatic = this.kitService.emptyResource;
        }
    }
    @Input() readonly: boolean;
    @Input()
    filterConfig;
    @Output()
    updateResource = new EventEmitter<any>();
    @Input()
    addresses: ListAddress;
    @Input()
    isCreatingNew: boolean;
    faTimes = faTimes;
    faTrash = faTrash;
    faCircle = faCircle;
    faHeart = faHeart;
    productAssignments: ProductInKit[] = [];
    productsIncluded: any[] = [];
    areChanges = false;
    dataIsSaving = false;
    productList: any;
    isAddingNewProduct = false;
    productToAdd: string;
    newProductAssignments: ProductInKit[] = [];
    constructor(
        private router: Router,
        private appAuthService: AppAuthService,
        private kitService: KitService,
        private middlewareKitService: MiddlewareKitService
    ) { }

    ngOnInit() {
        this.isCreatingNew = this.kitService.checkIfCreatingNew();
    }

    setForms(kitProduct: MarketplaceKitProduct): void {
        this.kitProductForm = new FormGroup({
            ID: new FormControl(kitProduct.Product.ID || ''),
            Name: new FormControl(kitProduct.Product.Name || ''),
            Active: new FormControl(kitProduct.Product.Active),
        });
    }

    handleUpdateProduct(event: any, field: string, typeOfValue?: string, product?: any): void {
        if (product?.ID) {
            const updatedAssignments = this.kitProductEditable.ProductAssignments.ProductsInKit;
            let index;
            for (let i = 0; i < updatedAssignments.length; i++) {
                if (updatedAssignments[i].ID === product.ID) { index = i; }
            }
            field = 'ProductAssignments.ProductsInKit[' + index + '].' + field;
        }
        const productUpdate = {
            field,
            value: typeOfValue === "boolean" ? event.target.checked :
                typeOfValue === 'number' ? Number(event.target.value) : event.target.value
        };
        this.updateProductResource(productUpdate);
    }

    updateProductResource(productUpdate: any): void {
        const resourceToUpdate = this.kitProductEditable || this.kitService.emptyResource;
        this.kitProductEditable = this.kitService.getUpdatedEditableResource(productUpdate, resourceToUpdate);
        this.checkForChanges();
    }

    async updateProduct(): Promise<void> {
        try {
            this.dataIsSaving = true;
            let superProduct = this.kitProductStatic;
            if (JSON.stringify(this.kitProductEditable) !== JSON.stringify(this.kitProductStatic)) {
                superProduct = await this.middlewareKitService.Update(this.kitProductEditable);
            }
            this.refreshProductData(superProduct);
            this.dataIsSaving = false;
        } catch (ex) {
            this.dataIsSaving = false;
            throw ex;
        }
    }

    async handleSelectedProductChange(product: MarketplaceKitProduct) {
        const marketplaceKitProduct = await this.middlewareKitService.Get(product.Product.ID);
        this.refreshProductData(marketplaceKitProduct);
    }

    handleDeleteAssignment(event: any, product: any) {
        const updatedAssignments = this.kitProductEditable.ProductAssignments.ProductsInKit;
        for (let i = 0; i < updatedAssignments.length; i++) {
            if (updatedAssignments[i].ID === product.ID) { updatedAssignments.splice(i, 1) }
        }
        const updatedProduct = {
            field: 'ProductAssignments.ProductsInKit',
            value: updatedAssignments
        }
        this.updateProductResource(updatedProduct);
    }

    selectProductToAdd(event: any) {
        this.productToAdd = event.target.value;
    }

    async handleCreateAssignment() {
        this.isAddingNewProduct = false;
        let ocProduct = await HeadStartSDK.Products.Get(this.productToAdd);
        const newProduct = {
            ID: this.productToAdd,
            Name: ocProduct.Product.Name,
            Price: ocProduct.PriceSchedule.PriceBreaks[0],
            Required: false, MinQty: 0, MaxQty: 0
        }
        const productInKit = { ID: this.productToAdd, Required: false, MinQty: 0, MaxQty: 0 }
        this.productsIncluded.push(newProduct);
        this.newProductAssignments.push(productInKit);
        this.kitProductEditable.ProductAssignments.ProductsInKit = this.newProductAssignments
        this.checkForChanges();
    }

    async createNewKitProduct(): Promise<void> {
        try {
            this.dataIsSaving = true;
            const superProduct = await this.middlewareKitService.Create(this.kitProductEditable);
            this.refreshProductData(superProduct);
            this.router.navigateByUrl(`/kitproducts/${superProduct.Product.ID}`);
            this.dataIsSaving = false;
        } catch (ex) {
            this.dataIsSaving = false;
            throw ex;
        }
    }

    async refreshProductData(product: MarketplaceKitProduct) {
        this.kitProductEditable = JSON.parse(JSON.stringify(product));
        this.kitProductStatic = JSON.parse(JSON.stringify(product));
        this.productsIncluded = await this.getProductsInKit(product);
        this.setForms(product);
        this.checkForChanges();
    }

    async getProductsInKit(product: MarketplaceKitProduct): Promise<any[]> {
        let productAssignments = [];
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        product.ProductAssignments.ProductsInKit.forEach(async p => {
            let ocProduct = await HeadStartSDK.Products.Get(p.ID, accessToken);
            productAssignments.push({
                ID: p.ID, Name: ocProduct.Product.Name, Price: ocProduct.PriceSchedule.PriceBreaks[0], Required: p.Required, MinQty: p.MinQty, MaxQty: p.MaxQty
            })
        });
        return productAssignments;
    }

    async getProductList() {
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        const productList = await HeadStartSDK.Products.List({ pageSize: 100 }, accessToken);
        this.productList = productList?.Items.filter(p => p.PriceSchedule.ID !== null);
    }
    async handleSave(): Promise<void> {
        if (this.isCreatingNew) this.createNewKitProduct();
        else this.updateProduct();
    }
    async handleDelete(): Promise<void> {
        await this.middlewareKitService.Delete(this.kitProductStatic.Product.ID);
        this.router.navigateByUrl('/kitproducts');
    }
    async addProductAssignment() {
        this.isAddingNewProduct = true;
        await this.getProductList();
    }
    getSaveBtnText(): string {
        return this.kitService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
    }
    checkForChanges(): void {
        this.areChanges = JSON.stringify(this.kitProductEditable) !== JSON.stringify(this.kitProductStatic);
    }
}
