import { Component, OnInit } from '@angular/core';
import { MeUser } from '@ordercloud/angular-sdk';
import { ToastrService } from 'ngx-toastr';
import { ShopperContextService } from 'marketplace';
import { faEdit, faUser, faPhone, faEnvelope } from '@fortawesome/free-solid-svg-icons';
@Component({
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class OCMProfile implements OnInit {
  me: MeUser;
  alive = true;
  faEdit = faEdit;
  faUser = faUser;
  faPhone = faPhone;
  faEnvelope = faEnvelope;
  showEditProfileForm = false;

  constructor(private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit() {
    this.context.currentUser.onUserChange(this.handleUserChange);
  }

  showEditProfile() {
    this.showEditProfileForm = true;
  }

  dismissProfileEditForm() {
    this.showEditProfileForm = false;
  }

  handleUserChange = (user: MeUser) => {
    if (!user) return;
    this.me = user;
  };

  async profileFormSubmitted(me: MeUser) {
    this.showEditProfileForm = false;
    await this.context.currentUser.patch(me);
    this.toasterService.success(`Profile Updated`);
  }
}
