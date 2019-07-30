import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { MeUser, Order, ListLineItem, LineItem } from '@ordercloud/angular-sdk';

@Injectable({
  providedIn: 'root',
})
export class AppStateService {
  // Documentation on BehaviorSubject http://reactivex.io/rxjs/manual/overview.html#behaviorsubject
  public userSubject: BehaviorSubject<MeUser>;
  public orderSubject: BehaviorSubject<Order>;
  public isAnonSubject: BehaviorSubject<boolean>;
  public isLoggedIn: BehaviorSubject<boolean>;
  public lineItemSubject: BehaviorSubject<ListLineItem>;

  public addToCartSubject: Subject<LineItem>;

  constructor() {
    this.userSubject = new BehaviorSubject<MeUser>(null);
    this.orderSubject = new BehaviorSubject<Order>(null);
    this.isAnonSubject = new BehaviorSubject<boolean>(true);
    this.isLoggedIn = new BehaviorSubject<boolean>(false);
    this.lineItemSubject = new BehaviorSubject<ListLineItem>({
      Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
      Items: [],
    });

    this.addToCartSubject = new Subject<LineItem>();
  }
}
