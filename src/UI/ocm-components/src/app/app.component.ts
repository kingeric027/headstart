import { Component } from '@angular/core';
import { MockProduct } from './models/mock-product';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'ocm-components';
  product = MockProduct;
}
