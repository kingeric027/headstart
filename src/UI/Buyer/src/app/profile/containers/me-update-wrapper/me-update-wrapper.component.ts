import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'profile-me-update-wrapper',
  templateUrl: './me-update-wrapper.component.html',
  styleUrls: ['./me-update-wrapper.component.scss'],
})
export class MeUpdateWrapperComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  ngOnInit() {}
}
