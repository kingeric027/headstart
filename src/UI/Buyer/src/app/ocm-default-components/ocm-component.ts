import { Input } from '@angular/core';
import { CartActions } from '@app-buyer/shared';
import { Navigator } from '@app-buyer/shared/services/navigator/navigator.service';

export class OCMComponent {
  @Input() navigator: Navigator;
  @Input() cartActions: CartActions;
}
