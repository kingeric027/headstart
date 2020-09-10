import { Component, AfterViewChecked, ChangeDetectorRef, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { getPsHeight } from '@app-seller/shared/services/dom.helper';
import { AccountContent } from '@app-seller/shared/components/account-content/account-content.component';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import { MeUser, OcAdminUserService, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { AppAuthService } from '@app-seller/auth';

@Component({
  selector: 'account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent extends AccountContent {
  userStatic: MeUser;
  userEditable: MeUser;
  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    changeDetectorRef: ChangeDetectorRef,
    currentUserService: CurrentUserService,
    @Inject(applicationConfiguration) appConfig: AppConfig,
    appAuthService: AppAuthService
  ) {
    super(router, activatedRoute, changeDetectorRef, currentUserService, appConfig, appAuthService);
  }
}
