import { Component, Input } from '@angular/core';
import { faPhone, faQuestionCircle, faFileAlt } from '@fortawesome/free-solid-svg-icons';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './app-footer.component.html',
  styleUrls: ['./app-footer.component.scss'],
})
export class OCMAppFooter {
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faFileAlt = faFileAlt;

  constructor(private context: ShopperContextService) {}

  toTermsAndConditions() {
    this.context.router.toRoute('/terms-and-conditions');
  }

  toSupport() {
    this.context.router.toRoute('/support');
  }

  toFAQ() {
    this.context.router.toRoute('/faq');
  }
}
