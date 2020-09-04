import { Router, ActivatedRoute } from '@angular/router';
import { AfterViewChecked, ChangeDetectorRef, OnInit, Inject } from '@angular/core';
import { getPsHeight } from '@app-seller/shared/services/dom.helper';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import { environment } from 'src/environments/environment';
import { UserContext } from '@app-seller/config/user-context';
import { MeUser, OcAdminUserService, OcSupplierUserService, ListPage } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { isEqual as _isEqual, set as _set, get as _get } from 'lodash';
import { HeadStartSDK, Asset, AssetUpload } from '@ordercloud/headstart-sdk';
import { AppAuthService } from '@app-seller/auth';

export abstract class AccountContent implements AfterViewChecked, OnInit {
  activePage: string;
  hasProfileImg: boolean = false;
  contentHeight: number;
  userContext: UserContext;
  myProfileImg: string;
  profileImgLoading: boolean = false;
  organizationName: string;
  areChanges: boolean;
  userForm: FormGroup;
  userStatic: MeUser;
  userEditable: MeUser;
  constructor(
    private router: Router,
    activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef,
    private currentUserService: CurrentUserService,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private appAuthService: AppAuthService
  ) {}

  ngAfterViewChecked(): void {
    this.contentHeight = getPsHeight(/* No additional class to pass */);
    this.changeDetectorRef.detectChanges();
  }

  async ngOnInit(): Promise<void> {
      await this.setUser();
      this.userContext.Me.Supplier ? this.getSupplierOrg() : (this.organizationName = this.appConfig.sellerName);
      this.refresh(this.userContext.Me);
      const commerceRole = this.userContext.UserType;
      if (commerceRole === 'SELLER') {
        this.hasProfileImg = (await await HeadStartSDK.Assets.ListAssets("AdminUsers", this.userContext.Me.ID, {filters: {Tags: ["ProfileImg"]}})).Items?.length > 0;
        this.myProfileImg = `${environment.middlewareUrl}/assets/${environment.sellerID}/AdminUsers/${this.userContext.Me.ID}/thumbnail?size=s`;
    } else {
      this.hasProfileImg = (await await HeadStartSDK.Assets.ListAssetsOnChild("Suppliers", this.userContext.Me.Supplier.ID, "SupplierUsers", this.userContext.Me?.ID, {filters: {Tags: ["ProfileImg"]}})).Items?.length > 0;
      this.myProfileImg = `${environment.middlewareUrl}/assets/${environment.sellerID}/Suppliers/${
        this.userContext.Me.Supplier.ID
      }/SupplierUsers/${this.userContext.Me.ID}/thumbnail?size=s`;
    }
  }

  async getSupplierOrg() {
    const mySupplier = await this.currentUserService.getMySupplier();
    this.organizationName = mySupplier.Name;
  }

  async setUser(): Promise<void> {
    this.userContext = await this.currentUserService.getUserContext();
  }

  refresh(user: MeUser): void {
    this.userStatic = this.copyResource(user);
    this.userEditable = this.copyResource(user);
    this.createUserForm(user);
    this.checkForChanges();
  }

  createUserForm(me: MeUser): void {
    this.userForm = new FormGroup({
      FirstName: new FormControl(me.FirstName),
      LastName: new FormControl(me.LastName),
      Email: new FormControl({ value: me.Email, disabled: true }),
      Username: new FormControl(me.Username),
      OrderEmails: new FormControl(_get(me, 'xp.OrderEmails')),
      AddtlRcpts: new FormControl(_get(me, 'xp.AddtlRcpts')),
    });
  }

  checkForChanges(): void {
    this.areChanges = !_isEqual(this.userEditable, this.userStatic);
  }

  updateUserFromEvent(event: any, field: string): void {
    const value = event.target.value;
    const userUpdate = { field, value };
    const updateUserCopy: MeUser = JSON.parse(JSON.stringify(this.userEditable));
    this.userEditable = _set(updateUserCopy, userUpdate.field, userUpdate.value);
    this.checkForChanges();
  }

  copyResource<T>(resource: T): T {
    return JSON.parse(JSON.stringify(resource));
  }

  discardChanges(): void {
    this.refresh(this.userStatic);
    this.checkForChanges();
  }

  async patchUser(fieldsToPatch: string[]): Promise<void> {
    let patch: any = {};
    fieldsToPatch.map(f => {
      patch[f] = this.userEditable[f];
    });
    const patchedUser = await this.currentUserService.patchUser(patch);
    this.refresh(patchedUser);
  }

  async manualFileUpload(event): Promise<void> {
      this.profileImgLoading = true;
      const file: File = event?.target?.files[0];
      let profileImgAssets: ListPage<Asset>;
      if (this.userContext.UserType === 'SELLER') {
        // seller stuff
        profileImgAssets = await HeadStartSDK.Assets.ListAssets("AdminUsers", this.userContext.Me.ID, {filters: {Tags: ["ProfileImg"]}});
        if (profileImgAssets?.Items?.length > 0) {
          // If logo exists, remove the assignment, then the logo itself
          await HeadStartSDK.Assets.DeleteAssetAssignment(profileImgAssets?.Items[0]?.ID, this.userContext?.Me?.ID, "AdminUsers", null, null);
          await HeadStartSDK.Assets.Delete(profileImgAssets.Items[0].ID);
        }
    } else {
        // supplier stuff
        profileImgAssets = await HeadStartSDK.Assets.ListAssetsOnChild("Suppliers", this.userContext.Me.Supplier.ID, "SupplierUsers", this.userContext.Me?.ID, {filters: {Tags: ["ProfileImg"]}});
        if (profileImgAssets?.Items?.length > 0) {
          // If logo exists, remove the assignment, then the logo itself
          await HeadStartSDK.Assets.DeleteAssetAssignment(profileImgAssets?.Items[0]?.ID, this.userContext?.Me?.ID, "SupplierUsers", this.userContext?.Me?.Supplier?.ID, "Suppliers");
          await HeadStartSDK.Assets.Delete(profileImgAssets.Items[0].ID);
        }
      }
      // Then upload logo asset
      try {
        await this.uploadProfileImg(this.userContext?.Me?.ID, file, 'Image');
      } catch (err) {
        this.hasProfileImg = false;
        this.profileImgLoading = false;
        throw err;
      } finally {
        this.hasProfileImg = true;
        this.profileImgLoading = false;
        // Reset the img src for profileImg
        this.setProfileImgSrc();
      }
  }

  async uploadProfileImg(userID: string, file: File, assetType: string): Promise<void> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const asset: AssetUpload = {
      Active: true,
      File: file,
      Type: (assetType as AssetUpload['Type']),
      FileName: file.name,
      Tags: ["ProfileImg"]
    }
    // Upload the asset, then make the asset assignment to Suppliers
    const newAsset: Asset = await HeadStartSDK.Upload.UploadAsset(asset, accessToken);
    if (this.userContext.UserType === 'SELLER') {
        await HeadStartSDK.Assets.SaveAssetAssignment({ResourceType: 'AdminUsers', ResourceID: userID, AssetID: newAsset.ID }, accessToken);
    } else {
        await HeadStartSDK.Assets.SaveAssetAssignment({ParentResourceType: 'Suppliers', ParentResourceID: this.userContext.Me.Supplier.ID, ResourceType: "SupplierUsers", ResourceID: userID, AssetID: newAsset.ID }, accessToken);
    }
  }

  async removeProfileImg(): Promise<void> {
    this.profileImgLoading = true;
    try {
      let profileImgAssets: ListPage<Asset>;
      if (this.userContext.UserType === 'SELLER') {
          // Get the profile img asset
          profileImgAssets = await HeadStartSDK.Assets.ListAssets("AdminUsers", this.userContext.Me.ID, {filters: {Tags: ["ProfileImg"]}});
          // Remove the profile img asset assignment
          await HeadStartSDK.Assets.DeleteAssetAssignment(profileImgAssets?.Items[0]?.ID, this.userContext?.Me?.ID, "AdminUsers", null, null);
          // Remove the profile img asset
          await HeadStartSDK.Assets.Delete(profileImgAssets.Items[0].ID);
      } else {
         // Get the profile img asset
         profileImgAssets = await HeadStartSDK.Assets.ListAssetsOnChild("Suppliers", this.userContext.Me.Supplier.ID, "SupplierUsers", this.userContext.Me?.ID, {filters: {Tags: ["ProfileImg"]}});
        // Remove the profile img asset assignment
        await HeadStartSDK.Assets.DeleteAssetAssignment(profileImgAssets?.Items[0]?.ID, this.userContext?.Me?.ID, "SupplierUsers", this.userContext?.Me?.Supplier?.ID, "Suppliers");
        // Remove the profile img asset
        await HeadStartSDK.Assets.Delete(profileImgAssets.Items[0].ID);
      }
    } catch (err) {
      throw err;
    } finally {
      this.hasProfileImg = false;
      this.profileImgLoading = false;
      // Reset the img src for logo
      this.setProfileImgSrc();
    }
  }

  setProfileImgSrc(): void {
    if (this.userContext.UserType === 'SELLER') {
        const url = `${environment.middlewareUrl}/assets/${this.appConfig.sellerID}/AdminUsers/${this.userContext.Me.ID}/thumbnail?size=m`;
        this.myProfileImg = url;
        document.getElementById('ProfileImgThumb')?.setAttribute('src', url);
        document.getElementById('ProfileImg')?.setAttribute('src', url);
    } else {
        const url = `${environment.middlewareUrl}/assets/${this.appConfig.sellerID}/Suppliers/${this.userContext.Me.Supplier.ID}/SupplierUsers/${this.userContext.Me.ID}/thumbnail?size=m`;
        this.myProfileImg = url;
        document.getElementById('ProfileImgThumb')?.setAttribute('src', url);
        document.getElementById('ProfileImg')?.setAttribute('src', url);
    }
  }
}
