import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'auth-reset-password-wrapper',
  templateUrl: './reset-password-wrapper.component.html',
  styleUrls: ['./reset-password-wrapper.component.scss'],
})
export class ResetPasswordWrapperComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  ngOnInit() {}
}
