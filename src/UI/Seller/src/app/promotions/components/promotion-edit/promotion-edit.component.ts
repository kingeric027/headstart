import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { FacetService } from '@app-seller/facets/facet.service';
import { faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { Promotion } from '@ordercloud/angular-sdk';
import { PromotionService } from '@app-seller/promotions/promotion.service';
@Component({
  selector: 'app-promotion-edit',
  templateUrl: './promotion-edit.component.html',
  styleUrls: ['./promotion-edit.component.scss'],
})
export class PromotionEditComponent implements OnInit {
  @Input()
  filterConfig;
  @Input()
  set resourceInSelection(promotion: Promotion) {
    if (promotion.ID) {
      this.createPromotionForm(promotion);
    } else {
      this.createPromotionForm(this.promotionService.emptyResource);
    }
  }
  @Input()
  updatedResource;
  @Output()
  updateResource = new EventEmitter<any>();
  resourceForm: FormGroup;
  isCreatingNew: boolean;
  faTimesCircle = faTimesCircle;
  constructor(public promotionService: PromotionService) {}

  ngOnInit(): void {
    this.isCreatingNew = this.promotionService.checkIfCreatingNew();
  }

  createPromotionForm(promotion: Promotion) {
    this.resourceForm = new FormGroup({
      Name: new FormControl(promotion.Name, Validators.required),
      Code: new FormControl(promotion.Code, Validators.required),
      RedemptionLimit: new FormControl(promotion.RedemptionLimit),
      RedemptionLimitPerUser: new FormControl(promotion.RedemptionLimitPerUser),
      RedemptionCount: new FormControl(promotion.RedemptionCount),
      Description: new FormControl(promotion.Description),
      FinePrint: new FormControl(promotion.FinePrint),
      StartDate: new FormControl(promotion.StartDate),
      ExpirationDate: new FormControl(promotion.ExpirationDate),
      CanCombine: new FormControl(promotion.CanCombine),
      AllowAllBuyers: new FormControl(promotion.AllowAllBuyers),
    });
  }

  updateResourceFromEvent(event: any, field: string): void {
    field === 'Active'
      ? this.updateResource.emit({ value: event.target.checked, field })
      : this.updateResource.emit({ value: event.target.value, field });
  }
}
