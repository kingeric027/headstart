import { Component, Input, Output, EventEmitter, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { get as _get } from 'lodash';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { UserContext } from '@app-seller/config/user-context';
import {
  ListAddress,
  OcSupplierAddressService,
  OcAdminAddressService,
  OcProductService,
} from '@ordercloud/angular-sdk';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Product } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { DomSanitizer } from '@angular/platform-browser';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { faTrash, faTimes } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '@app-seller/products/product.service';
import { ReplaceHostUrls } from '@app-seller/products/product-image.helper';
import { ProductImage, SuperMarketplaceProduct, ListPage, MarketplaceSDK } from 'marketplace-javascript-sdk';
import TaxCodes from 'marketplace-javascript-sdk/dist/api/TaxCodes';
import { ValidateMinMax } from '@app-seller/validators/validators';
import { ProductStaticContent } from 'marketplace-javascript-sdk/dist/models/ProductStaticContent';

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss'],
})
export class ProductEditComponent implements OnInit {
  @Input()
  productForm: FormGroup;
  @Input()
  set orderCloudProduct(product: Product) {
    if (product.ID) {
      this.handleSelectedProductChange(product);
    } else {
      this.createProductForm(this.productService.emptyResource);
    }
  }
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  @Input()
  addresses: ListAddress;
  @Input()
  isCreatingNew: boolean;
  @Input()
  dataIsSaving = false;

  userContext = {};
  hasVariations = false;
  images: ProductImage[] = [];
  files: FileHandle[] = [];
  faTrash = faTrash;
  faTimes = faTimes;
  _superMarketplaceProductStatic: SuperMarketplaceProduct;
  _superMarketplaceProductEditable: SuperMarketplaceProduct;
  areChanges = false;
  taxCodeCategorySelected = false;
  taxCodes: ListPage<TaxCodes>;
  productType: string;
  fileType: string;
  imageFiles: FileHandle[] = [];
  staticContentFiles: FileHandle[] = [];
  staticContent: ProductStaticContent[] = [];
  documentName: string;
  url: string;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private router: Router,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocProductService: OcProductService,
    private ocAdminAddressService: OcAdminAddressService,
    private productService: ProductService,
    private sanitizer: DomSanitizer,
    private modalService: NgbModal,
    private middleware: MiddlewareAPIService,
    private toasterService: ToastrService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) { }

  async ngOnInit() {
    // TODO: Eventually move to a resolve so that they are there before the component instantiates.
    this.isCreatingNew = this.productService.checkIfCreatingNew();
    this.getAddresses();
    this.userContext = await this.currentUserService.getUserContext();
  }

  async getAddresses(): Promise<void> {
    const context: UserContext = await this.currentUserService.getUserContext();
    context.Me.Supplier
      ? (this.addresses = await this.ocSupplierAddressService.List(context.Me.Supplier.ID).toPromise())
      : (this.addresses = await this.ocAdminAddressService.List().toPromise());
  }

  async refreshProductData(superProduct: SuperMarketplaceProduct) {
    this._superMarketplaceProductStatic = superProduct;
    this._superMarketplaceProductEditable = superProduct;
    if (
      this._superMarketplaceProductEditable.Product?.xp?.Tax?.Category
    ) {
      const taxCategory =
        this._superMarketplaceProductEditable.Product.xp.Tax.Category === 'FR000000'
          ? this._superMarketplaceProductEditable.Product.xp.Tax.Category.substr(0, 2)
          : this._superMarketplaceProductEditable.Product.xp.Tax.Category.substr(0, 1);
      const avalaraTaxCodes = await this.listTaxCodes(taxCategory, '', 1, 100);
      this.taxCodes = avalaraTaxCodes;
    } else {
      this.taxCodes = { Meta: {}, Items: [] };
    }
    this.productType = superProduct.Product?.xp?.ProductType;
    this.staticContent = superProduct.Product?.xp?.StaticContent;
    this.createProductForm(superProduct);
    console.log(superProduct)

    this.images = ReplaceHostUrls(superProduct.Product);
    this.taxCodeCategorySelected = this._superMarketplaceProductEditable.Product?.xp?.Tax?.Category !== null;
    this.isCreatingNew = this.productService.checkIfCreatingNew();
    this.staticContent.forEach(file => this.sanitizer.bypassSecurityTrustResourceUrl(file.URL))
    this.checkForChanges();
  }

  createProductForm(superMarketplaceProduct: SuperMarketplaceProduct) {
    if (superMarketplaceProduct.Product) {
      this.productForm = new FormGroup({
        Active: new FormControl(superMarketplaceProduct.Product.Active),
        Name: new FormControl(superMarketplaceProduct.Product.Name, [Validators.required, Validators.maxLength(100)]),
        ID: new FormControl(superMarketplaceProduct.Product.ID),
        Description: new FormControl(superMarketplaceProduct.Product.Description, Validators.maxLength(1000)),
        Inventory: new FormControl(superMarketplaceProduct.Product.Inventory),
        QuantityMultiplier: new FormControl(superMarketplaceProduct.Product.QuantityMultiplier),
        ShipFromAddressID: new FormControl(superMarketplaceProduct.Product.ShipFromAddressID),
        ShipHeight: new FormControl(superMarketplaceProduct.Product.ShipHeight, [Validators.required, Validators.min(0)]),
        ShipWidth: new FormControl(superMarketplaceProduct.Product.ShipWidth, [Validators.required, Validators.min(0)]),
        ShipLength: new FormControl(superMarketplaceProduct.Product.ShipLength, [Validators.required, Validators.min(0)]),
        ShipWeight: new FormControl(superMarketplaceProduct.Product.ShipWeight, [Validators.required, Validators.min(0)]),
        Price: new FormControl(_get(superMarketplaceProduct.PriceSchedule, 'PriceBreaks[0].Price', null)),
        MinQuantity: new FormControl(superMarketplaceProduct.PriceSchedule.MinQuantity, Validators.min(1)),
        MaxQuantity: new FormControl(superMarketplaceProduct.PriceSchedule.MaxQuantity, Validators.min(1)),
        Note: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Note'), Validators.maxLength(140)),
        ProductType: new FormControl(_get(superMarketplaceProduct.Product, 'xp.ProductType')),
        // SpecCount: new FormControl(superMarketplaceProduct.SpecCount),
        // VariantCount: new FormControl(superMarketplaceProduct.VariantCount),
        TaxCodeCategory: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Category', null)),
        TaxCode: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Code', null)),
      }, { validators: ValidateMinMax }
      );
    }
  }

  async handleSave() {
    if (this.isCreatingNew) {
      await this.createNewProduct();
    } else {
      this.updateProduct();
    }
  }

  async handleDelete($event): Promise<void> {
    await this.ocProductService.Delete(this._superMarketplaceProductStatic.Product.ID).toPromise();
    this.router.navigateByUrl('/products');
  }

  handleDiscardChanges(): void {
    this.imageFiles = [];
    this.staticContentFiles = [];
    this._superMarketplaceProductEditable = this._superMarketplaceProductStatic;
    this.refreshProductData(this._superMarketplaceProductStatic);
  }

  async createNewProduct() {
    try {
      this.dataIsSaving = true;
      const superProduct = await this.createNewSuperMarketplaceProduct(this._superMarketplaceProductEditable);
      await this.addFiles(this.files, superProduct.Product.ID);
      this.refreshProductData(superProduct);
      this.router.navigateByUrl(`/products/${superProduct.Product.ID}`);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async updateProduct() {
    try {
      this.dataIsSaving = true;
      const superProduct = await this.updateMarketplaceProduct(this._superMarketplaceProductEditable);
      this._superMarketplaceProductStatic = superProduct;
      this._superMarketplaceProductEditable = superProduct;
      if (this.imageFiles) this.addFiles(this.imageFiles, superProduct.Product.ID, "image");
      if (this.staticContentFiles) {
        this.addFiles(this.staticContentFiles, superProduct.Product.ID, "staticContent");
      }
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  updateProductResource(productUpdate: any) {
    const resourceToUpdate = this._superMarketplaceProductEditable || this.productService.emptyResource;
    this._superMarketplaceProductEditable = this.productService.getUpdatedEditableResource(productUpdate, resourceToUpdate);
    this.checkForChanges();
  }

  handleUpdateProduct(event: any, field: string, typeOfValue?: string) {
    const productUpdate = {
      field,
      value:
        field === 'Product.Active'
          ? event.target.checked
          : typeOfValue === 'number'
            ? Number(event.target.value)
            : event.target.value,
    };
    this.updateProductResource(productUpdate);
  }

  // Used only for Product.Description coming out of quill editor (no 'event.target'.)
  updateResourceFromFieldValue(field: string, value: any) {
    const updateProductResourceCopy = this.productService.copyResource(
      this._superMarketplaceProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Product = {
      ...updateProductResourceCopy.Product,
      [field]: value,
    };
    this._superMarketplaceProductEditable = updateProductResourceCopy;
    this.checkForChanges();
  }
  // TODO: Remove duplicate function, function exists in resource-crud.component.ts (minus the files check);
  checkForChanges(): void {
    this.areChanges =
      JSON.stringify(this._superMarketplaceProductEditable) !== JSON.stringify(this._superMarketplaceProductStatic) ||
      this.imageFiles.length > 0 || this.staticContentFiles.length > 0;
  }

  /** ****************************************
   *  **** PRODUCT IMAGE UPLOAD FUNCTIONS ****
   * ******************************************/

  manualFileUpload(event, fileType: string): void {
    if (fileType === "image") {
      const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
        const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
        return { File: file, URL };
      });
      this.stageImages(files);
    } else if (fileType === "staticContent") {
      const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
        const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
        return { File: file, URL, fileName: this.documentName };
      });
      this.stageDocuments(files);
    }
  }

  stageImages(files: FileHandle[]) {
    this.imageFiles = this.imageFiles.concat(files);
    this.checkForChanges();
    console.log('staging img', this.imageFiles)
  }

  async addFiles(files: FileHandle[], productID: string, fileType?: string) {
    let superProduct;
    if (fileType === "image") {
      for (const file of files) {
        superProduct = await this.middleware.uploadProductImage(file.File, productID);
      }
    } else {
      this.staticContentFiles.forEach(async file => {
        superProduct = await this.middleware.uploadStaticContent(file.File, productID, file.fileName);
      });
    }
    fileType === "image" ?
      this.imageFiles = [] :
      this.staticContentFiles = [];
    // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
    superProduct = Object.assign(this._superMarketplaceProductStatic || {}, superProduct);
    this.refreshProductData(superProduct);
  }

  async removeFile(imgUrl: string) {
    const prodID = this._superMarketplaceProductStatic.Product.ID;
    const imageName = imgUrl.split('/').slice(-1)[0];
    let superProduct = await MarketplaceSDK.Files.Delete(this.appConfig.marketplaceID, prodID, imageName);
    superProduct = Object.assign(this._superMarketplaceProductStatic, superProduct);
    this.refreshProductData(superProduct);
  }

  unstageImage(index: number) {
    this.imageFiles.splice(index, 1)
    this.checkForChanges();
  }

  /** ****************************************
   *  *** PRODUCT DOCUMENT UPLOAD FUNCTIONS ***
   * ******************************************/

  getDocumentUrl(url: string) {
    let spliturl = url.split('/')
    spliturl.splice(4, 1);
    let str = spliturl.join('/')
    return str;
  }

  getDocumentName(event: KeyboardEvent) {
    this.documentName = (event.target as HTMLInputElement).value;
  }

  stageDocuments(files: FileHandle[]) {
    this.staticContentFiles = this.staticContentFiles.concat(files);
    this.checkForChanges();
  }

  unstageDocument(index) {
    this.staticContentFiles.splice(index, 1);
    this.checkForChanges();
  }

  async removeDocument(file: ProductStaticContent) {
    //const prodID = this._superMarketplaceProductStatic.Product.ID;
    let superProduct = await this.middleware.deleteStaticContent(file.URL);
    superProduct = Object.assign(this._superMarketplaceProductStatic, superProduct);
    this.refreshProductData(superProduct);
  }

  async open(content) {
    await this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
  }

  async handleTaxCodeCategorySelection(event): Promise<void> {
    // TODO: This is a temporary fix to accomodate for data not having xp.TaxCode yet
    if (
      this._superMarketplaceProductEditable?.Product?.xp &&
      !this._superMarketplaceProductEditable.Product.xp.Tax
    ) {
      this._superMarketplaceProductEditable.Product.xp.Tax = { Category: '', Code: '', Description: '' };
    }
    this.resetTaxCodeAndDescription();
    this.handleUpdateProduct(event, 'Product.xp.Tax.Category');
    this._superMarketplaceProductEditable.Product.xp.Tax.Code = '';
    const avalaraTaxCodes = await this.listTaxCodes(event.target.value, '', 1, 100);
    this.taxCodes = avalaraTaxCodes;
  }
  // Reset TaxCode Code and Description if a new TaxCode Category is selected
  resetTaxCodeAndDescription(): void {
    this.handleUpdateProduct({ target: { value: null } }, 'Product.xp.Tax.Code');
    this.handleUpdateProduct({ target: { value: null } }, 'Product.xp.Tax.Description');
  }

  async searchTaxCodes(searchTerm: string) {
    if (searchTerm === undefined) searchTerm = '';
    const taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
    const avalaraTaxCodes = await this.listTaxCodes(taxCodeCategory, searchTerm, 1, 100);
    this.taxCodes = avalaraTaxCodes;
  }

  async handleScrollEnd(searchTerm: string) {
    if (searchTerm === undefined) searchTerm = '';
    const totalPages = this.taxCodes.Meta.TotalPages;
    const nextPageNumber = this.taxCodes.Meta.Page + 1;
    if (totalPages > nextPageNumber) {
      const taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
      const avalaraTaxCodes = await this.listTaxCodes(taxCodeCategory, searchTerm, nextPageNumber, 100);
      this.taxCodes = {
        Meta: avalaraTaxCodes.Meta,
        Items: [...this.taxCodes.Items, ...avalaraTaxCodes.Items],
      };
      this.changeDetectorRef.detectChanges();
    }
  }

  getSaveBtnText(): string {
    return this.productService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
  }

  private async createNewSuperMarketplaceProduct(
    superMarketplaceProduct: SuperMarketplaceProduct
  ): Promise<SuperMarketplaceProduct> {
    superMarketplaceProduct.Product.xp.Status = 'Draft';
    superMarketplaceProduct.PriceSchedule.Name = `Default_Marketplace_Buyer${superMarketplaceProduct.Product.Name}`;
    return await MarketplaceSDK.Products.Post(superMarketplaceProduct);
  }

  private async updateMarketplaceProduct(superMarketplaceProduct: SuperMarketplaceProduct): Promise<SuperMarketplaceProduct> {
    // TODO: Temporary while Product set doesn't reflect the current strongly typed Xp
    superMarketplaceProduct.Product.xp.Status = 'Draft';
    return await MarketplaceSDK.Products.Put(superMarketplaceProduct.Product.ID, superMarketplaceProduct);
  };

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await MarketplaceSDK.Products.Get(product.ID);
    this.refreshProductData(marketPlaceProduct);
  }

  private async listTaxCodes(taxCategory, search, page, pageSize): Promise<any> {
    return await MarketplaceSDK.TaxCodes.GetTaxCodes({ filters: { Category: taxCategory }, search, page, pageSize });
  }
}
