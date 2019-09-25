import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'auth-register-wrapper',
  templateUrl: './register-wrapper.component.html',
  styleUrls: ['./register-wrapper.component.scss'],
})
export class RegisterWrapperComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  ngOnInit() {}
}
