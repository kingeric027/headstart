import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit, Inject, SimpleChanges } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup, FormControl } from '@angular/forms';
import { SupportedRates, SupportedCurrencies } from '@app-seller/shared/models/supported-rates.interface';
import { SupplierService } from '../supplier.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import {
  MiddlewareAPIService,
  SupplierFilterConfigDocument,
} from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ListPage, MarketplaceSupplier, HeadStartSDK, AssetUpload, Asset } from '@ordercloud/headstart-sdk';
import { HeaderComponent } from '@app-seller/layout/header/header.component';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AppAuthService } from '@app-seller/auth';
import { faTimes, faSpinner, faExclamationCircle, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';
import { User, OcSupplierUserService } from '@ordercloud/angular-sdk';
@Component({
  selector: 'app-supplier-edit',
  templateUrl: './supplier-edit.component.html',
  styleUrls: ['./supplier-edit.component.scss'],
})
export class SupplierEditComponent implements OnInit, OnChanges {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  @Output()
  logoStaged = new EventEmitter<File>();
  @Input() set supplierEditable(value: MarketplaceSupplier) {
    this._supplierEditable = value;
  }
  supplierUsers: ListPage<User>;
  _supplierEditable: MarketplaceSupplier;
  availableCurrencies: SupportedRates[] = [];
  isCreatingNew: boolean;
  countriesServicingOptions = [];
  countriesServicingForm:  FormGroup;
  hasLogo = false;
  logoUrl: string = "";
  stagedLogoUrl: SafeUrl = null;
  logoLoading = false;
  faTimes = faTimes;
  faSpinner = faSpinner;
  faExclamationCircle = faExclamationCircle;
  faTimesCircle = faTimesCircle;

  constructor(
    public supplierService: SupplierService,
    private currentUserService: CurrentUserService,
    private sanitizer: DomSanitizer,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private toastrService: ToastrService,
    private ocSupplierUserService: OcSupplierUserService
  ) {
    this.isCreatingNew = this.supplierService.checkIfCreatingNew();
  }

  async ngOnInit(): Promise<void> {
    this.availableCurrencies = (await HeadStartSDK.ExchangeRates.GetRateList()).Items;
    this.availableCurrencies = this.availableCurrencies.filter(c =>
      Object.values(SupportedCurrencies).includes(SupportedCurrencies[c.Currency])
    );
  }

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    if (changes?.supplierEditable?.currentValue?.ID !== changes?.supplierEditable?.previousValue?.ID) {
      await this.handleSelectedSupplierChange(changes.supplierEditable.currentValue);
    }
  }

  async handleSelectedSupplierChange(supplier: MarketplaceSupplier): Promise<void> {
    this.logoUrl = `${environment.middlewareUrl}/assets/${this.appConfig.sellerID}/Suppliers/${supplier.ID}/thumbnail?size=m`;
    !this.isCreatingNew && (this.hasLogo = (await await HeadStartSDK.Assets.ListAssets("Suppliers", this._supplierEditable?.ID, {filters: {Tags: ["Logo"]}})).Items?.length > 0);
    !this.isCreatingNew && (this.supplierUsers = await this.ocSupplierUserService.List(this._supplierEditable.ID).toPromise());
    this.setUpSupplierCountrySelectIfNeeded();
  }

  setUpSupplierCountrySelectIfNeeded(): void {
    const indexOfCountriesServicingConfig = this.filterConfig.Filters?.findIndex(
      s => s.Path === 'xp.CountriesServicing'
    );
    if (indexOfCountriesServicingConfig > -1) {
      this.countriesServicingOptions = this.filterConfig.Filters[indexOfCountriesServicingConfig].Items;
      const formGroupCountriesServicing = {};
      this.countriesServicingOptions.forEach(option => {
        formGroupCountriesServicing[option.Value] = new FormControl((this._supplierEditable as any).xp?.CountriesServicing?.includes(option.Value) || false);
      });
      this.countriesServicingForm = new FormGroup(formGroupCountriesServicing);
    }
  }

  updateCountriesServicing(event: any, country: string): void {
    const checked = event.target.checked;
    let newCountriesSupported = (this._supplierEditable as any).xp.CountriesServicing || [];
    const isCountryInExistingValue = newCountriesSupported.includes(country);
    if (checked && !isCountryInExistingValue) {
      newCountriesSupported = [...newCountriesSupported, country];
    } else if (!checked && isCountryInExistingValue) {
      newCountriesSupported = newCountriesSupported.filter(n => n !== country);
    }
    this.updateResource.emit({ value: newCountriesSupported, field: 'xp.CountriesServicing' });
  }

  updateResourceFromEvent(event: any, field: string): void {
    if (field.startsWith('xp.ProductTypes')) {
      const form = this.resourceForm.getRawValue();
      const valueToArray = [];
      Object.keys(form.ProductTypes).forEach(item => {
        if (form.ProductTypes[item]) valueToArray.push(item);
      });
      this.updateResource.emit({ field: 'xp.ProductTypes', value: valueToArray });
    } else {
      const value = ['Active', 'xp.SyncFreightPop'].includes(field) ? event.target.checked : event.target.value;
      this.updateResource.emit({ value, field });
    }
  }

  async dropFileUpload(event: FileHandle[]): Promise<void> {
    if (this.isCreatingNew) {
      this.logoStaged.emit(event[0].File);
      this.hasLogo = true;
      this.stagedLogoUrl = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(event[0].File));
    } else {
      this.logoLoading = true;
      try {
        await this.uploadAsset(this._supplierEditable?.ID, event[0].File, 'Image');
      } catch (err) {
        this.hasLogo = false;
        this.logoLoading = false;
        throw err;
      } finally {
        this.hasLogo = true;
        this.logoLoading = false;
        // Reset the img src for logo
        this.setLogoSrc();
      }
    }
  }

  // TODO: Some work to be done around 'isCreatingNew
  async manualFileUpload(event): Promise<void> {
    if (this.isCreatingNew) {
      this.logoStaged.emit(event?.target?.files[0]);
      this.hasLogo = true;
      this.stagedLogoUrl = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(event?.target?.files[0]));
    } else {
      this.logoLoading = true;
      const file: File = event?.target?.files[0];
      const logoAssets = await HeadStartSDK.Assets.ListAssets("Suppliers", this._supplierEditable?.ID, {filters: {Tags: ["Logo"]}});
      if (logoAssets?.Items?.length > 0) {
        // If logo exists, remove the assignment, then the logo itself
        await HeadStartSDK.Assets.DeleteAssetAssignment(logoAssets?.Items[0]?.ID, this._supplierEditable?.ID, "Suppliers", null, null);
        await HeadStartSDK.Assets.Delete(logoAssets.Items[0].ID);
      }
      // Then upload logo asset
      try {
        await this.uploadAsset(this._supplierEditable?.ID, file, 'Image');
      } catch (err) {
        this.hasLogo = false;
        this.logoLoading = false;
        throw err;
      } finally {
        this.hasLogo = true;
        this.logoLoading = false;
        // Reset the img src for logo
        this.setLogoSrc();
      }
    }
  }

  async uploadAsset(supplierID: string, file: File, assetType: string): Promise<void> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const asset: AssetUpload = {
      Active: true,
      File: file,
      Type: (assetType as AssetUpload['Type']),
      FileName: file.name,
      Tags: ["Logo"]
    }
    // Upload the asset, then make the asset assignment to Suppliers
    const newAsset: Asset = await HeadStartSDK.Upload.UploadAsset(asset, accessToken);
    await HeadStartSDK.Assets.SaveAssetAssignment({ResourceType: 'Suppliers', ResourceID: supplierID, AssetID: newAsset.ID }, accessToken);
  }

  async removeLogo(): Promise<void> {
    this.logoLoading = true;
    try {
      // Get the logo asset
      const logoAssets = await HeadStartSDK.Assets.ListAssets("Suppliers", this._supplierEditable?.ID, {filters: {Tags: ["Logo"]}});
      // Remove the logo asset assignment
      await HeadStartSDK.Assets.DeleteAssetAssignment(logoAssets?.Items[0]?.ID, this._supplierEditable?.ID, "Suppliers", null, null);
      // Remove the logo asset
      await HeadStartSDK.Assets.Delete(logoAssets.Items[0].ID);
    } catch (err) {
      throw err;
    } finally {
      this.hasLogo = false;
      this.logoLoading = false;
      // Reset the img src for logo
      this.setLogoSrc();
    }
  }

  setLogoSrc(): void {
    document.getElementById('supplier-logo')?.setAttribute('src', `${environment.middlewareUrl}/assets/${this.appConfig.sellerID}/Suppliers/${this._supplierEditable?.ID}/thumbnail?size=m`);
  }

  assignSupplierUser(email: string): void {
    const index = this._supplierEditable?.xp?.NotificationRcpts.indexOf(email);
    if (index !== -1) {
      this.removeAddtlRcpt(index);
      return;
    }
    const existingRcpts = this._supplierEditable?.xp?.NotificationRcpts || [];
    const constructedEvent = {target: {value: [...existingRcpts, email]}};
    this.updateResourceFromEvent(constructedEvent, 'xp.NotificationRcpts');
  }

  removeAddtlRcpt(index: number): void {
    const copiedResource = JSON.parse(JSON.stringify(this._supplierEditable));
    const editedArr = copiedResource.xp?.NotificationRcpts.filter(e => e !== copiedResource.xp?.NotificationRcpts[index]);
    this.updateResourceFromEvent({target: {value: editedArr}}, 'xp.NotificationRcpts');
  }

  isAssigned(email: string): boolean {
    return this._supplierEditable?.xp?.NotificationRcpts?.includes(email);
  }
}
