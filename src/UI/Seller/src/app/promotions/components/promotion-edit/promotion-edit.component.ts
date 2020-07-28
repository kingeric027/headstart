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
  scopeToSupplier: boolean = false;
  constructor(public promotionService: PromotionService, private ocPromotionService: OcPromotionService, private ocSupplierService: OcSupplierService, private router: Router,) {}

  ngOnInit(): void {
    this.isCreatingNew = this.promotionService.checkIfCreatingNew();
  }

  refreshPromoData(promo: Promotion<PromotionXp>): void {
    const now = moment(Date.now()).format('YYYY-MM-DD[T]hh:mm');
    promo.ExpirationDate ? (this.isExpired = Date.parse(promo.ExpirationDate) < Date.parse(now)) : (this.isExpired = false);
    // Modify the datetime to work with the UI
    promo.StartDate && (promo.StartDate = moment(promo.StartDate).format('YYYY-MM-DD[T]hh:mm'));
    if (promo.ExpirationDate) {
      this.hasExpiration = true;
      promo.ExpirationDate = promo.ExpirationDate = moment(promo.ExpirationDate).format('YYYY-MM-DD[T]hh:mm');
    } else {
      this.hasExpiration = false;
    }
    promo.RedemptionLimit ? this.hasRedemptionLimit = true : this.hasRedemptionLimit = false;
    promo.RedemptionLimitPerUser ? this.limitPerUser = true : this.limitPerUser = false;
    promo.xp?.MaxShipCost ? this.capShipCost = true : this.capShipCost = false;
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
    if (this._promotionEditable?.xp?.ScopeToSupplier) this.handleUpdatePromo({target: { value: s.ID }}, 'xp.Supplier');
  }

  createPromotionForm(promotion: Promotion) {
    this.resourceForm = new FormGroup({
      Code: new FormControl(promotion.Code, Validators.required),
      Type: new FormControl(_get(promotion, 'xp.Type')),
      Value: new FormControl(_get(promotion, 'xp.Value'), Validators.min(0)),
      AppliesTo: new FormControl(_get(promotion, 'xp.AppliesTo')),
      ScopeToSupplier: new FormControl(_get(promotion, 'xp.ScopeToSupplier')),
      Supplier: new FormControl(_get(promotion, 'xp.Supplier')),
      RedemptionLimit: new FormControl(promotion.RedemptionLimit, Validators.min(0)),
      RedemptionLimitPerUser: new FormControl(promotion.RedemptionLimitPerUser, Validators.min(0)),
      Description: new FormControl(promotion.Description),
      FinePrint: new FormControl(promotion.FinePrint),
      StartDate: new FormControl(promotion.StartDate, Validators.required),
      ExpirationDate: new FormControl(promotion.ExpirationDate),
      CanCombine: new FormControl(promotion.CanCombine),
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
        (field === 'Type' || field === 'CanCombine' || field === 'xp.ScopeToSupplier') ? event.target.checked : typeOfValue === 'number' ? Number(event.target.value) : event.target.value,
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
    let valueString = "off entire order";
    const promo = this._promotionEditable;
    if (promo?.xp?.Type === "FixedAmount") valueString = `$${promo?.xp?.Value} ${valueString}`;
    if (promo?.xp?.Type === "Percentage") valueString = `${promo?.xp?.Value}% ${valueString}`;
    if (promo?.xp?.Type === "FreeShipping") valueString = `Free shipping on entire order`;
    if (promo?.xp?.MinReq?.Type === "MinPurchase" && promo?.xp?.MinReq?.Int) {
      promo?.xp?.ScopeToSupplier ? 
      valueString = `${valueString} when you purhcase $${promo?.xp?.MinReq?.Int} or more worth of ${this.selectedSupplier?.Name} products`
        :
      valueString = `${valueString} over $${promo?.xp?.MinReq?.Int}`;
    }
    if (promo?.xp?.MinReq?.Type === "MinItemQty" && promo?.xp?.MinReq?.Int) {
      valueString = promo?.xp?.ScopeToSupplier ? 
      `${valueString} when you purchase ${this._promotionEditable?.xp?.MinReq?.Int} or more ${this.selectedSupplier?.Name} products`
        :
      `${valueString} over ${this._promotionEditable?.xp?.MinReq?.Int} items`
    }
    // Update `promotion.Description` with this value string
    this.handleUpdatePromo({target: {value: valueString}}, 'Description');
    return valueString;
  }

  getDateRangeDisplay(): string {
    let dateRangeString = "Valid from";
    const formattedStart = this._promotionEditable.StartDate.substr(0, 4) === moment().format('YYYY') ? moment(this._promotionEditable.StartDate).format('MMM Do') : moment(this._promotionEditable.StartDate).format('MMM Do, YYYY');
    const formattedExpiry = this._promotionEditable.ExpirationDate.substr(0, 4) === moment().format('YYYY') ? moment(this._promotionEditable.ExpirationDate).format('MMM Do') : moment(this._promotionEditable.ExpirationDate).format('MMM Do, YYYY');
    moment(this._promotionEditable.StartDate).format('MM-DD-YYYY') === moment().format('MM-DD-YYYY') ?
      dateRangeString = `${dateRangeString} today to ${formattedExpiry}`
    :
      dateRangeString = `${dateRangeString} ${formattedStart} to ${formattedExpiry}`;
    return dateRangeString;
  }

  getEligibilityDisplay(): string {
    let eligibilityString = "For";
    if (this._promotionEditable.AllowAllBuyers) eligibilityString = `${eligibilityString} all buyers`;
    // In the future, there will be other considerations for finer grained eligibility
    return eligibilityString;
  }

  getUsageLimitDisplay(): string {
    let usageLimitString = "Limit of";
    if (this._promotionEditable.RedemptionLimit) usageLimitString = `${usageLimitString} ${this._promotionEditable.RedemptionLimit} ${this._promotionEditable.RedemptionLimit > 1 ? 'uses' : 'use'}`;
    if (this._promotionEditable.RedemptionLimitPerUser) usageLimitString = `${usageLimitString} ${this._promotionEditable.RedemptionLimitPerUser} per user`
    if (this._promotionEditable.RedemptionLimit && this._promotionEditable.RedemptionLimitPerUser) usageLimitString = `Limit of ${this._promotionEditable.RedemptionLimit} uses, ${this._promotionEditable.RedemptionLimitPerUser} per user`
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

  getSaveBtnText(): string {
    return this.promotionService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
  }

  handleClearMinReq(): void {
    this._promotionEditable.EligibleExpression = "true";
    this.handleUpdatePromo({target: { value: {Type: null, Int: null} }}, "xp.MinReq");
    this.handleUpdatePromo({target: { value: false }}, "xp.ScopeToSupplier");
  }

  handleDiscardChanges(): void {
    this.refreshPromoData(this._promotionStatic)
  }

  buildValueExpression(): void {
    let valueExpression: string = "Order.Subtotal";
    switch(this._promotionEditable.xp?.Type) {
      case 'FixedAmount':
        valueExpression = `${this._promotionEditable.xp?.Value}`
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
    let eligibleExpression: string;
    switch (this._promotionEditable.xp?.MinReq?.Type) {
      case 'MinPurchase':
        eligibleExpression = this._promotionEditable?.xp?.ScopeToSupplier ? 
        `items.total(SupplierID = '${this.selectedSupplier?.ID}') >= ${this._promotionEditable.xp?.MinReq?.Int}`
          :
        `Order.Subtotal >= ${this._promotionEditable.xp?.MinReq?.Int}`;
        break;
      case 'MinItemQty':
        eligibleExpression = this._promotionEditable?.xp?.ScopeToSupplier ? 
        `items.Quantity(SupplierID = '${this.selectedSupplier?.ID}') >= ${this._promotionEditable.xp?.MinReq?.Int}`
          :
        `Order.LineItemCount >= ${this._promotionEditable.xp?.MinReq?.Int}`
        break;
    }
    if (this._promotionEditable.xp?.MaxShipCost) {
      this._promotionEditable.xp?.MinReq?.Type ? eligibleExpression = `${eligibleExpression} and Order.ShippingCost < ${this._promotionEditable.xp?.MaxShipCost}`
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
