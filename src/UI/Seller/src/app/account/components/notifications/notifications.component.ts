import { Component, ChangeDetectorRef, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountContent } from '@app-seller/shared/components/account-content/account-content.component';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcAdminUserService, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { faExclamationCircle, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { AppAuthService } from '@app-seller/auth';

@Component({
  selector: 'account-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
})
export class NotificationsComponent extends AccountContent {
  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    changeDetectorRef: ChangeDetectorRef,
    currentUserService: CurrentUserService,
    @Inject(applicationConfiguration) appConfig: AppConfig,
    appAuthService: AppAuthService,
    private toastrService: ToastrService,
  ) {
    super(router, activatedRoute, changeDetectorRef, currentUserService, appConfig, appAuthService);
  }
  faExclamationCircle = faExclamationCircle;
  faTimesCircle = faTimesCircle;

  toggleOrderEmails(value: boolean): void {
    if (value === false) {
      this.updateUserFromEvent({target: {value: []}}, 'xp.AddtlRcpts');
    }
    this.updateUserFromEvent({target: {value: value}}, 'xp.OrderEmails')
  }

  addAddtlRcpt() {
    const addtlEmail = (document.getElementById('AdditionalEmail') as any).value;
    const isValid = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,24}$/.test(addtlEmail);
    if (!isValid) {
      this.toastrService.error(`"${addtlEmail}" is not a valid email.`)
    } else if (addtlEmail === this.userEditable.Email && isValid) {
      this.toastrService.info("Your account email cannot be added to the list of additional recipients.")
    } else {
      const existingRcpts = this.userEditable?.xp?.AddtlRcpts || [];
      const constructedEvent = {target: {value: [addtlEmail, ...existingRcpts]}};
      this.updateUserFromEvent(constructedEvent, 'xp.AddtlRcpts');
    }
    (document.getElementById('AdditionalEmail') as any).value = null;
    document.getElementById('AdditionalEmail').focus();
  }
  
  removeAddtlRcpt(index: number): void {
    const copiedResource = this.copyResource(this.userEditable);
    const editedArr = copiedResource.xp?.AddtlRcpts.filter(e => e !== copiedResource.xp?.AddtlRcpts[index]);
    this.updateUserFromEvent({target: {value: editedArr}}, 'xp.AddtlRcpts');
  }
}