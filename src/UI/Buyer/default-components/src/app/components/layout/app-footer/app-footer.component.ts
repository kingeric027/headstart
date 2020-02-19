import { Component } from '@angular/core';
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

  toTermsAndConditions(): void {
    this.context.router.toRoute('/terms-and-conditions');
  }

  toSupport(): void {
    this.context.router.toRoute('/support');
  }

  toFAQ(): void {
    this.context.router.toRoute('/faq');
  }
}
