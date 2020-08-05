import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AppAuthService } from '@app-seller/auth';
import { KitService } from '@app-seller/kits/kits.service';
import { MarketplaceKitProduct, MiddlewareKitService, ProductInKit } from '@app-seller/shared/services/middleware-api/middleware-kit.service';
import { faCircle, faHeart, faTimes, faTrash } from '@fortawesome/free-solid-svg-icons';
import { ListAddress, Product } from '@ordercloud/angular-sdk';
import { HeadStartSDK } from '@ordercloud/headstart-sdk';
import { Router } from '@angular/router';
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
    productsInKit: Product[];
    productAssignments: ProductInKit[];
    areChanges = false;
    dataIsSaving = false;

    constructor(
        private router: Router,
        private appAuthService: AppAuthService,
        private kitService: KitService,
        private middlewareKitService: MiddlewareKitService
    ) { }

    ngOnInit() { }

    setForms(kitProduct: MarketplaceKitProduct): void {
        this.kitProductForm = new FormGroup({
            ID: new FormControl(kitProduct.Product.ID || ''),
            Name: new FormControl(kitProduct.Product.Name || ''),
            Active: new FormControl(kitProduct.Product.Active),
        });
    }

    handleUpdateProduct(event: any, field: string, typeOfValue?: string, i?: number): void {
        if (i !== undefined) field = 'ProductAssignments.ProductsInKit[' + i + '].' + field;
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

    handleDeleteAssignment(event: any, index: number) {
        const updatedAssignments = this.kitProductEditable.ProductAssignments.ProductsInKit;
        updatedAssignments.splice(index, 1);
        const updatedProduct = {
            field: 'ProductAssignments.ProductsInKit',
            value: updatedAssignments
        }
        this.updateProductResource(updatedProduct);
    }

    async refreshProductData(product: MarketplaceKitProduct) {
        this.kitProductEditable = JSON.parse(JSON.stringify(product));
        this.kitProductStatic = JSON.parse(JSON.stringify(product));
        this.productAssignments = product?.ProductAssignments?.ProductsInKit;
        this.productsInKit = await this.getProductsInKit(product);
        this.setForms(product);
        this.checkForChanges();
    }

    async getProductsInKit(product: MarketplaceKitProduct): Promise<Product[]> {
        let productsInKit = [];
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        product.ProductAssignments.ProductsInKit.forEach(async product => {
            let ocProduct = await HeadStartSDK.Products.Get(product.ID, accessToken);
            productsInKit.push(ocProduct.Product);
        });
        return productsInKit;
    }
    async handleSave(): Promise<void> {
        this.updateProduct();
    }
    async handleDelete(): Promise<void> {
        await this.middlewareKitService.Delete(this.kitProductStatic.Product.ID);
        this.router.navigateByUrl('/kitproducts');
    }

    getSaveBtnText(): string {
        return this.kitService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
    }
    checkForChanges(): void {
        this.areChanges = JSON.stringify(this.kitProductEditable) !== JSON.stringify(this.kitProductStatic);
    }
}
