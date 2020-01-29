import { Component, Input, Output, EventEmitter, OnInit, Inject } from '@angular/core';
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
import { MarketPlaceProduct, MarketPlaceProductImage } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { Router } from '@angular/router';
import { Product } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';
import { faTrash, faTimes } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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
    if (Object.keys(product).length) {
      this.handleSelectedProductChange(product);
    } else {
      this.createProductForm({});
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

  userContext = {};
  hasVariations = false;
  images: MarketPlaceProductImage[] = [];
  files: FileHandle[] = [];
  faTrash = faTrash;
  faTimes = faTimes;
  _marketPlaceProductStatic: MarketPlaceProduct;
  _marketPlaceProductEditable: MarketPlaceProduct;
  areChanges = false;
  dataSaved = false;

  constructor(
    private router: Router,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocProductService: OcProductService,
    private ocAdminAddressService: OcAdminAddressService,
    private productService: ProductService,
    private middleware: MiddlewareAPIService,
    private sanitizer: DomSanitizer,
    private modalService: NgbModal,
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

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await this.productService.getMarketPlaceProductByID(product.ID);
    this.refreshProductData(marketPlaceProduct);
  }

  refreshProductData(product: MarketPlaceProduct) {
    this._marketPlaceProductStatic = product;
    this._marketPlaceProductEditable = product;
    this.createProductForm(product);
    this.images = ReplaceHostUrls(product);
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
      Description: new FormControl(marketPlaceProduct.Description, Validators.maxLength(1000)),
      Inventory: new FormControl(marketPlaceProduct.Inventory),
      QuantityMultiplier: new FormControl(marketPlaceProduct.QuantityMultiplier),
      ShipFromAddressID: new FormControl(marketPlaceProduct.ShipFromAddressID),
      ShipHeight: new FormControl(marketPlaceProduct.ShipHeight, Validators.required),
      ShipWidth: new FormControl(marketPlaceProduct.ShipWidth, Validators.required),
      ShipLength: new FormControl(marketPlaceProduct.ShipLength, Validators.required),
      ShipWeight: new FormControl(marketPlaceProduct.ShipWeight, Validators.required),
      Price: new FormControl(_get(marketPlaceProduct, 'PriceSchedule.PriceBreaks[0].Price', null)),
      // SpecCount: new FormControl(marketPlaceProduct.SpecCount),
      // VariantCount: new FormControl(marketPlaceProduct.VariantCount),
      TaxCode: new FormControl(_get(marketPlaceProduct, 'xp.TaxCode.Code', null)),
      xp: new FormControl(marketPlaceProduct.xp),
    });
  }

  handleSave() {
    if (this.isCreatingNew) {
      this.createNewProduct();
      this.dataSaved = true;
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
    const product = await this.productService.createNewMarketPlaceProduct(this._marketPlaceProductEditable);
    await this.addFiles(this.files, product.ID);
    this.refreshProductData(product);
    this.router.navigateByUrl(`/products/${product.ID}`);
  }

  async updateProduct() {
    const product = await this.productService.updateMarketPlaceProduct(this._marketPlaceProductEditable);
    if (this.files) this.addFiles(this.files, product.ID);
  }

  updateResourceFromEvent(event: any, field: string): void {
    if (field === 'Price') {
      // placeholder for just handling a single price
      this._marketPlaceProductEditable = {
        ...this._marketPlaceProductEditable,

        // this will overwrite all existing price breaks with the price
        // when more robust price setting is creating this should be changed
        PriceSchedule: {
          ...this._marketPlaceProductEditable.PriceSchedule,
          PriceBreaks: [{ Quantity: 1, Price: Number(event.target.value) }],
        },
      };
      this.checkForChanges();
    } else if (field === 'TaxCode') {
      this._marketPlaceProductEditable = {
        ...this._marketPlaceProductEditable,
        xp: {
          ...this._marketPlaceProductEditable.xp,
          TaxCode: {
            Name: event.target.options[event.target.selectedIndex].text,
            Code: event.target.value,
          },
        },
      };
      this.checkForChanges();
    } else {
      this.updateResourceFromFieldValue(field, event.target.value);
      // this._marketPlaceProductEditable = { ...this._marketPlaceProductEditable, [field]: event.target.value };
    }
  }

  updateResourceFromFieldValue(field: string, value: any) {
    if (
      field === 'QuantityMultiplier' ||
      field === 'ShipHeight' ||
      field === 'ShipWidth' ||
      field === 'ShipLength' ||
      field === 'ShipWeight'
    )
      value = Number(value);
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
      const Url = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
      return { File: file, Url: Url };
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
}
