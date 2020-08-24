import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AppAuthService } from '@app-seller/auth';
import { KitService } from '@app-seller/kits/kits.service';
import { faCircle, faHeart, faTimes, faTrash } from '@fortawesome/free-solid-svg-icons';
import { ListAddress, Product } from '@ordercloud/angular-sdk';
import { HeadStartSDK, Asset, AssetUpload, ListPage } from '@ordercloud/headstart-sdk';
import { Router } from '@angular/router';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MiddlewareKitService, MarketplaceKitProduct, ProductInKit } from '@app-seller/shared/services/middleware-api/middleware-kit.service';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
@Component({
    selector: 'app-kits-edit',
    templateUrl: './kits-edit.component.html',
    styleUrls: ['./kits-edit.component.scss'],
})
export class KitsEditComponent implements OnInit {
    kitProductEditable: MarketplaceKitProduct;
    kitProductStatic: MarketplaceKitProduct;
    kitProductForm: FormGroup;
    @Input()
    set selectedKitProduct(product: MarketplaceKitProduct) {
        if (product?.Product) this.handleSelectedProductChange(product);
        else {
            this.setForms(this.kitService.emptyResource);
            this.kitProductEditable = this.kitService.emptyResource;
            this.kitProductStatic = this.kitService.emptyResource;
        }
    }
    faTimes = faTimes;
    faTrash = faTrash;
    faCircle = faCircle;
    faHeart = faHeart;
    @Input() readonly: boolean;
    @Input()
    filterConfig;
    @Output()
    updateResource = new EventEmitter<any>();
    isCreatingNew: boolean;
    isLoading = false;
    dataIsSaving = false;
    areChanges = false;
    productAssignments: ProductInKit[] = [];
    productsIncluded: any[] = [];
    productList: ListPage<Product>;
    productsToAdd: string[] = [];
    newProductAssignments: ProductInKit[] = [];
    imageFiles: FileHandle[] = [];
    images: Asset[] = [];
    staticContentFiles: FileHandle[] = [];
    staticContent: Asset[] = [];
    documentName: string;
    searchTermInput: string;
    constructor(
        private router: Router,
        private appAuthService: AppAuthService,
        private kitService: KitService,
        private middlewareKitService: MiddlewareKitService,
        private sanitizer: DomSanitizer,
        private modalService: NgbModal,
    ) { }

    async ngOnInit() {
        this.isCreatingNew = this.kitService.checkIfCreatingNew();
        this.productList = await this.getProductList();
    }

    setForms(kitProduct: MarketplaceKitProduct): void {
        this.kitProductForm = new FormGroup({
            ID: new FormControl(kitProduct.Product.ID),
            Name: new FormControl(kitProduct.Product.Name),
            Active: new FormControl(kitProduct.Product.Active)
        });
    }

    async getProductList(args?: ListArgs): Promise<ListPage<Product>> {
        this.isLoading = true;
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        const productList = args ? await HeadStartSDK.Products.List(args, accessToken) : await HeadStartSDK.Products.List({}, accessToken);
        this.isLoading = false;
        return { Meta: productList.Meta, Items: productList?.Items.filter(p => p.PriceSchedule.ID !== null) };
    }

    async handleSelectedProductChange(product: MarketplaceKitProduct) {
        const marketplaceKitProduct = this.isCreatingNew ?
            this.kitService.emptyResource : await this.middlewareKitService.Get(product.Product.ID);
        this.refreshProductData(marketplaceKitProduct);
    }

    async refreshProductData(product: MarketplaceKitProduct) {
        this.kitProductEditable = JSON.parse(JSON.stringify(product));
        this.kitProductStatic = JSON.parse(JSON.stringify(product));
        this.staticContent = product.Attachments;
        this.images = product.Images;
        this.setForms(product);
        this.isCreatingNew = this.kitService.checkIfCreatingNew();
        if (!this.isCreatingNew) this.getProductsInKit(product);
        this.checkForChanges();
    }

    async getProductsInKit(product: MarketplaceKitProduct): Promise<void> {
        let productAssignments = [];
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        product.ProductAssignments.ProductsInKit.forEach(async p => {
            let ocProduct = await HeadStartSDK.Products.Get(p.ID, accessToken);
            productAssignments.push({
                ID: p.ID, Name: ocProduct.Product.Name, Variants: p.Variants, MinQty: p.MinQty, MaxQty: p.MaxQty, Static: p.Static
            });
        });
        this.productsIncluded = productAssignments;
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
            if (this.imageFiles.length > 0) await this.addImages(this.imageFiles, superProduct.Product.ID);
            if (this.staticContentFiles.length > 0) { await this.addDocuments(this.staticContentFiles, superProduct.Product.ID); }
            this.refreshProductData(superProduct);
            this.dataIsSaving = false;
        } catch (ex) {
            this.dataIsSaving = false;
            throw ex;
        }
    }

    async createNewKitProduct(): Promise<void> {
        try {
            this.dataIsSaving = true;
            const superProduct = await this.middlewareKitService.Create(this.kitProductEditable);
            this.refreshProductData(superProduct);
            if (this.imageFiles.length > 0) await this.addImages(this.imageFiles, superProduct.Product.ID);
            if (this.staticContentFiles.length > 0) await this.addDocuments(this.staticContentFiles, superProduct.Product.ID);
            this.router.navigateByUrl(`/kitproducts/${superProduct.Product.ID}`);
            this.dataIsSaving = false;
        } catch (ex) {
            this.dataIsSaving = false;
            throw ex;
        }
    }

    async changePage(page: number) {
        this.productList = await this.getProductList({ page })
    }

    async handleSave(): Promise<void> {
        if (this.isCreatingNew) this.createNewKitProduct();
        else this.updateProduct();
    }

    async handleDelete(): Promise<void> {
        await this.middlewareKitService.Delete(this.kitProductStatic.Product.ID);
        this.router.navigateByUrl('/kitproducts');
    }

    handleDiscardChanges(): void {
        this.imageFiles = [];
        this.staticContentFiles = [];
        this.kitProductEditable = this.kitProductStatic;
        this.refreshProductData(this.kitProductStatic);
    }

    getSaveBtnText(): string {
        return this.kitService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
    }

    checkForChanges(): void {
        this.areChanges = JSON.stringify(this.kitProductEditable) !== JSON.stringify(this.kitProductStatic) ||
            this.imageFiles?.length > 0 || this.staticContentFiles?.length > 0;
    }

    /*********************************************
    ******** PRODUCT ASSIGNMENT FUNCTIONS ********
    *********************************************/

    async handleCreateAssignment() {
        let updatedAssignments = this.isCreatingNew ? [] : this.kitProductEditable.ProductAssignments.ProductsInKit;
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        await this.asyncForEach(this.productsToAdd, async (productID) => {
            let ocProduct = await HeadStartSDK.Products.Get(productID, accessToken);
            const newProduct = {
                ID: productID, Name: ocProduct.Product.Name,
                Variants: ocProduct.Variants, SpecCombo: '',
                MinQty: null, MaxQty: null, Static: false
            };
            const productInKit = { ID: productID, MinQty: null, MaxQty: null, Static: false, Variants: ocProduct.Variants, SpecCombo: '' };
            if (!this.productsIncluded.includes(newProduct)) this.productsIncluded.push(newProduct);
            updatedAssignments.push(productInKit);
        });
        const updatedProduct = { field: 'ProductAssignments.ProductsInKit', value: updatedAssignments };
        this.updateProductResource(updatedProduct);
    }

    // this helper function is to ensure that each async api call is completed before continuing
    async asyncForEach(array, cb) {
        for (let i = 0; i < array.length; i++) {
            await cb(array[i], i, array);
        }
    }

    handleDeleteAssignment(product: any): void {
        const updatedAssignments = this.kitProductEditable.ProductAssignments.ProductsInKit;
        for (let i = 0; i < updatedAssignments.length; i++) {
            if (updatedAssignments[i].ID === product.ID) { updatedAssignments.splice(i, 1); }
            if (this.productsIncluded[i]?.ID === product.ID) { this.productsIncluded.splice(i, 1); }
        }
        const updatedProduct = {
            field: 'ProductAssignments.ProductsInKit',
            value: updatedAssignments
        }
        this.updateProductResource(updatedProduct);
    }

    async searchedResources(searchText: any) {
        this.searchTermInput = searchText;
        this.productList = await this.getProductList({ search: searchText });
    }

    setProductConfigurability(event: any, product) {
        product.Static = event.target.checked;
    }

    isProductInKit(productID: string): boolean {
        for (let i = 0; i < this.productsIncluded.length; i++) {
            if (productID === this.productsIncluded[i].ID) return true;
        } return false;
    }

    selectProductsToAdd(event: any, productID: string): void {
        if (event.target.checked && !this.isProductInKit(productID)) {
            this.productsToAdd.push(productID);
        } else if (!event.target.checked) {
            for (let i = 0; i < this.productsToAdd.length; i++) {
                if (productID === this.productsToAdd[i]) this.productsToAdd.splice(i, 1);
                debugger;
            }
        }
    }

    openProductList(content): void {
        this.modalService.open(content, { ariaLabelledBy: 'product-list' })
    }

    /*********************************************
    **** PRODUCT IMAGE / DOC UPLOAD FUNCTIONS ****
    *********************************************/

    manualFileUpload(event, fileType: string): void {
        if (fileType === 'image') {
            const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
                const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
                return { File: file, URL };
            });
            this.stageImages(files);
        } else if (fileType === 'staticContent') {

            const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
                const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
                return { File: file, URL, Filename: this.documentName };
            });
            this.documentName = null;
            this.stageDocuments(files);
        }
    }
    stageDocuments(files: FileHandle[]): void {
        files.forEach(file => {
            const fileName = file.File.name.split('.');
            const ext = fileName[1];
            const fileNameWithExt = file.Filename + '.' + ext;
            file.Filename = fileNameWithExt;
        });
        this.staticContentFiles = this.staticContentFiles.concat(files);
        this.checkForChanges();
    }
    stageImages(files: FileHandle[]): void {
        this.imageFiles = this.imageFiles.concat(files);
        this.checkForChanges();
    }
    async addDocuments(files: FileHandle[], productID: string): Promise<void> {
        let superProduct;
        for (const file of files) {
            superProduct = await this.uploadAsset(productID, file, 'Attachment');
        }
        this.staticContentFiles = [];
        // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
        superProduct = Object.assign(this.kitProductStatic || {}, superProduct);
        this.refreshProductData(superProduct);
    }
    async addImages(files: FileHandle[], productID: string): Promise<void> {
        let superProduct;
        for (const file of files) {
            superProduct = await this.uploadAsset(productID, file, 'Image');
        }
        this.imageFiles = []
        // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
        superProduct = Object.assign(this.kitProductStatic || {}, superProduct);
        this.refreshProductData(superProduct);
    }
    async uploadAsset(productID: string, file: FileHandle, assetType: string): Promise<MarketplaceKitProduct> {
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        const asset: AssetUpload = {
            Active: true,
            File: file.File,
            Type: (assetType as AssetUpload['Type']),
            FileName: file.Filename
        }
        const newAsset: Asset = await HeadStartSDK.Upload.UploadAsset(asset, accessToken);
        await HeadStartSDK.Assets.SaveAssetAssignment({ ResourceType: 'Products', ResourceID: productID, AssetID: newAsset.ID }, accessToken);
        return await this.middlewareKitService.Get(productID);
    }
    async removeFile(file: Asset): Promise<void> {
        const accessToken = await this.appAuthService.fetchToken().toPromise();
        await HeadStartSDK.Assets.Delete(file.ID, accessToken);
        if (file.Type === 'Image') {
            this.kitProductStatic.Images = this.kitProductStatic.Images.filter(i => i.ID !== file.ID);
        } else {
            this.kitProductStatic.Attachments = this.kitProductStatic.Attachments.filter(a => a.ID !== file.ID);
        }
        this.refreshProductData(this.kitProductStatic);
    }
    unstageFile(index: number, fileType: string): void {
        if (fileType === 'image') {
            this.imageFiles.splice(index, 1)
        } else {
            this.staticContentFiles.splice(index, 1);
        }
        this.checkForChanges();
    }
    getDocumentName(event: KeyboardEvent): void {
        this.documentName = (event.target as HTMLInputElement).value;
    }

    openConfirm(content): void {
        this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
    }
}
