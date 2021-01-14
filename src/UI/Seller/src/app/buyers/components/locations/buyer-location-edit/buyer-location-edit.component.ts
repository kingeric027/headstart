import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core'
import { FormGroup, Validators, FormControl } from '@angular/forms'
import { Address, BuyerAddress } from '@ordercloud/angular-sdk'
import { BuyerLocationService } from '../buyer-location.service'
import { ValidatePhone, ValidateEmail } from '@app-seller/validators/validators'
import { Router } from '@angular/router'
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service'
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service'
import { getSuggestedAddresses } from '@app-seller/shared/services/address-suggestion.helper'
import { MarketplaceBuyerLocation } from 'marketplace-javascript-sdk/dist/models/MarketplaceBuyerLocation'
import {
  HeadStartSDK,
  HSCatalog,
  HSCatalogAssignmentRequest,
} from '@ordercloud/headstart-sdk'
import { GeographyConfig } from '@app-seller/shared/models/supported-countries.constant'
import { CatalogsTempService } from '@app-seller/shared/services/middleware-api/catalogs-temp.service'
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config'
import { ResourceUpdate } from '@app-seller/models/shared.types'
import { SupportedCountries } from '@app-seller/models/currency-geography.types'
@Component({
  selector: 'app-buyer-location-edit',
  templateUrl: './buyer-location-edit.component.html',
  styleUrls: ['./buyer-location-edit.component.scss'],
})
export class BuyerLocationEditComponent implements OnInit {
  @Input()
  set orderCloudAddress(address: Address) {
    const routeUrl = this.router.routerState.snapshot.url
    this.buyerID = routeUrl.split('/')[2]
    if (address.ID) {
      this.handleSelectedAddressChange(address)
    } else {
      this.createBuyerLocationForm(this.buyerLocationService.emptyResource)
    }
  }
  @Input()
  resourceForm: FormGroup
  @Input()
  filterConfig
  @Input()
  suggestedAddresses: Array<BuyerAddress>
  @Output()
  updateResource = new EventEmitter<ResourceUpdate>()
  @Output()
  isCreatingNew: boolean
  catalogAssignments: HSCatalogAssignmentRequest = { CatalogIDs: [] }
  buyerID: string
  selectAddress = new EventEmitter<any>()
  buyerLocationEditable: MarketplaceBuyerLocation
  buyerLocationStatic: MarketplaceBuyerLocation
  areChanges = false
  dataIsSaving = false
  countryOptions: SupportedCountries[]
  catalogs: HSCatalog[] = []

  constructor(
    private buyerLocationService: BuyerLocationService,
    private router: Router,
    private middleware: MiddlewareAPIService,
    private currentUserService: CurrentUserService,
    private hsCatalogService: CatalogsTempService
  ) {
    this.countryOptions = GeographyConfig.getCountries()
  }

  refreshBuyerLocationData(buyerLocation: MarketplaceBuyerLocation): void {
    this.buyerLocationEditable = buyerLocation
    this.buyerLocationStatic = buyerLocation
    this.catalogAssignments.CatalogIDs = this.buyerLocationEditable.UserGroup.xp.CatalogAssignments
    this.createBuyerLocationForm(buyerLocation)
    this.isCreatingNew = this.buyerLocationService.checkIfCreatingNew()
    this.areChanges = this.buyerLocationService.checkForChanges(
      this.buyerLocationEditable,
      this.buyerLocationStatic
    )
  }

  async ngOnInit(): Promise<void> {
    if (this.buyerID !== REDIRECT_TO_FIRST_PARENT) {
      await this.getCatalogs()
    }
    this.isCreatingNew = this.buyerLocationService.checkIfCreatingNew()
  }

  createBuyerLocationForm(buyerLocation: MarketplaceBuyerLocation): void {
    this.resourceForm = new FormGroup({
      ID: new FormControl(buyerLocation.Address.ID),
      LocationName: new FormControl(
        buyerLocation.UserGroup.Name,
        Validators.required
      ),
      AddressName: new FormControl(
        buyerLocation.Address.AddressName,
        Validators.required
      ),
      CompanyName: new FormControl(
        buyerLocation.Address.CompanyName,
        Validators.required
      ),
      Street1: new FormControl(
        buyerLocation.Address.Street1,
        Validators.required
      ),
      Street2: new FormControl(buyerLocation.Address.Street2),
      City: new FormControl(buyerLocation.Address.City, Validators.required),
      State: new FormControl(buyerLocation.Address.State, Validators.required),
      Zip: new FormControl(buyerLocation.Address.Zip, [Validators.required]),
      Country: new FormControl(
        buyerLocation.Address.Country,
        Validators.required
      ),
      Phone: new FormControl(buyerLocation.Address.Phone, ValidatePhone),
      Email: new FormControl(buyerLocation.Address.xp.Email, ValidateEmail),
      LocationID: new FormControl(buyerLocation.Address.xp.LocationID),
      Currency: new FormControl(
        buyerLocation.UserGroup.xp.Currency,
        Validators.required
      ),
      // TODO: remove this workaround when headstart sdk has been updated to include correct type
      BillingNumber: new FormControl(
        (buyerLocation.Address.xp as any).BillingNumber
      ),
    })
  }

  async getCatalogs(): Promise<void> {
    const catalogsResponse = await this.hsCatalogService.list(this.buyerID)
    this.catalogs = catalogsResponse.Items
  }

  handleSelectedAddress(event: Address): void {
    const copiedResource = this.buyerLocationService.copyResource(
      this.buyerLocationEditable
    )
    copiedResource.Address = event
    this.buyerLocationEditable = copiedResource
    this.areChanges = this.buyerLocationService.checkForChanges(
      this.buyerLocationEditable,
      this.buyerLocationStatic
    )
  }

  getSaveBtnText(): string {
    return this.buyerLocationService.getSaveBtnText(
      this.dataIsSaving,
      this.isCreatingNew,
      this.suggestedAddresses?.length > 0
    )
  }

  async handleSave(): Promise<void> {
    if (this.isCreatingNew) {
      await this.createNewBuyerLocation()
    } else {
      this.updateBuyerLocation()
    }
  }

  async createNewBuyerLocation(): Promise<void> {
    try {
      this.dataIsSaving = true
      this.buyerLocationEditable.UserGroup.xp.Type = 'BuyerLocation'
      this.buyerLocationEditable.UserGroup.ID = this.buyerLocationEditable.Address.ID
      ;(this.buyerLocationEditable.UserGroup
        .xp as any).Country = this.buyerLocationEditable.Address.Country
      const newBuyerLocation = await HeadStartSDK.BuyerLocations.Create(
        this.buyerID,
        this.buyerLocationEditable
      )
      if (this.isCreatingNew)
        await HeadStartSDK.Catalogs.SetAssignments(
          this.buyerID,
          newBuyerLocation.UserGroup.ID,
          this.catalogAssignments
        )
      this.refreshBuyerLocationData(newBuyerLocation)
      this.router.navigateByUrl(
        `/buyers/${this.buyerID}/locations/${newBuyerLocation.Address.ID}`
      )
      this.dataIsSaving = false
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex.response.data)
      this.dataIsSaving = false
    }
  }

  async updateBuyerLocation(): Promise<void> {
    try {
      this.dataIsSaving = true
      ;(this.buyerLocationEditable.UserGroup
        .xp as any).Country = this.buyerLocationEditable.Address.Country
      this.buyerLocationEditable.UserGroup.xp.CatalogAssignments = this.catalogAssignments?.CatalogIDs
      const updatedBuyerLocation = await HeadStartSDK.BuyerLocations.Save(
        this.buyerID,
        this.buyerLocationEditable.Address.ID,
        this.buyerLocationEditable
      )
      this.suggestedAddresses = null
      this.buyerLocationEditable = updatedBuyerLocation
      this.buyerLocationStatic = updatedBuyerLocation
      this.areChanges = this.buyerLocationService.checkForChanges(
        this.buyerLocationEditable,
        this.buyerLocationStatic
      )
      this.dataIsSaving = false
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex?.response?.data)
      this.dataIsSaving = false
    }
  }

  async handleDelete($event): Promise<void> {
    await HeadStartSDK.BuyerLocations.Delete(
      this.buyerID,
      this.buyerLocationEditable.Address.ID
    )
    this.router.navigateByUrl(`/buyers/${this.buyerID}/locations`)
  }

  updateBuyerLocationResource(buyerLocationUpdate: any): void {
    const resourceToEdit =
      this.buyerLocationEditable || this.buyerLocationService.emptyResource
    this.buyerLocationEditable = this.buyerLocationService.getUpdatedEditableResource(
      buyerLocationUpdate,
      resourceToEdit
    )
    this.areChanges = this.buyerLocationService.checkForChanges(
      this.buyerLocationEditable,
      this.buyerLocationStatic
    )
  }

  handleUpdateBuyerLocation(event: any, field: string): void {
    const buyerLocationUpdate = {
      field,
      value: field === 'Active' ? event.target.checked : event.target.value,
    }
    this.updateBuyerLocationResource(buyerLocationUpdate)
    if (field === 'Address.Country') {
      this.setCurrencyByCountry(buyerLocationUpdate.value)
    }
  }

  setCurrencyByCountry(countryCode: string): void {
    const selectedCountry = this.countryOptions.find(
      (country) => country.abbreviation === countryCode
    )
    this.resourceForm.controls.Currency.setValue(selectedCountry.currency)
    this.buyerLocationEditable.UserGroup.xp.Currency = this.resourceForm.value.Currency
  }

  handleDiscardChanges(): void {
    this.buyerLocationEditable = this.buyerLocationStatic
    this.suggestedAddresses = null
    this.areChanges = this.buyerLocationService.checkForChanges(
      this.buyerLocationEditable,
      this.buyerLocationStatic
    )
  }

  addCatalogAssignments(event): void {
    this.catalogAssignments = event
  }

  private async handleSelectedAddressChange(address: Address): Promise<void> {
    const hsBuyerLocation = await HeadStartSDK.BuyerLocations.Get(
      this.buyerID,
      address.ID
    )
    this.refreshBuyerLocationData(hsBuyerLocation)
  }
}
