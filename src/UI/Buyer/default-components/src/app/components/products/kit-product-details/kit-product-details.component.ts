import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { Asset, MarketplaceMeKitProduct, MarketplaceMeProduct, MeProductInKit, Variant, VariantSpec } from '@ordercloud/headstart-sdk';
import { ShopperContextService } from 'marketplace';
import { PriceBreak, Spec } from 'ordercloud-javascript-sdk';
import { Subscription } from 'rxjs';
import { ProductDetailService } from '../product-details/product-detail.service';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';
import { SpecFormEvent } from '../spec-form/spec-form-values.interface';
import { SpecFormService } from '../spec-form/spec-form.service';
import { faCaretRight, faCaretDown } from '@fortawesome/free-solid-svg-icons';

@Component({
  templateUrl: './kit-product-details.component.html',
  styleUrls: ['./kit-product-details.component.scss']
})
export class OCMKitProductDetails implements OnInit, OnDestroy {
  panelActiveIDs: string[]
  isAddingToCart = false;
  userCurrency: string;
  _product: MarketplaceMeKitProduct;
  routeSubscription: Subscription;
  faCaretDown = faCaretDown;
  faCaretRight = faCaretRight;

  // holds everything about a product needed for this view, easily referenced by productID
  productDictionary: {
    [key: string]: {
      product: MarketplaceMeProduct;
      images: Asset[];
      imageUrl: string;
      priceBreaks: PriceBreak[];
      specs: Spec[];
      specForm: FormGroup;
      quantity?: number;
      quantityValid: boolean;
      price?: number;
      percentSavings?: number;
      isExpanded?: boolean;
      errorMessage?: string;
      disabledVariants: Variant[];
    };
  } = {};

  @Input() set product(product: MarketplaceMeKitProduct) {
    this._product = product
    this.setKitDetails();

    // expand all panels that have specs by default
    this.panelActiveIDs = this._product.ProductAssignments.ProductsInKit.filter(p => {
      return p.Specs && p.Specs.length
    }).map(p => p.Product.ID);
  }

  constructor(
    private context: ShopperContextService,
    private specFormService: SpecFormService,
    private productDetailService: ProductDetailService,
    private router: Router
  ) {
    this.dontReuseComponent();
  }

  dontReuseComponent(): void {
    // tell angular to reload the component on url changes - by default angular will try to reuse the same component
    // so url will change but view won't change when we route to product detail of a product in the kit
    this.router.routeReuseStrategy.shouldReuseRoute = (): boolean => false;
    this.routeSubscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        // Trick the Router into believing it's last link wasn't previously loaded
        this.router.navigated = false;
      }
    });
  }

  setKitDetails(): void {
    this._product.ProductAssignments.ProductsInKit.forEach(p => {

      // min/max qty flow down from kit (possibly put this in middleware)
      const isOrderable = p.Product.PriceSchedule;
      if (isOrderable) {
        p.Product.PriceSchedule.MinQuantity = p.MinQty;
        p.Product.PriceSchedule.MaxQuantity = p.MaxQty;
      }
      const images = (p as any).Images as Asset[]; // TODO: remove this cast once types are generated
      const appConfig = this.context.appSettings;
      this.productDictionary[p.ID] = {
        product: p.Product,
        specs: p.Static ? this.filterStaticKitSpecs(p) : p.Specs,
        images,
        imageUrl: images && images.length ? `${appConfig.middlewareUrl}/assets/${appConfig.sellerID}/products/${p.ID}/thumbnail?size=S` : 'http://placehold.it/100x100',
        priceBreaks: p.Product.PriceSchedule?.PriceBreaks ?? [],
        specForm: {} as FormGroup,
        quantity: p.Product.PriceSchedule?.PriceBreaks[0].Quantity,
        quantityValid: true,
        price: undefined,
        percentSavings: undefined,
        isExpanded: true,
        disabledVariants: p.Variants.filter(v => !v.Active)
      }
      this.calculatePrice(p.ID);
    })
  }

  ngOnInit(): void {
    this.userCurrency = this.context.currentUser.get().Currency;
  }

  qtyChange(productID: string, { valid, qty }: QtyChangeEvent): void {
    this.productDictionary[productID].quantityValid = valid;
    if (valid) {
      this.productDictionary[productID].errorMessage = '';
      this.productDictionary[productID].quantity = qty
      this.calculatePrice(productID);
    } else {
      const maxQty = this.productDictionary[productID].product.PriceSchedule.MaxQuantity;
      const minQty = this.productDictionary[productID].product.PriceSchedule.MinQuantity;
      if (qty > maxQty) {
        this.productDictionary[productID].errorMessage = `Quantity must not exceed ${maxQty}`
      } else if (qty < minQty) {
        this.productDictionary[productID].errorMessage = `Quantity must not be less than ${minQty}`
      }
    }
  }

  calculatePrice(productID: string): void {
    const details = this.productDictionary[productID];
    details.price = this.productDetailService.getProductPrice(
      details.priceBreaks,
      details.specs,
      details.specForm,
      details.quantity
    );
    if (details.priceBreaks.length) {
      const baseprice = details.quantity * details.priceBreaks[0].Price;
      details.percentSavings = this.productDetailService.getPercentSavings(details.price, baseprice)
    }
  }

  onSpecFormChange(productID: string, event: SpecFormEvent): void {
    const details = this.productDictionary[productID];
    details.specForm = event.form;
    this.calculatePrice(productID);
  }

  async addToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      const lineItems = this._product.ProductAssignments.ProductsInKit.map(kitDefinition => {
        const details = this.productDictionary[kitDefinition.Product.ID];
        return {
          ProductID: kitDefinition.Product.ID,
          Quantity: kitDefinition.MinQty,
          Specs: kitDefinition.Static ?
            this.buildStaticKitSpecs(kitDefinition) :
            this.specFormService.getLineItemSpecs(details.specs, details.specForm),
          xp: {
            ImageUrl: this.specFormService.getLineItemImageUrl(details.images, details.specs, details.specForm),
            KitProductImageUrl: this._product.Images && this._product.Images.length ? this._product.Images[0].Url : null,
            KitProductID: this._product.Product.ID // used to group kit line items during checkout
          }
        }
      })
      await this.context.order.cart.addMany(lineItems as any);
    } finally {
      this.isAddingToCart = false;
    }
  }

  buildStaticKitSpecs(kitDefinition: MeProductInKit): VariantSpec[] {
    // a static kit is one where the options for all specs are pre-determined
    // spec combo is the identifier for the combination of spec/variant
    const specCombo = kitDefinition.SpecCombo;
    const variant = kitDefinition.Variants.find((v: any) => v.xp.SpecCombo === specCombo);
    if (!variant) {
      return []
    }
    return variant.Specs;
  }

  filterStaticKitSpecs(kitDefinition: MeProductInKit): any {
    // since a static kit means specs are pre-determined then we can't
    // let user choose another spec option and need to limit those options
    const allowedSpecs = this.buildStaticKitSpecs(kitDefinition);
    const filteredSpecs = kitDefinition.Specs.map(spec => {
      const allowedSpec = allowedSpecs.find(s => s.SpecID === spec.ID);
      /** cast as any to set readonly property*/
      (spec as any).Options = spec.Options.filter(o => o.ID === allowedSpec.OptionID)
      return spec;
    })

    return filteredSpecs;
  }

  areQuantitesValid(): boolean {
    return !Object.values(this.productDictionary).find(details => !details.quantityValid)
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }

}
