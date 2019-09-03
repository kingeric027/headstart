import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { faCube, faTruck } from '@fortawesome/free-solid-svg-icons';
import { Order } from '@ordercloud/angular-sdk';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';

@Component({
  selector: 'order-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss'],
})
export class OrderComponent implements OnInit {
  faCube = faCube;
  faTruck = faTruck;
  approvalVersion: boolean;
  order$: Observable<Order>;

  constructor(private activatedRoute: ActivatedRoute, private router: Router, protected currentUser: CurrentUserService) {}

  ngOnInit() {
    this.approvalVersion = this.router.url.includes('/approval');
    this.order$ = this.activatedRoute.data.pipe(map(({ orderResolve }) => orderResolve.order));
  }

  isFavorite(orderID: string): boolean {
    return this.currentUser.favoriteOrderIDs.includes(orderID);
  }

  toggleFavorite(order: Order) {
    const newValue = !this.isFavorite(order.ID);
    this.currentUser.setIsFavoriteOrder(newValue, order.ID);
  }
}
