import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Promotion, OcPromotionService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class PromotionService extends ResourceCrudService<Promotion> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocPromotionService: OcPromotionService) {
    super(router, activatedRoute, ocPromotionService, '/promotions', 'promotions');
  }
}
