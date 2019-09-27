import { Component, OnInit } from '@angular/core';
import { ProfileTab } from 'src/app/profile/models/profile-tabs.enum';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
@Component({
  selector: 'profile-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  selectedTab: string;
  tabs: ProfileTab[];

  constructor(private appAuthService: AuthService) {
    this.tabs = [
      { display: 'Details', route: ['/profile', 'details'] },
      { display: 'Change Password', route: ['/profile', 'change-password'] },
      { display: 'Addresses', route: ['/profile', 'addresses'] },
      { display: 'Payment Methods', route: ['/profile', 'payment-methods'] },
      { display: 'My Orders', route: ['/profile', 'orders'] },
      {
        display: 'Orders To Approve',
        route: ['/profile', 'orders', 'approval'],
      },
    ];
  }

  ngOnInit(): void {
    this.selectTab(this.tabs[0]);
  }

  selectTab(tab: ProfileTab): void {
    this.selectedTab = tab.display;
  }

  logout() {
    this.appAuthService.logout();
  }
}
