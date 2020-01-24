import { Component, Input, Output, EventEmitter, OnInit, Inject } from '@angular/core';
import { get as _get } from 'lodash';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { UserContext } from '@app-seller/config/user-context';
import { ListAddress, OcSupplierAddressService, OcAdminAddressService } from '@ordercloud/angular-sdk';
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
  files: FileHandle[];
  faTrash = faTrash;
  faTimes = faTimes;
  _marketPlaceProduct: MarketPlaceProduct;
  _marketPlaceProductUpdated: MarketPlaceProduct;

  constructor(
    private router: Router,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
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
    this._marketPlaceProduct = product;
    this._marketPlaceProductUpdated = product;
    this.createProductForm(product);
    this.images = ReplaceHostUrls(product);
    this.checkIfCreatingNew();
  }

  private checkIfCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
  }

  createProductForm(marketPlaceProduct: MarketPlaceProduct) {
    this.productForm = new FormGroup({
      Name: new FormControl(marketPlaceProduct.Name, Validators.required),
      Description: new FormControl(marketPlaceProduct.Description),
      Inventory: new FormControl(marketPlaceProduct.Inventory),
      QuantityMultiplier: new FormControl(marketPlaceProduct.QuantityMultiplier),
      ShipFromAddressID: new FormControl(marketPlaceProduct.ShipFromAddressID),
      ShipHeight: new FormControl(marketPlaceProduct.ShipHeight),
      ShipWidth: new FormControl(marketPlaceProduct.ShipWidth),
      ShipLength: new FormControl(marketPlaceProduct.ShipLength),
      ShipWeight: new FormControl(marketPlaceProduct.ShipWeight),
      Price: new FormControl(_get(marketPlaceProduct, 'PriceSchedule.PriceBreaks[0].Price', null)),
      // SpecCount: new FormControl(marketPlaceProduct.SpecCount),
      // VariantCount: new FormControl(marketPlaceProduct.VariantCount),
      xp: new FormControl(marketPlaceProduct.xp),
    });
  }

  handleSave() {
    if (this.isCreatingNew) {
      this.createNewProduct();
    } else {
      this.updateProduct();
    }
  }

  async createNewProduct() {
    const product = await this.productService.createNewMarketPlaceProduct(this._marketPlaceProductUpdated);
    await this.addFiles(this.files, product.ID);
    this.refreshProductData(product);
    this.router.navigateByUrl(`/products/${product.ID}`);
  }

  async updateProduct() {
    const product = await this.productService.updateMarketPlaceProduct(this._marketPlaceProductUpdated);
    this.addFiles(this.files, product.ID);
  }

  updateResourceFromEvent(event: any, field: string): void {
    if (field === 'Price') {
      // placeholder for just handling a single price
      this._marketPlaceProductUpdated = {
        ...this._marketPlaceProductUpdated,

        // this will overwrite all existing price breaks with the price
        // when more robust price setting is creating this should be changed
        PriceSchedule: {
          ...this._marketPlaceProductUpdated,
          PriceBreaks: [{ Price: event.target.value, Quantity: 1 }],
        },
      };
    } else {
      this.updateResourceFromFieldValue(field, event.target.value);
      // this._marketPlaceProductUpdated = { ...this._marketPlaceProductUpdated, [field]: event.target.value };
    }
  }

  updateResourceFromFieldValue(field: string, value: any) {
    this._marketPlaceProductUpdated = { ...this._marketPlaceProductUpdated, [field]: value };
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
  }

  async addFiles(files: FileHandle[], productID: string) {
    let product;
    for (const file of files) {
      product = await this.middleware.uploadProductImage(file.File, productID);
    }
    this.files = [];
    // Only need the `|| {}` to account for creating new product where this._marketPlaceProduct doesn't exist yet.
    product = Object.assign(this._marketPlaceProduct || {}, product);
    this.refreshProductData(product);
  }

  async removeFile(imgUrl: string) {
    let product = await this.middleware.deleteProductImage(this._marketPlaceProduct.ID, imgUrl);
    product = Object.assign(this._marketPlaceProduct, product);
    this.refreshProductData(product);
  }

  unStage(index: number) {
    this.files.splice(index, 1);
  }

  async open(content) {
    await this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
  }
}
