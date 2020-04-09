import { Component, ViewEncapsulation } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { ProfileSection } from 'marketplace/projects/marketplace/src/lib/services/route/profile-routing.config';

@Component({
  templateUrl: './profile-nav.component.html',
  styleUrls: ['./profile-nav.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class OCMProfileNav {
  profileSections: ProfileSection[] = [];

  constructor(public context: ShopperContextService) {
    this.profileSections = context.router.getProfileSections()
  }
}
