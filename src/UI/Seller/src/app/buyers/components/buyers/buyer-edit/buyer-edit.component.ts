import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { BuyerTempService } from '@app-seller/shared/services/middleware-api/buyer-temp.service';
import { MarketplaceBuyer } from 'marketplace-javascript-sdk';
@Component({
  selector: 'app-buyer-edit',
  templateUrl: './buyer-edit.component.html',
  styleUrls: ['./buyer-edit.component.scss'],
})
export class BuyerEditComponent {
  @Input()
  resourceForm: FormGroup;
  @Input()
  orderCloudBuyer(buyer: MarketplaceBuyer) {
    if (product.ID) {
      this.handleSelectedBuyerChange(buyer);
    } else {
      this.createBuyerForm(this.productService.emptyResource);
    }
  }
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();

  constructor(private buyerTempService: BuyerTempService, private appAuthService: AppAuthService) {}

  updateResourceFromEvent(event: any, field: string): void {
    const value = field === 'Active' ? event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
  }

  async handleSelectedBuyerChange(buyer: MarketplaceBuyer): Promise<void> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const superMarketplaceBuyer = await this.buyerTempService.get(buyer.ID, accessToken);
    this.refreshBuyerData(superMarketplaceBuyer);
  }
}
