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
import {
  MarketPlaceProductImage,
  MarketPlaceProductTaxCode,
  SuperMarketplaceProduct,
} from '@app-seller/shared/models/MarketPlaceProduct.interface';
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
import { ListPage } from '@app-seller/shared/services/middleware-api/listPage.interface';
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
  images: MarketPlaceProductImage[] = [];
  files: FileHandle[] = [];
  faTrash = faTrash;
  faTimes = faTimes;
  _superMarketplaceProductStatic: SuperMarketplaceProduct;
  _superMarketplaceProductEditable: SuperMarketplaceProduct;
  areChanges = false;
  taxCodeCategorySelected = false;
  taxCodes: ListPage<MarketPlaceProductTaxCode>;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private router: Router,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocProductService: OcProductService,
    private ocAdminAddressService: OcAdminAddressService,
    private productService: ProductService,
    private middleware: MiddlewareAPIService,
    private sanitizer: DomSanitizer,
    private modalService: NgbModal,
    private toasterService: ToastrService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  async ngOnInit() {
    // TODO: Eventually move to a resolve so that they are there before the component instantiates.
    this.checkIfCreatingNew();
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
      this._superMarketplaceProductEditable.Product &&
      this._superMarketplaceProductEditable.Product.xp &&
      this._superMarketplaceProductEditable.Product.xp.Tax &&
      this._superMarketplaceProductEditable.Product.xp.Tax.Category
    ) {
      const taxCategory =
        this._superMarketplaceProductEditable.Product.xp.Tax.Category === 'FR000000'
          ? this._superMarketplaceProductEditable.Product.xp.Tax.Category.substr(0, 2)
          : this._superMarketplaceProductEditable.Product.xp.Tax.Category.substr(0, 1);
      const avalaraTaxCodes = await this.middleware.listTaxCodes(taxCategory, '', 1, 100);
      this.taxCodes = avalaraTaxCodes;
    } else {
      this.taxCodes = { Meta: {}, Items: [] };
    }
    this.createProductForm(superProduct);
    this.images = ReplaceHostUrls(superProduct.Product);
    this.taxCodeCategorySelected =
      (this._superMarketplaceProductEditable.Product &&
        this._superMarketplaceProductEditable.Product.xp &&
        this._superMarketplaceProductEditable.Product.xp.Tax &&
        this._superMarketplaceProductEditable.Product.xp.Tax.Category) !== null;
    this.checkIfCreatingNew();
    this.checkForChanges();
  }

  createProductForm(superMarketplaceProduct: SuperMarketplaceProduct) {
    this.productForm = new FormGroup({
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
      Note: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Note'), Validators.maxLength(140)),
      // SpecCount: new FormControl(superMarketplaceProduct.SpecCount),
      // VariantCount: new FormControl(superMarketplaceProduct.VariantCount),
      TaxCodeCategory: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Category', null)),
      TaxCode: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Code', null)),
    });
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
    this.files = [];
    this._superMarketplaceProductEditable = this._superMarketplaceProductStatic;
    this.refreshProductData(this._superMarketplaceProductStatic);
  }

  async createNewProduct() {
    try {
      this.dataIsSaving = true;
      const superProduct = await this.middleware.createNewSuperMarketplaceProduct(
        this._superMarketplaceProductEditable
      );
      await this.addFiles(this.files, superProduct.Product.ID);
      this.refreshProductData(superProduct);
      this.router.navigateByUrl(`/products/${superProduct.Product.ID}`);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      if (ex.error && ex.error.Errors && ex.error.Errors.some(e => e.ErrorCode === 'IdExists')) {
        this.toasterService.error('A product with that ID already exists');
      } else {
        throw ex;
      }
    }
  }

  async updateProduct() {
    try {
      this.dataIsSaving = true;
      const superProduct = await this.middleware.updateMarketplaceProduct(this._superMarketplaceProductEditable);
      this._superMarketplaceProductStatic = superProduct;
      this._superMarketplaceProductEditable = superProduct;
      if (this.files) this.addFiles(this.files, superProduct.Product.ID);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  updateProductResource(productUpdate: any) {
    /* 
    * TODO:
    * This function is used to dynamically update deeply nested objects
    * It is currently used in two places, but will likely soon become
    * obsolete when the product edit component gets refactored.
    */
    const piecesOfField = productUpdate.field.split('.');
    const depthOfField = piecesOfField.length;
    const updateProductResourceCopy = this.copyProductResource(
      this._superMarketplaceProductEditable || this.productService.emptyResource
    );
    console.log(updateProductResourceCopy);
    switch (depthOfField) {
      case 4:
        updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]][piecesOfField[3]] =
          productUpdate.value;
        break;
      case 3:
        updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]] = productUpdate.value;
        break;
      case 2:
        updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]] = productUpdate.value;
        break;
      default:
        updateProductResourceCopy[piecesOfField[0]] = productUpdate.value;
        break;
    }
    this._superMarketplaceProductEditable = updateProductResourceCopy;
    this.checkForChanges();
  }

  handleUpdateProduct(event: any, field: string, typeOfValue?: string) {
    const productUpdate = {
      field,
      value:
        field === 'Active'
          ? event.target.checked
          : typeOfValue === 'number'
            ? Number(event.target.value)
            : event.target.value,
    };
    this.updateProductResource(productUpdate);
  }

  copyProductResource(product: any) {
    return JSON.parse(JSON.stringify(product));
  }

  // Used only for Product.Description coming out of quill editor (no 'event.target'.)
  updateResourceFromFieldValue(field: string, value: any) {
    const updateProductResourceCopy = this.copyProductResource(
      this._superMarketplaceProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Product = {
      ...updateProductResourceCopy.Product,
      [field]: value,
    };
    this._superMarketplaceProductEditable = updateProductResourceCopy;
    this.checkForChanges();
  }

  checkForChanges(): void {
    this.areChanges =
      JSON.stringify(this._superMarketplaceProductEditable) !== JSON.stringify(this._superMarketplaceProductStatic) ||
      this.files.length > 0;
  }

  /** ****************************************
   *  **** PRODUCT IMAGE UPLOAD FUNCTIONS ****
   * ******************************************/

  manualFileUpload(event): void {
    const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
      const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
      return { File: file, URL };
    });
    this.stageFiles(files);
  }

  stageFiles(files: FileHandle[]) {
    this.files = this.files.concat(files);
    this.checkForChanges();
  }

  async addFiles(files: FileHandle[], productID: string) {
    let superProduct;
    for (const file of files) {
      superProduct = await this.middleware.uploadProductImage(file.File, productID);
    }
    this.files = [];
    // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
    superProduct = Object.assign(this._superMarketplaceProductStatic || {}, superProduct);
    this.refreshProductData(superProduct);
  }

  async removeFile(imgUrl: string) {
    let superProduct = await this.middleware.deleteProductImage(this._superMarketplaceProductStatic.Product.ID, imgUrl);
    superProduct = Object.assign(this._superMarketplaceProductStatic, superProduct);
    this.refreshProductData(superProduct);
  }

  unStage(index: number) {
    this.files.splice(index, 1);
    this.checkForChanges();
  }

  async open(content) {
    await this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
  }

  async handleTaxCodeCategorySelection(event): Promise<void> {
    // TODO: This is a temporary fix to accomodate for data not having xp.TaxCode yet
    if (
      this._superMarketplaceProductEditable.Product &&
      this._superMarketplaceProductEditable.Product.xp &&
      !this._superMarketplaceProductEditable.Product.xp.Tax
    ) {
      this._superMarketplaceProductEditable.Product.xp.Tax = { Category: '', Code: '', Description: '' };
    }
    this.resetTaxCodeAndDescription();
    this.handleUpdateProduct(event, 'Product.xp.Tax.Category');
    this._superMarketplaceProductEditable.Product.xp.Tax.Code = '';
    const avalaraTaxCodes = await this.middleware.listTaxCodes(event.target.value, '', 1, 100);
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
    const avalaraTaxCodes = await this.middleware.listTaxCodes(taxCodeCategory, searchTerm, 1, 100);
    this.taxCodes = avalaraTaxCodes;
  }

  async handleScrollEnd(searchTerm: string) {
    if (searchTerm === undefined) searchTerm = '';
    const totalPages = this.taxCodes.Meta.TotalPages;
    const nextPageNumber = this.taxCodes.Meta.Page + 1;
    if (totalPages > nextPageNumber) {
      const taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
      const avalaraTaxCodes = await this.middleware.listTaxCodes(taxCodeCategory, searchTerm, nextPageNumber, 100);
      this.taxCodes = {
        Meta: avalaraTaxCodes.Meta,
        Items: [...this.taxCodes.Items, ...avalaraTaxCodes.Items],
      };
      this.changeDetectorRef.detectChanges();
    }
  }

  private checkIfCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
  }

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await this.middleware.getSuperMarketplaceProductByID(product.ID);
    this.refreshProductData(marketPlaceProduct);
  }
}
