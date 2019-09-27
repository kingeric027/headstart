import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'profile-change-password-wrapper',
  templateUrl: './me-change-password-wrapper.component.html',
  styleUrls: ['./me-change-password-wrapper.component.scss'],
})
export class MeChangePasswordWrapperComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  ngOnInit() {}
}
