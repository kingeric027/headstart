import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { ListAddress, Address } from '@ordercloud/angular-sdk';
import { BuyerLocationService } from '../buyer-location.service';
import { MarketplaceBuyerLocation } from '@app-seller/shared/models/MarketplaceBuyerLocation.interface';
import { ValidatePhone, ValidateUSZip, ValidateEmail } from '@app-seller/validators/validators';
import { Router } from '@angular/router';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
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
  suggestedAddresses: ListAddress;
  @Output()
  updateResource = new EventEmitter<any>();
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
  private async handleSelectedAddressChange(address: Address): Promise<void> {
    const marketplaceBuyerLocation = await this.middleware.getBuyerLocationByID(this.buyerID, address.ID);
    this.refreshBuyerLocationData(marketplaceBuyerLocation);
  }

  async refreshBuyerLocationData(buyerLocation: MarketplaceBuyerLocation) {
    this.buyerLocationEditable = buyerLocation;
    this.buyerLocationStatic = buyerLocation;
    this.createBuyerLocationForm(buyerLocation);
    this.checkIfCreatingNew();
    this.checkForChanges();
  }

  private checkIfCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
    console.log(this.isCreatingNew);
  }

  checkForChanges(): void {
    this.areChanges = JSON.stringify(this.buyerLocationEditable) !== JSON.stringify(this.buyerLocationStatic);
  }

  ngOnInit(): void {
    this.checkIfCreatingNew();
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
      Zip: new FormControl(buyerLocation.Address.Zip, [Validators.required, ValidateUSZip]),
      Country: new FormControl(buyerLocation.Address.Country, Validators.required),
      Phone: new FormControl(buyerLocation.Address.Phone, ValidatePhone),
      Email: new FormControl(buyerLocation.Address.xp.Email, ValidateEmail),
    });
  }

  updateResourceFromEvent(event: any, field: string): void {
    this.updateResource.emit({ value: event.target.value, field });
    this.checkForChanges();
  }

  handleAddressSelect(address) {
    this.selectAddress.emit(address);
    this.checkForChanges();
  }

  getSaveBtnText(): string {
    if (this.dataIsSaving) return 'Saving...';
    if (this.isCreatingNew) return 'Create';
    if (!this.isCreatingNew) return 'Save Changes';
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
      const newBuyerLocation = await this.middleware.createBuyerLocation(this.buyerID, this.buyerLocationEditable);
      this.refreshBuyerLocationData(newBuyerLocation);
      this.router.navigateByUrl(`/buyers/${this.buyerID}/locations/${newBuyerLocation.Address.ID}`);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async updateBuyerLocation(): Promise<void> {
    try {
      this.dataIsSaving = true;
      console.log('edited buyer loc', this.buyerLocationEditable);
      const updatedBuyerLocation = await this.middleware.updateBuyerLocationByID(
        this.buyerID,
        this.buyerLocationEditable.Address.ID,
        this.buyerLocationEditable
      );
      this.buyerLocationEditable = updatedBuyerLocation;
      this.buyerLocationStatic = updatedBuyerLocation;
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async handleDelete($event): Promise<void> {
    await this.middleware.deleteBuyerLocation(this.buyerID, this.buyerLocationEditable.Address.ID);
    this.router.navigateByUrl(`/buyers/${this.buyerID}/locations`);
  }

  updateBuyerLocationResource(buyerLocationUpdate: any) {
    /* 
    * TODO:
    * This function is used to dynamically update deeply nested objects
    * It is currently used in two places, but will likely soon become
    * obsolete when the product edit component gets refactored.
    */
    console.log('buyer location update', buyerLocationUpdate);
    const piecesOfField = buyerLocationUpdate.field.split('.');
    const depthOfField = piecesOfField.length;
    const updateProductResourceCopy = this.copyProductResource(
      this.buyerLocationEditable || this.buyerLocationService.emptyResource
    );
    switch (depthOfField) {
      case 4:
        updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]][piecesOfField[3]] =
          buyerLocationUpdate.value;
        break;
      case 3:
        updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]] = buyerLocationUpdate.value;
        break;
      case 2:
        updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]] = buyerLocationUpdate.value;
        break;
      default:
        updateProductResourceCopy[piecesOfField[0]] = buyerLocationUpdate.value;
        break;
    }
    this.buyerLocationEditable = updateProductResourceCopy;
    this.checkForChanges();
  }

  handleUpdateBuyerLocation(event: any, field: string) {
    const buyerLocationUpdate = {
      field,
      value: field === 'Active' ? event.target.checked : event.target.value,
    };
    this.updateBuyerLocationResource(buyerLocationUpdate);
  }

  copyProductResource(product: any) {
    return JSON.parse(JSON.stringify(product));
  }

  handleDiscardChanges(): void {
    this.buyerLocationEditable = this.buyerLocationStatic;
  }
}
