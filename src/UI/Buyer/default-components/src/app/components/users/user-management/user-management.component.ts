import { Component } from '@angular/core';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class OCMUserManagement {
  constructor(private context: ShopperContextService) {}
}
