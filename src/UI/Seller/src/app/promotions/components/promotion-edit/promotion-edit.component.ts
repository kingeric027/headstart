import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { faTimesCircle, faCalendar, faExclamationCircle, faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Promotion, OcPromotionService, OcSupplierService } from '@ordercloud/angular-sdk';
import { PromotionService } from '@app-seller/promotions/promotion.service';
import { PromotionXp, MarketplacePromoType, MarketplacePromoEligibility } from '@app-seller/shared/models/marketplace-promo.interface';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { MarketplaceSupplier } from '@ordercloud/headstart-sdk';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'app-promotion-edit',
  templateUrl: './promotion-edit.component.html',
  styleUrls: ['./promotion-edit.component.scss'],
})
export class PromotionEditComponent implements OnInit {
  @ViewChild('popover', { static: false })
  public popover: NgbPopover;
  @Input()
  filterConfig;
  @Input()
  set resourceInSelection(promotion: Promotion<PromotionXp>) {
    if (promotion.ID) {
      this.setUpSuppliers(promotion.xp?.Supplier);
      this.refreshPromoData(promotion);
    } else {
      this.setUpSuppliers();
      this.refreshPromoData(this.promotionService.emptyResource);
    }
  }
  @Input()
  updatedResource;
  @Output()
  updateResource = new EventEmitter<any>();
  suppliers: MarketplaceSupplier[];
  selectedSupplier: MarketplaceSupplier;
  resourceForm: FormGroup;
  _promotionEditable: Promotion<PromotionXp>;
  _promotionStatic: Promotion<PromotionXp>;
  hasRedemptionLimit = false;
  limitPerUser = false;
  hasExpiration = false;
  capShipCost = false;
  areChanges = false;
  isExpired = false;
  dataIsSaving = false;
  isCreatingNew: boolean;
  faTimesCircle = faTimesCircle;
  faExclamationCircle = faExclamationCircle;
  faQuestionCircle = faQuestionCircle;
  faCalendar = faCalendar;
  constructor(public promotionService: PromotionService, private ocPromotionService: OcPromotionService, private ocSupplierService: OcSupplierService, private router: Router, private translate: TranslateService) {}

  ngOnInit(): void {
    this.isCreatingNew = this.promotionService.checkIfCreatingNew();
  }

  refreshPromoData(promo: Promotion<PromotionXp>): void {
    const now = moment(Date.now()).format('YYYY-MM-DD[T]hh:mm');
    this.isExpired = promo.ExpirationDate ? (Date.parse(promo.ExpirationDate) < Date.parse(now)) : false;
    // Modify the datetime to work with the UI
    if(promo.StartDate) promo.StartDate = moment(promo.StartDate).format('YYYY-MM-DD[T]hh:mm');
    if (promo.ExpirationDate) {
      this.hasExpiration = true;
      promo.ExpirationDate = promo.ExpirationDate = moment(promo.ExpirationDate).format('YYYY-MM-DD[T]hh:mm');
    } else {
      this.hasExpiration = false;
    }
    this.hasRedemptionLimit = promo.RedemptionLimit ? true : false;
    this.limitPerUser = promo.RedemptionLimitPerUser ? true : false;
    this.capShipCost = promo.xp?.MaxShipCost ? true : false;
    this._promotionEditable = JSON.parse(JSON.stringify(promo));
    this._promotionStatic = JSON.parse(JSON.stringify(promo));
    this.createPromotionForm(promo);
  }

  async setUpSuppliers(existingSupplierID?: string): Promise<void> {
    const supplierResponse = await this.ocSupplierService.List({pageSize: 100}).toPromise();
    this.suppliers = supplierResponse.Items;
    await this.selectSupplier(existingSupplierID || this.suppliers[0].ID);
  }

  async selectSupplier(supplierID: string): Promise<void> {
    const s = await this.ocSupplierService.Get(supplierID).toPromise();
    this.selectedSupplier = s;
    if (this._promotionEditable?.xp?.AppliesTo === 'SpecificSupplier') this.handleUpdatePromo({target: { value: s.ID }}, 'xp.Supplier');
  }

  createPromotionForm(promotion: Promotion) {
    this.resourceForm = new FormGroup({
      Code: new FormControl(promotion.Code, Validators.required),
      Type: new FormControl(_get(promotion, 'xp.Type')),
      Value: new FormControl(_get(promotion, 'xp.Value'), Validators.min(0)),
      AppliesTo: new FormControl(_get(promotion, 'xp.AppliesTo')),
      Supplier: new FormControl(_get(promotion, 'xp.Supplier')),
      RedemptionLimit: new FormControl(promotion.RedemptionLimit, Validators.min(0)),
      RedemptionLimitPerUser: new FormControl(promotion.RedemptionLimitPerUser, Validators.min(0)),
      Description: new FormControl(promotion.Description),
      FinePrint: new FormControl(promotion.FinePrint),
      StartDate: new FormControl(promotion.StartDate, Validators.required),
      ExpirationDate: new FormControl(promotion.ExpirationDate),
      CanCombine: new FormControl(promotion.CanCombine),
      Automatic: new FormControl(_get(promotion, 'xp.Automatic')),
      AllowAllBuyers: new FormControl(promotion.AllowAllBuyers),
      MinReqType: new FormControl(_get(promotion, 'xp.MinReq.Type')),
      MinReqInt: new FormControl(_get(promotion, 'xp.MinReq.Int'), Validators.min(0)),
      MaxShipCost: new FormControl(_get(promotion, 'xp.MaxShipCost'), Validators.min(0))
    });
  }

  generateRandomCode(): void {
    const randomCode = Math.random().toString(36).slice(2).substr(0, 5).toUpperCase();
    this.handleUpdatePromo({target: {value: randomCode}}, 'Code');
    this._promotionEditable.Code = randomCode;
    this.resourceForm.controls['Code'].setValue(randomCode);
  }

  handleUpdatePromo(event: any, field: string, typeOfValue?: string): void {
    const promoUpdate = {
      field,
      value:
        ['Type', 'CanCombine', 'xp.Automatic'].includes(field) ? 
          event.target.checked : typeOfValue === 'number' ? Number(event.target.value) : event.target.value,
    };
    this.updatePromoResource(promoUpdate);
  }

  updatePromoResource(promoUpdate: any): void {
    const resourceToUpdate = this._promotionEditable || this.promotionService.emptyResource;
    this._promotionEditable = this.promotionService.getUpdatedEditableResource(promoUpdate, resourceToUpdate);
    this.areChanges = this.promotionService.checkForChanges(this._promotionEditable, this._promotionStatic);
    this.buildValueExpression();
    this.buildEligibleExpression();
  }

  promoTypeCheck(type: MarketplacePromoType): boolean {
    return type === this._promotionEditable?.xp?.Type;
  }

  promoEligibilityCheck(eligibility: MarketplacePromoEligibility): boolean {
    return eligibility === this._promotionEditable?.xp?.AppliesTo;
  }

  getValueDisplay(): string {
    const promo = this._promotionEditable;
    let valueString = promo?.xp?.AppliesTo === 'SpecificSupplier' ? `${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.OFF_ENTIRE')} ${this.selectedSupplier?.Name} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.PRODUCTS_ORDER')}` : this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.OFF_ENTIRE_ORDER');
    if (promo?.xp?.Type === "FixedAmount") valueString = `$${promo?.xp?.Value} ${valueString}`;
    if (promo?.xp?.Type === "Percentage") valueString = `${promo?.xp?.Value}% ${valueString}`;
    if (promo?.xp?.Type === "FreeShipping") valueString = this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.FREE_SHIPPING_ENTIRE_ORDER');
    if (promo?.xp?.MinReq?.Type === "MinPurchase" && promo?.xp?.MinReq?.Int) valueString = `${valueString} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.OVER')} $${promo?.xp?.MinReq?.Int}`;
    if (promo?.xp?.MinReq?.Type === "MinItemQty" && promo?.xp?.MinReq?.Int) `${valueString} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.OVER')} ${this._promotionEditable?.xp?.MinReq?.Int} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.VALUE.ITEMS')}`
    // Update `promotion.Description` with this value string
    this.handleUpdatePromo({target: {value: valueString}}, 'Description');
    return valueString;
  }

  getDateRangeDisplay(): string {
    let dateRangeString = this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.DATE.VALID_FROM');
    const formattedStart = this._promotionEditable.StartDate.substr(0, 4) === moment().format('YYYY') ? moment(this._promotionEditable.StartDate).format('MMM Do') : moment(this._promotionEditable.StartDate).format('MMM Do, YYYY');
    const formattedExpiry = this._promotionEditable.ExpirationDate.substr(0, 4) === moment().format('YYYY') ? moment(this._promotionEditable.ExpirationDate).format('MMM Do') : moment(this._promotionEditable.ExpirationDate).format('MMM Do, YYYY');
    moment(this._promotionEditable.StartDate).format('MM-DD-YYYY') === moment().format('MM-DD-YYYY') ?
      dateRangeString = `${dateRangeString} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.DATE.TODAY_TO')} ${formattedExpiry}`
    :
      dateRangeString = `${dateRangeString} ${formattedStart} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.DATE.TO')} ${formattedExpiry}`;
    return dateRangeString;
  }

  getEligibilityDisplay(): string {
    let eligibilityString = this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.ELIGIBILITY.FOR');
    if (this._promotionEditable.AllowAllBuyers) eligibilityString = `${eligibilityString} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.ELIGIBILITY.ALL_BUYERS')}`;
    // In the future, there will be other considerations for finer grained eligibility
    return eligibilityString;
  }

  getUsageLimitDisplay(): string {
    let usageLimitString = this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.LIMIT_OF');
    if (this._promotionEditable.RedemptionLimit) usageLimitString = `${usageLimitString} ${this._promotionEditable.RedemptionLimit} ${this._promotionEditable.RedemptionLimit > 1 ? this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.USES') : this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.USE')}`;
    if (this._promotionEditable.RedemptionLimitPerUser) usageLimitString = `${usageLimitString} ${this._promotionEditable.RedemptionLimitPerUser} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.PER_USER')}`
    if (this._promotionEditable.RedemptionLimit && this._promotionEditable.RedemptionLimitPerUser) usageLimitString = `${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.LIMIT_OF')} ${this._promotionEditable.RedemptionLimit} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.USES')}, ${this._promotionEditable.RedemptionLimitPerUser} ${this.translate.instant('ADMIN.PROMOTIONS.DISPLAY.USAGE.PER_USER')}`
    return usageLimitString;
  }

  getMinDate(): string {
    return moment().format("YYYY-MM-DD[T]hh:mm")
  }

  toggleHasRedemptionLimit(): void {
    this.hasRedemptionLimit = !this.hasRedemptionLimit;
    if (!this.hasRedemptionLimit) this._promotionEditable.RedemptionLimit = null;
  }

  toggleLimitPerUser(): void {
    this.limitPerUser = !this.limitPerUser;
    if(!this.limitPerUser) this._promotionEditable.RedemptionLimitPerUser = null;
  }

  toggleHasExpiration(): void {
    this.hasExpiration = !this.hasExpiration;
    if (!this.hasExpiration) this._promotionEditable.ExpirationDate = null;
  }

  toggleCapShipCost(): void {
    this.capShipCost = !this.capShipCost;
    if (!this.capShipCost) this._promotionEditable.xp.MaxShipCost = null;
  }

  toggleApplyToSpecificSupplier(): void {
    (this._promotionEditable as any).LineItemLevel = !(this._promotionEditable as any).LineItemLevel;
  }

  getSaveBtnText(): string {
    return this.promotionService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
  }

  handleClearMinReq(): void {
    this._promotionEditable.EligibleExpression = "true";
    this.handleUpdatePromo({target: { value: {Type: null, Int: null} }}, "xp.MinReq");
  }

  handleDiscardChanges(): void {
    this.refreshPromoData(this._promotionStatic)
  }

  buildValueExpression(): void {
    let valueExpression: string = this._promotionEditable?.xp?.AppliesTo === 'SpecificSupplier' ? "item.LineSubtotal" : "Order.Subtotal";
    switch(this._promotionEditable.xp?.Type) {
      case 'FixedAmount':
        valueExpression = this._promotionEditable?.xp?.AppliesTo === 'SpecificSupplier' ? 
        `${this._promotionEditable.xp?.Value} / items.count(SupplierID = '${this.selectedSupplier?.ID}')` 
          : 
        `${this._promotionEditable.xp?.Value}`
        break;
      case 'Percentage':
        valueExpression = `${valueExpression} * ${(this._promotionEditable.xp?.Value / 100)}`
        break;
      case 'FreeShipping':
        valueExpression = `Order.ShippingCost`;
        break;
    }
    this._promotionEditable.ValueExpression = valueExpression;
  }

  buildEligibleExpression(): void {
    let eligibleExpression: string = this._promotionEditable?.xp?.AppliesTo === 'SpecificSupplier' ? `item.SupplierID = '${this.selectedSupplier?.ID}'` : `true`;
    switch (this._promotionEditable.xp?.MinReq?.Type) {
      case 'MinPurchase':
        eligibleExpression = this._promotionEditable?.xp?.AppliesTo === 'SpecificSupplier' ? 
        `${eligibleExpression} and items.total(SupplierID = '${this.selectedSupplier?.ID}') >= ${this._promotionEditable.xp?.MinReq?.Int}`
          :
        `Order.Subtotal >= ${this._promotionEditable.xp?.MinReq?.Int}`;
        break;
      case 'MinItemQty':
        eligibleExpression = this._promotionEditable?.xp?.AppliesTo === 'SpecificSupplier' ? 
        `${eligibleExpression} and items.Quantity(SupplierID = '${this.selectedSupplier?.ID}') >= ${this._promotionEditable.xp?.MinReq?.Int}`
          :
        `Order.LineItemCount >= ${this._promotionEditable.xp?.MinReq?.Int}`
        break;
    }
    if (this._promotionEditable.xp?.MaxShipCost) {
      this._promotionEditable.xp?.MinReq?.Type ? eligibleExpression = `Order.ShippingCost < ${this._promotionEditable.xp?.MaxShipCost}`
      :
      eligibleExpression = `Order.ShippingCost < ${this._promotionEditable.xp?.MaxShipCost}`
    }
    this._promotionEditable.EligibleExpression = eligibleExpression;
  }

  async handleSave(): Promise<void> {
    if (this.isCreatingNew) {
      await this.createPromotion(this._promotionEditable);
    } else {
      await this.updatePromotion(this._promotionEditable);
    }
  }

  async createPromotion(promo: Promotion<PromotionXp>): Promise<void> {
    try {
      this.dataIsSaving = true;
      // Set promotion.Name to promotion.Code automatically
      promo.Name = promo.Code;
      const newPromo = await this.ocPromotionService.Create(promo).toPromise();
      this.refreshPromoData(newPromo);
      this.router.navigateByUrl(`/promotions/${newPromo.ID}`);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  // TODO: Find diff'd object and only 'PATCH' what changed?
  async updatePromotion(promo: Promotion<PromotionXp>): Promise<void> {
    try {
      this.dataIsSaving = true;
      const updatedPromo = await this.ocPromotionService.Save(promo.ID, promo).toPromise();
      this.refreshPromoData(updatedPromo);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async handleDelete(event: any): Promise<void> {
    await this.ocPromotionService.Delete(this._promotionStatic.ID).toPromise();
    this.router.navigateByUrl('/promotions');
  }
}
