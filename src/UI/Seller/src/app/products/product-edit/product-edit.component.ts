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
  MarketPlaceProduct,
  MarketPlaceProductImage,
  MarketPlaceProductTaxCode,
} from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { Router } from '@angular/router';
import { Product } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';
import { faTrash, faTimes } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ListResource } from '@app-seller/shared/services/resource-crud/resource-crud.types';
import { ToastrService } from 'ngx-toastr';
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
  @Input() dataIsSaving = false;

  userContext = {};
  hasVariations = false;
  images: MarketPlaceProductImage[] = [];
  files: FileHandle[] = [];
  faTrash = faTrash;
  faTimes = faTimes;
  _marketPlaceProductStatic: MarketPlaceProduct;
  _marketPlaceProductEditable: MarketPlaceProduct;
  areChanges = false;
  taxCodeCategorySelected = false;
  taxCodes: ListResource<MarketPlaceProductTaxCode>;

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
  ) { }

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

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await this.productService.getMarketPlaceProductByID(product.ID);
    this.refreshProductData(marketPlaceProduct);
  }

  async refreshProductData(product: MarketPlaceProduct) {
    this._marketPlaceProductStatic = product;
    this._marketPlaceProductEditable = product;
    if (
      this._marketPlaceProductEditable &&
      this._marketPlaceProductEditable.xp &&
      this._marketPlaceProductEditable.xp.TaxCode &&
      this._marketPlaceProductEditable.xp.TaxCode.Category
    ) {
      const taxCategory =
        this._marketPlaceProductEditable.xp.TaxCode.Category === 'FR000000'
          ? this._marketPlaceProductEditable.xp.TaxCode.Category.substr(0, 2)
          : this._marketPlaceProductEditable.xp.TaxCode.Category.substr(0, 1);
      const avalaraTaxCodes = await this.middleware.listTaxCodes(taxCategory, '', 1, 100);
      this.taxCodes = avalaraTaxCodes;
    } else {
      this.taxCodes = { Meta: {}, Items: [] };
    }
    this.createProductForm(product);
    this.images = ReplaceHostUrls(product);
    this.taxCodeCategorySelected =
      (this._marketPlaceProductEditable &&
        this._marketPlaceProductEditable.xp &&
        this._marketPlaceProductEditable.xp.TaxCode &&
        this._marketPlaceProductEditable.xp.TaxCode.Category) !== null;
    this.checkIfCreatingNew();
    this.checkForChanges();
  }

  private checkIfCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
  }

  createProductForm(marketPlaceProduct: MarketPlaceProduct) {
    this.productForm = new FormGroup({
      Name: new FormControl(marketPlaceProduct.Name, [Validators.required, Validators.maxLength(100)]),
      ID: new FormControl(marketPlaceProduct.ID),
      Description: new FormControl(marketPlaceProduct.Description, Validators.maxLength(1000)),
      Inventory: new FormControl(marketPlaceProduct.Inventory),
      QuantityMultiplier: new FormControl(marketPlaceProduct.QuantityMultiplier),
      ShipFromAddressID: new FormControl(marketPlaceProduct.ShipFromAddressID),
      ShipHeight: new FormControl(marketPlaceProduct.ShipHeight, Validators.required),
      ShipWidth: new FormControl(marketPlaceProduct.ShipWidth, Validators.required),
      ShipLength: new FormControl(marketPlaceProduct.ShipLength, Validators.required),
      ShipWeight: new FormControl(marketPlaceProduct.ShipWeight, Validators.required),
      Price: new FormControl(_get(marketPlaceProduct, 'PriceSchedule.PriceBreaks[0].Price', null)),
      Note: new FormControl(_get(marketPlaceProduct, 'xp.Note'), Validators.maxLength(140)),
      // SpecCount: new FormControl(marketPlaceProduct.SpecCount),
      // VariantCount: new FormControl(marketPlaceProduct.VariantCount),
      TaxCodeCategory: new FormControl(_get(marketPlaceProduct, 'xp.TaxCode.Category', null)),
      TaxCode: new FormControl(_get(marketPlaceProduct, 'xp.TaxCode.Code', null)),
      xp: new FormControl(marketPlaceProduct.xp),
    });
  }

  async handleSave() {
    if (this.isCreatingNew) {
      try {
        await this.createNewProduct();
      } catch {
        this.toasterService.error(`A product with that ID already exists`);
      }
    } else {
      this.updateProduct();
    }
  }

  async handleDelete($event): Promise<void> {
    await this.ocProductService.Delete(this._marketPlaceProductStatic.ID).toPromise();
    this.router.navigateByUrl('/products');
  }

  handleDiscardChanges(): void {
    this.files = [];
    this._marketPlaceProductEditable = this._marketPlaceProductStatic;
    this.refreshProductData(this._marketPlaceProductStatic);
  }

  async createNewProduct() {
    this.dataIsSaving = true;
    const product = await this.productService.createNewMarketPlaceProduct(this._marketPlaceProductEditable);
    await this.addFiles(this.files, product.ID);
    this.refreshProductData(product);
    this.router.navigateByUrl(`/products/${product.ID}`);
    this.dataIsSaving = false;
  }

  async updateProduct() {
    this.dataIsSaving = true;
    const product = await this.productService.updateMarketPlaceProduct(this._marketPlaceProductEditable);
    this._marketPlaceProductStatic = product;
    this._marketPlaceProductEditable = product;
    if (this.files) this.addFiles(this.files, product.ID);
    this.dataIsSaving = false;
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
      this._marketPlaceProductEditable || this.productService.emptyResource
    );
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
    this._marketPlaceProductEditable = updateProductResourceCopy;
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
    this._marketPlaceProductEditable = { ...this._marketPlaceProductEditable, [field]: value };
    this.checkForChanges();
  }

  checkForChanges(): void {
    this.areChanges =
      JSON.stringify(this._marketPlaceProductEditable) !== JSON.stringify(this._marketPlaceProductStatic) ||
      this.files.length > 0;
  }

  /******************************************
   *  **** PRODUCT IMAGE UPLOAD FUNCTIONS ****
   * ******************************************/

  manualFileUpload(event): void {
    const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
      const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
      return { File: file, URL: URL };
    });
    this.stageFiles(files);
  }

  stageFiles(files: FileHandle[]) {
    this.files = files;
    this.checkForChanges();
  }

  async addFiles(files: FileHandle[], productID: string) {
    let product;
    for (const file of files) {
      product = await this.middleware.uploadProductImage(file.File, productID);
    }
    this.files = [];
    // Only need the `|| {}` to account for creating new product where this._marketPlaceProductStatic doesn't exist yet.
    product = Object.assign(this._marketPlaceProductStatic || {}, product);
    this.refreshProductData(product);
  }

  async removeFile(imgUrl: string) {
    let product = await this.middleware.deleteProductImage(this._marketPlaceProductStatic.ID, imgUrl);
    product = Object.assign(this._marketPlaceProductStatic, product);
    this.refreshProductData(product);
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
      this._marketPlaceProductEditable &&
      this._marketPlaceProductEditable.xp &&
      !this._marketPlaceProductEditable.xp.TaxCode
    ) {
      this._marketPlaceProductEditable.xp.TaxCode = { Category: '', Code: '', Description: '' };
    }
    this.handleUpdateProduct(event, 'xp.TaxCode.Category');
    const avalaraTaxCodes = await this.middleware.listTaxCodes(event.target.value, '', 1, 100);
    this.taxCodes = avalaraTaxCodes;
  }

  async searchTaxCodes(searchTerm: string) {
    if (searchTerm === undefined) searchTerm = '';
    const taxCodeCategory = this._marketPlaceProductEditable.xp.TaxCode.Category;
    const avalaraTaxCodes = await this.middleware.listTaxCodes(taxCodeCategory, searchTerm, 1, 100);
    this.taxCodes = avalaraTaxCodes;
  }

  async handleScrollEnd(searchTerm: string) {
    if (searchTerm === undefined) searchTerm = '';
    const totalPages = this.taxCodes.Meta.TotalPages;
    const nextPageNumber = this.taxCodes.Meta.Page + 1;
    if (totalPages > nextPageNumber) {
      const taxCodeCategory = this._marketPlaceProductEditable.xp.TaxCode.Category;
      const avalaraTaxCodes = await this.middleware.listTaxCodes(taxCodeCategory, searchTerm, nextPageNumber, 100);
      this.taxCodes = {
        Meta: avalaraTaxCodes.Meta,
        Items: [...this.taxCodes.Items, ...avalaraTaxCodes.Items],
      };
      this.changeDetectorRef.detectChanges();
    }
  }
}
