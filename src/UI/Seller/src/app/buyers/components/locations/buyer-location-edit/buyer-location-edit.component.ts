import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { ListAddress, Address, BuyerAddress } from '@ordercloud/angular-sdk';
import { BuyerLocationService } from '../buyer-location.service';
import { ValidatePhone, ValidateUSZip, ValidateEmail } from '@app-seller/validators/validators';
import { Router } from '@angular/router';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { ResourceUpdate } from '@app-seller/shared/models/resource-update.interface';
import { getSuggestedAddresses } from '@app-seller/shared/services/address-suggestion.helper';
import { MarketplaceBuyerLocation } from 'marketplace-javascript-sdk/dist/models/MarketplaceBuyerLocation';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
@Component({
  selector: 'app-buyer-location-edit',
  templateUrl: './buyer-location-edit.component.html',
  styleUrls: ['./buyer-location-edit.component.scss'],
})
export class BuyerLocationEditComponent implements OnInit {
  @Input()
  set orderCloudAddress(address: Address) {
    const routeUrl = this.router.routerState.snapshot.url;
    this.buyerID = routeUrl.split('/')[2];
    if (address.ID) {
      this.handleSelectedAddressChange(address);
    } else {
      this.createBuyerLocationForm(this.buyerLocationService.emptyResource);
    }
  }
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Input()
  suggestedAddresses: Array<BuyerAddress>;
  @Output()
  updateResource = new EventEmitter<ResourceUpdate>();
  @Output()
  isCreatingNew: boolean;
  buyerID: string;
  selectAddress = new EventEmitter<any>();
  buyerLocationEditable: MarketplaceBuyerLocation;
  buyerLocationStatic: MarketplaceBuyerLocation;
  areChanges = false;
  dataIsSaving = false;

  constructor(
    private buyerLocationService: BuyerLocationService,
    private router: Router,
    private middleware: MiddlewareAPIService,
    private currentUserService: CurrentUserService
  ) {}

  async refreshBuyerLocationData(buyerLocation: MarketplaceBuyerLocation) {
    this.buyerLocationEditable = buyerLocation;
    this.buyerLocationStatic = buyerLocation;
    this.createBuyerLocationForm(buyerLocation);
    this.isCreatingNew = this.buyerLocationService.checkIfCreatingNew();
    this.areChanges = this.buyerLocationService.checkForChanges(this.buyerLocationEditable, this.buyerLocationStatic);
  }

  ngOnInit(): void {
    this.isCreatingNew = this.buyerLocationService.checkIfCreatingNew();
  }

  createBuyerLocationForm(buyerLocation: MarketplaceBuyerLocation) {
    this.resourceForm = new FormGroup({
      ID: new FormControl(buyerLocation.Address.ID),
      LocationName: new FormControl(buyerLocation.UserGroup.Name, Validators.required),
      AddressName: new FormControl(buyerLocation.Address.AddressName, Validators.required),
      CompanyName: new FormControl(buyerLocation.Address.CompanyName, Validators.required),
      Street1: new FormControl(buyerLocation.Address.Street1, Validators.required),
      Street2: new FormControl(buyerLocation.Address.Street2),
      City: new FormControl(buyerLocation.Address.City, Validators.required),
      State: new FormControl(buyerLocation.Address.State, Validators.required),
      Zip: new FormControl(buyerLocation.Address.Zip, [Validators.required]),
      Country: new FormControl(buyerLocation.Address.Country, Validators.required),
      Phone: new FormControl(buyerLocation.Address.Phone, ValidatePhone),
      Email: new FormControl(buyerLocation.Address.xp.Email, ValidateEmail),
    });
  }

  updateResourceFromEvent(event: any, field: string): void {
    this.updateResource.emit({ value: event.target.value, field });
    this.areChanges = this.buyerLocationService.checkForChanges(this.buyerLocationEditable, this.buyerLocationStatic);
  }

  handleSelectedAddress(event: Address): void {
    const copiedResource = this.buyerLocationService.copyResource(this.buyerLocationEditable);
    copiedResource.Address = event;
    this.buyerLocationEditable = copiedResource;
    this.areChanges = this.buyerLocationService.checkForChanges(this.buyerLocationEditable, this.buyerLocationStatic);
  }

  getSaveBtnText(): string {
    return this.buyerLocationService.getSaveBtnText(
      this.dataIsSaving,
      this.isCreatingNew,
      this.suggestedAddresses?.length > 0
    );
  }

  async handleSave(): Promise<void> {
    if (this.isCreatingNew) {
      await this.createNewBuyerLocation();
    } else {
      this.updateBuyerLocation();
    }
  }

  async createNewBuyerLocation(): Promise<void> {
    try {
      this.dataIsSaving = true;
      this.buyerLocationEditable.UserGroup.xp.Type = 'BuyerLocation';
      if (!this.buyerLocationEditable.Address.ID)
        this.buyerLocationEditable.Address.ID = this.buyerLocationEditable.Address.AddressName.split(' ')
          .join('-')
          .replace(/[^a-zA-Z0-9 ]/g, '');
      this.buyerLocationEditable.UserGroup.ID = this.buyerLocationEditable.Address.ID;
      const newBuyerLocation = await MarketplaceSDK.BuyerLocations.Create(this.buyerID, this.buyerLocationEditable);
      this.refreshBuyerLocationData(newBuyerLocation);
      this.router.navigateByUrl(`/buyers/${this.buyerID}/locations/${newBuyerLocation.Address.ID}`);
      this.dataIsSaving = false;
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex.response.data);
      this.dataIsSaving = false;
    }
  }

  async updateBuyerLocation(): Promise<void> {
    try {
      this.dataIsSaving = true;
      const updatedBuyerLocation = await MarketplaceSDK.BuyerLocations.Update(
        this.buyerLocationEditable.Address.ID,
        this.buyerID,
        this.buyerLocationEditable
      );
      this.suggestedAddresses = null;
      this.buyerLocationEditable = updatedBuyerLocation;
      this.buyerLocationStatic = updatedBuyerLocation;
      this.areChanges = this.buyerLocationService.checkForChanges(this.buyerLocationEditable, this.buyerLocationStatic);
      this.dataIsSaving = false;
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex.response.data);
      this.dataIsSaving = false;
    }
  }

  async handleDelete($event): Promise<void> {
    await MarketplaceSDK.BuyerLocations.Delete(this.buyerLocationEditable.Address.ID, this.buyerID);
    this.router.navigateByUrl(`/buyers/${this.buyerID}/locations`);
  }

  updateBuyerLocationResource(buyerLocationUpdate: any) {
    const resourceToEdit = this.buyerLocationEditable || this.buyerLocationService.emptyResource;
    this.buyerLocationEditable = this.buyerLocationService.getUpdatedEditableResource(
      buyerLocationUpdate,
      resourceToEdit
    );
    this.areChanges = this.buyerLocationService.checkForChanges(this.buyerLocationEditable, this.buyerLocationStatic);
  }

  handleUpdateBuyerLocation(event: any, field: string) {
    const buyerLocationUpdate = {
      field,
      value: field === 'Active' ? event.target.checked : event.target.value,
    };
    this.updateBuyerLocationResource(buyerLocationUpdate);
  }

  handleDiscardChanges(): void {
    this.buyerLocationEditable = this.buyerLocationStatic;
    this.suggestedAddresses = null;
    this.areChanges = this.buyerLocationService.checkForChanges(this.buyerLocationEditable, this.buyerLocationStatic);
  }

  private async handleSelectedAddressChange(address: Address): Promise<void> {
    const marketplaceBuyerLocation = await MarketplaceSDK.BuyerLocations.Get(this.buyerID, address.ID);
    this.refreshBuyerLocationData(marketplaceBuyerLocation);
  }
}
