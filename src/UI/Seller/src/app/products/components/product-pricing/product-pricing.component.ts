import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ResourceUpdate } from '@app-seller/shared/models/resource-update.interface';
import { PriceSchedule, OcBuyerService } from '@ordercloud/angular-sdk';
import { ToastrService } from 'ngx-toastr';
import { FormControl } from '@angular/forms';
import { SupportedRates } from '@app-seller/shared/models/supported-rates.interface';
import { BuyerTempService, SuperMarketplaceBuyer } from '@app-seller/shared/services/middleware-api/buyer-temp.service';
import { CatalogsTempService } from '@app-seller/shared/services/middleware-api/catalogs-temp.service';
import { SuperMarketplaceProduct, MarketplaceBuyer, HeadStartSDK, MarketplacePriceSchedule } from '@ordercloud/headstart-sdk';

@Component({
  selector: 'product-pricing-component',
  templateUrl: './product-pricing.component.html',
  styleUrls: ['./product-pricing.component.scss'],
})
export class ProductPricingComponent {
  @Input()
  readonly = false;
  @Input()
  productForm: FormControl;
  @Input()
  supplierCurrency: SupportedRates;
  @Input()
  sellerCurrency: SupportedRates;
  @Input()
  isRequired: boolean;
  @Input()
  set superMarketplaceProductStatic(value: SuperMarketplaceProduct) {
    this.superProduct = value;
    if (value.Product?.xp?.ProductType === 'Quote' && value.PriceSchedule.PriceBreaks === null) {
      this.superProduct.PriceSchedule.PriceBreaks = [{Price: null, Quantity: null}];
    }
    if (value) {
      this.supplierPriceSchedule = JSON.parse(JSON.stringify(value.PriceSchedule));
      if(this.readonly) {
        this.setUpBuyers();
        this.setUpExchangeRate();
        this.buyerMarkedUpSupplierPrices = this.getBuyerDisplayOfSupplierPriceSchedule();
      }
    }
  }

  @Output()
  updateProduct = new EventEmitter<ResourceUpdate>();
  supplierPriceSchedule: PriceSchedule;
  buyerMarkedUpSupplierPrices: PriceSchedule;

  // defaulting to they are the same currency
  supplierToSellerCurrencyRate = 1;
  superProduct;

  buyers: MarketplaceBuyer[] = [];
  selectedBuyerIndex = 0;
  selectedSuperMarketplaceBuyer: SuperMarketplaceBuyer;

  isUsingPriceOverride = false;
  areChangesToBuyerVisibility = false;
  
  emptyPriceSchedule = {
    UseCumulativeQuantity: true,
    PriceBreaks: [{
      Price: 0,
      Quantity: 1,
    }
  ]
} as PriceSchedule;

  isSavedOverride = false;
  overridePriceScheduleEditable = this.emptyPriceSchedule;
  overridePriceScheduleStatic = this.emptyPriceSchedule;

  constructor(
    private toasterService: ToastrService, 
    private ocBuyerService: OcBuyerService, 
    private catalogsTempService: CatalogsTempService, 
    private buyerTempService: BuyerTempService) {}
  
  async setUpExchangeRate(): Promise<void> {
    if(this.supplierCurrency !== this.sellerCurrency) {
      const usdExchangeRates = await HeadStartSDK.ExchangeRates.Get(this.sellerCurrency.Currency as any);
      const supplierToSellerExchangeRate = usdExchangeRates.Items.find(r => r.Currency === this.supplierCurrency.Currency);
      this.supplierToSellerCurrencyRate = supplierToSellerExchangeRate.Rate;
    }
  }

  getBuyerPercentMarkupPrice(supplierPrice: number): number {
    const markupMultiplier = (this.selectedSuperMarketplaceBuyer?.Markup?.Percent || 0)/100 + 1; 
    const conversionRate = this.supplierToSellerCurrencyRate * markupMultiplier;
    return Math.round(supplierPrice * conversionRate);
  }

  updateSupplierPriceSchedule(event: PriceSchedule): void {
    this.updateProduct.emit({field: 'PriceSchedule', value: event});
  }
  
  updateOverridePriceSchedule(event: PriceSchedule): void {
    this.overridePriceScheduleEditable = event;
    this.diffBuyerVisibility();
  }

  diffBuyerVisibility(): void {
    if(!this.isSavedOverride && this.isUsingPriceOverride) {
      this.areChangesToBuyerVisibility = true;
    } else if (this.isSavedOverride && !this.isUsingPriceOverride) {
      this.areChangesToBuyerVisibility = true;
    } else {
      this.areChangesToBuyerVisibility = JSON.stringify(this.overridePriceScheduleEditable) !== JSON.stringify(this.overridePriceScheduleStatic);
    }
  }

  handleDiscardBuyerPricingChanges(): void {
    this.isUsingPriceOverride = this.isSavedOverride;
    this.resetOverridePriceSchedules(this.overridePriceScheduleStatic);
  }

  async handleSaveBuyerPricing(): Promise<void> {
    const productID = this.superProduct.Product.ID;
    const buyerID = this.selectedSuperMarketplaceBuyer.Buyer.ID;
    this.overridePriceScheduleEditable.Name = this.superProduct.Product.Name + ' Seller Override';
    if(!this.isSavedOverride && this.isUsingPriceOverride) {
      const newPriceSchedule = await this.catalogsTempService.CreatePricingOverride(productID, buyerID, this.overridePriceScheduleEditable);
      this.isSavedOverride = true;
      this.resetOverridePriceSchedules(newPriceSchedule);
    } else if (this.isSavedOverride && !this.isUsingPriceOverride) {
      await this.catalogsTempService.DeletePricingOverride(productID, buyerID);
      this.isSavedOverride = false;
      this.resetOverridePriceSchedules(this.emptyPriceSchedule);
    } else {
      const newPriceSchedule = await this.catalogsTempService.UpdatePricingOverride(productID, buyerID, this.overridePriceScheduleEditable);
      this.isSavedOverride = true;
      this.resetOverridePriceSchedules(newPriceSchedule);
    } 
  }

  getBuyerDisplayOfSupplierPriceSchedule(): PriceSchedule {
    const priceScheduleCopy = JSON.parse(JSON.stringify(this.supplierPriceSchedule));
    priceScheduleCopy.PriceBreaks = priceScheduleCopy.PriceBreaks.map(b => {
      b.Price = this.getBuyerPercentMarkupPrice(b.Price)
      return b;
    });
    return priceScheduleCopy;
  }

  setIsUsingPriceOverride(isUsingPriceOverride: boolean) {
    this.isUsingPriceOverride = isUsingPriceOverride;
    this.diffBuyerVisibility()
  }

  async getPriceScheduleOverrides(): Promise<void> {
    try {
      const override = await this.catalogsTempService.GetPricingOverride(this.superProduct.Product.ID, this.selectedSuperMarketplaceBuyer.Buyer.ID);
      this.resetOverridePriceSchedules(override);
      this.isSavedOverride = true;
      this.isUsingPriceOverride = true;
    } catch(ex) {
      if(ex?.error?.[0].ErrorCode === 'NotFound') {
        this.isSavedOverride = false;
        this.isUsingPriceOverride = false;
        this.resetOverridePriceSchedules(this.emptyPriceSchedule);
      } else {
        //  if it's anything other than a not found error throw it
        // not found just means no override exists
        throw ex;
      }
    }
  }

  resetOverridePriceSchedules(priceSchedule: MarketplacePriceSchedule): void {
    this.overridePriceScheduleEditable = JSON.parse(JSON.stringify(priceSchedule));
    this.overridePriceScheduleStatic = JSON.parse(JSON.stringify(priceSchedule));
    this.diffBuyerVisibility();
  }

  async setUpBuyers(): Promise<void> {
    const buyersResponse = await this.ocBuyerService.List({pageSize: 100}).toPromise();
    this.buyers = buyersResponse.Items;
    await this.selectBuyer(this.buyers[0]);
  }

  async selectBuyer(buyer: MarketplaceBuyer): Promise<void> {
    const superBuyer = await this.buyerTempService.get(buyer.ID);
    this.selectedSuperMarketplaceBuyer = superBuyer;
    this.buyerMarkedUpSupplierPrices = this.getBuyerDisplayOfSupplierPriceSchedule();
    this.getPriceScheduleOverrides();
  }
}
