import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { BuyerAddress, ListAddress, Address } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ValidateUSZip, ValidatePhone, ValidateEmail } from '@app-seller/validators/validators';
import { BuyerLocationService } from '../buyer-location.service';
import { BuyerService } from '../../buyers/buyer.service';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';

@Component({
  selector: 'app-buyer-location-table',
  templateUrl: './buyer-location-table.component.html',
  styleUrls: ['./buyer-location-table.component.scss'],
})
export class BuyerLocationTableComponent extends ResourceCrudComponent<BuyerAddress> {
  suggestedAddresses: ListAddress;
  selectedAddress: Address;

  constructor(
    private buyerLocationService: BuyerLocationService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService,
    private middleware: MiddlewareAPIService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, buyerLocationService, router, activatedroute, ngZone);
  }

  // handleAddressSelect(address) {
  //   this.updatedResource = address;
  // }

  // discardChanges(): void {
  //   this.suggestedAddresses = null;
  //   this.setUpdatedResourceAndResourceForm(this.resourceInSelection);
  // }

  // async updateExistingResource(): Promise<void> {
  //   console.log('updating existing resource', this.updatedResource);
  //   try {
  //     this.dataIsSaving = true;
  //     const updatedResource = await this.ocService.updateResource(this.updatedResource);
  //     this.resourceInSelection = this.copyResource(updatedResource);
  //     this.setUpdatedResourceAndResourceForm(updatedResource);
  //     this.suggestedAddresses = null;
  //     this.dataIsSaving = false;
  //   } catch (ex) {
  //     this.suggestedAddresses = this.ocService.getSuggestedAddresses(ex, this.updatedResource);
  //     this.dataIsSaving = false;
  //     throw ex;
  //   }
  // }

  // async createNewResource(): Promise<void> {
  //   console.log('creating new with this updated resource', this.updatedResource);
  //     try {
  //       this.dataIsSaving = true;
  //       const newResource = await this.ocService.createNewResource(this.updatedResource);
  //       this.selectResource(newResource);
  //       this.suggestedAddresses = null;
  //       this.dataIsSaving = false;
  //     } catch (ex) {
  //       this.suggestedAddresses = this.ocService.getSuggestedAddresses(ex, this.updatedResource);
  //       this.dataIsSaving = false;
  //       throw ex;
  //     }
  // }
}
