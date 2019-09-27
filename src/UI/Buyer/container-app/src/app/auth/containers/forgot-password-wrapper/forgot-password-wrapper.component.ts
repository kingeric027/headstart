import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'auth-forgot-password-wrapper',
  templateUrl: './forgot-password-wrapper.component.html',
  styleUrls: ['./forgot-password-wrapper.component.scss'],
})
export class ForgotPasswordWrapperComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  ngOnInit() {}
}
