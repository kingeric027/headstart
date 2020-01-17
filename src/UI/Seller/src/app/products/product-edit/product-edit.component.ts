import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { ListAddress, OcSupplierAddressService, MeUser } from '@ordercloud/angular-sdk';
import { SupplierAddressService } from '@app-seller/shared/services/supplier/supplier-address.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { Router } from '@angular/router';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { Product } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss'],
})
export class ProductEditComponent implements OnInit {
  _marketPlaceProduct: MarketPlaceProduct;
  _marketPlaceProductUpdated: MarketPlaceProduct;
  files = [];
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
  hasVariations = false;
  @Input()
  supplierAddresses: ListAddress;
  @Input()
  isCreatingNew: boolean;

  async getSupplierAddresses(): Promise<void> {
    const user: MeUser = await this.currentUserService.getUser();
    if (user.Supplier) {
      this.supplierAddresses = await this.ocSupplierAddressService.List(user.Supplier.ID).toPromise();
    }
  }

  constructor(
    private router: Router,
    private supplierAddressService: SupplierAddressService,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private productService: ProductService,
    private middleware: MiddlewareAPIService
  ) {}

  ngOnInit() {
    // TODO: Eventually move to a resolve so that they are there before the component instantiates.
    this.getSupplierAddresses();
    this.checkIfCreatingNew();
  }

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await this.productService.getMarketPlaceProductByID(product.ID);
    this._marketPlaceProduct = marketPlaceProduct;
    this._marketPlaceProductUpdated = marketPlaceProduct;
    this.createProductForm(marketPlaceProduct);
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

  createNewProduct() {
    this.productService.createNewMarketPlaceProduct(this._marketPlaceProductUpdated);
  }

  updateProduct() {
    this.productService.updateMarketPlaceProduct(this._marketPlaceProductUpdated);
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
      this._marketPlaceProductUpdated = { ...this._marketPlaceProductUpdated, [field]: event.target.value };
    }
  }

  filesDropped(event) {
    const file = event[0].file;
    debugger;
    this.middleware.uploadProductImage(file, 'd1nwNNzAsk2Y5JH3gij6qg', 1);
  }
}
