import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  templateUrl: `<ocm-profile-me-update [context]="context"></ocm-profile-me-update>
  `,
})
export class MeUpdateWrapperComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  ngOnInit() {}
}
