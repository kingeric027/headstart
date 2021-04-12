import { Router, ActivatedRoute } from '@angular/router'
import {
  AfterViewChecked,
  ChangeDetectorRef,
  OnInit,
  Inject,
} from '@angular/core'
import { getPsHeight } from '@app-seller/shared/services/dom.helper'
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service'
import { applicationConfiguration } from '@app-seller/config/app.config'
import { environment } from 'src/environments/environment.local'
import { MeUser } from '@ordercloud/angular-sdk'
import { FormGroup, FormControl } from '@angular/forms'
import { isEqual as _isEqual, set as _set, get as _get } from 'lodash'
import { Asset, AssetUpload } from '@ordercloud/headstart-sdk'
import { JDocument } from '@ordercloud/cms-sdk'
import { AppAuthService } from '@app-seller/auth/services/app-auth.service'
import { ContentManagementClient } from '@ordercloud/cms-sdk'
import { UserContext } from '@app-seller/models/user.types'
import { AppConfig } from '@app-seller/models/environment.types'
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service'
import { getImageIDFromUrl } from '@app-seller/shared/services/image.helper'

export abstract class AccountContent implements AfterViewChecked, OnInit {
  activePage: string
  currentUserInitials: string
  hasProfileImg = false
  contentHeight: number
  userContext: UserContext
  myProfileImg: string
  profileImgLoading = false
  organizationName: string
  areChanges: boolean
  user: MeUser
  userForm: FormGroup
  userStatic: MeUser
  userEditable: MeUser
  notificationsToReview: JDocument[]

  constructor(
    private router: Router,
    activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private appAuthService: AppAuthService,
    private currentUserService: CurrentUserService,
    private middleware: MiddlewareAPIService,
  ) {
    this.setUpSubs()
  }

  ngAfterViewChecked(): void {
    this.contentHeight = getPsHeight(/* No additional class to pass */)
    this.changeDetectorRef.detectChanges()
  }

  async ngOnInit(): Promise<void> {
    await this.setUser()
    this.userContext.Me.Supplier
      ? this.getSupplierOrg()
      : (this.organizationName = this.appConfig.sellerName)
    this.refresh(this.userContext.Me)
  }

  setUpSubs(): void {
    this.currentUserService.userSubject.subscribe((user) => {
      this.user = user
      this.setCurrentUserInitials(this.user)
      this.hasProfileImg = user.xp?.Image?.ThumbnailUrl && user.xp?.Image?.ThumbnailUrl !== '' 
    })
    // this.currentUserService.profileImgSubject.subscribe((img) => {
    //   this.hasProfileImg = Object.keys(img).length > 0
    // })
  }

  setCurrentUserInitials(user: MeUser): void {
    const firstFirst = user?.FirstName?.substr(0, 1)
    const firstLast = user?.LastName?.substr(0, 1)
    this.currentUserInitials = `${firstFirst}${firstLast}`
  }

  async getSupplierOrg(): Promise<void> {
    const mySupplier = await this.currentUserService.getMySupplier()
    this.organizationName = mySupplier.Name
  }

  async setUser(): Promise<void> {
    this.userContext = await this.currentUserService.getUserContext()
  }

  refresh(user: MeUser): void {
    this.userStatic = this.copyResource(user)
    this.userEditable = this.copyResource(user)
    this.createUserForm(user)
    this.areChanges = this.checkIfAreChanges(this.userStatic, this.userEditable)
  }

  navigateToPage(route: string) {
    this.router.navigateByUrl(route)
  }

  createUserForm(me: MeUser): void {
    this.userForm = new FormGroup({
      FirstName: new FormControl(me.FirstName),
      LastName: new FormControl(me.LastName),
      Email: new FormControl({ value: me.Email, disabled: true }),
      Username: new FormControl(me.Username),
      RequestInfoEmails: new FormControl(_get(me, 'xp.RequestInfoEmails')),
      OrderEmails: new FormControl(_get(me, 'xp.OrderEmails')),
      ProductEmails: new FormControl(_get(me, 'xp.ProductEmails')),
      AddtlRcpts: new FormControl(_get(me, 'xp.AddtlRcpts')),
    })
  }

  checkIfAreChanges = <T>(_static: T, _editable: T): boolean =>
    !_isEqual(_editable, _static)

  updateUserFromEvent(event: any, field: string): void {
    const value = event.target.value
    const userUpdate = { field, value }
    const updateUserCopy: MeUser = JSON.parse(JSON.stringify(this.userEditable))
    this.userEditable = _set(updateUserCopy, userUpdate.field, userUpdate.value)
    this.areChanges = this.checkIfAreChanges(this.userStatic, this.userEditable)
  }

  copyResource<T>(resource: T): T {
    return JSON.parse(JSON.stringify(resource))
  }

  discardChanges(): void {
    this.refresh(this.userStatic)
    this.areChanges = this.checkIfAreChanges(this.userStatic, this.userEditable)
  }

  async patchUser(fieldsToPatch: string[]): Promise<void> {
    const patch: any = {}
    fieldsToPatch.map((f) => {
      patch[f] = this.userEditable[f]
    })
    const patchedUser = await this.currentUserService.patchUser(patch)
    this.refresh(patchedUser)
  }

  async manualFileUpload(event): Promise<void> {
    this.profileImgLoading = true
    const file: File = event?.target?.files[0]
    if(this.user?.xp?.Image?.Url) {
      await this.middleware.deleteImage(getImageIDFromUrl(this.user.xp.Image.Url))
    }
    try {
      const data = new FormData()
      data.append('File', file)
      const imgUrls = await this.middleware.uploadImage(data)
      const patchObj = {
        xp: {
          Image: imgUrls
        }
      }
      await this.currentUserService.patchUser(patchObj);
    } catch (err) {
      this.hasProfileImg = false
      this.profileImgLoading = false
      throw err
    } finally {
      this.hasProfileImg = true
      this.profileImgLoading = false
    }
  }

  async uploadProfileImg(userID: string, file: File): Promise<Asset> {
    const accessToken = await this.appAuthService.fetchToken().toPromise()
    const asset: AssetUpload = {
      Active: true,
      File: file,
      FileName: file.name,
      Tags: ['ProfileImg'],
    }
    // Upload the asset, then make the asset assignment to Suppliers
    const newAsset: Asset = await ContentManagementClient.Assets.Upload(
      asset,
      accessToken
    )
    if (this.userContext.UserType === 'SELLER') {
      await ContentManagementClient.Assets.SaveAssetAssignment(
        {
          ResourceType: 'AdminUsers',
          ResourceID: userID,
          AssetID: newAsset.ID,
        },
        accessToken
      )
    } else {
      await ContentManagementClient.Assets.SaveAssetAssignment(
        {
          ParentResourceType: 'Suppliers',
          ParentResourceID: this.userContext.Me.Supplier.ID,
          ResourceType: 'SupplierUsers',
          ResourceID: userID,
          AssetID: newAsset.ID,
        },
        accessToken
      )
    }
    return newAsset
  }

  async removeProfileImg(): Promise<void> {
    this.profileImgLoading = true
    try {
      await this.middleware.deleteImage(getImageIDFromUrl(this.user?.xp?.Image?.Url))
      const patchObj = {
        xp: {
          Image: {}
        }
      }
      await this.currentUserService.patchUser(patchObj)
    } catch (err) {
      throw err
    } finally {
      this.hasProfileImg = false
      this.profileImgLoading = false
    }
  }
}
