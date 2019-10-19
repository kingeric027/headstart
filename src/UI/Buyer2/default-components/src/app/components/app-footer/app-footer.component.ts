import { Component, Input } from '@angular/core';
import { faPhone, faQuestionCircle, faFileAlt } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './app-footer.component.html',
  styleUrls: ['./app-footer.component.scss'],
})
export class OCMAppFooter extends OCMComponent {
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faFileAlt = faFileAlt;
  @Input() showFooter: boolean; // TODO - find a way to remove this

  ngOnContextSet() {}

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
