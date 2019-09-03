import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MeUser, OcMeService, OcTokenService, User } from '@ordercloud/angular-sdk';
import * as jwtDecode from 'jwt-decode';
import { isUndefined as _isUndefined } from 'lodash';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  private readonly MaxFavorites: number = 40;
  private readonly favOrders = 'FavoriteOrders';
  private readonly favProducts = 'FavoriteProducts';

  public userSubject: BehaviorSubject<MeUser> = new BehaviorSubject<MeUser>(null);
  public isAnonSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private ocMeService: OcMeService, private ocTokenService: OcTokenService, private toastrService: ToastrService) {}

  async reset(): Promise<void> {
    const isAnon = !_isUndefined(this.getOrderIDFromToken());
    this.isAnonSubject.next(isAnon);
    const user = await this.ocMeService.Get().toPromise();
    this.userSubject.next(user);
  }

  getOrderIDFromToken(): string | void {
    return jwtDecode(this.ocTokenService.GetAccess()).orderid;
  }

  get loggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  set loggedIn(value: boolean) {
    this.loggedInSubject.next(value);
  }

  get isAnon(): boolean {
    return this.isAnonSubject.value;
  }

  set isAnon(value: boolean) {
    this.isAnonSubject.next(value);
  }

  get user(): User {
    return this.userSubject.value;
  }

  set user(value: User) {
    this.userSubject.next(value);
  }

  get favoriteProductIDs(): string[] {
    const me = this.userSubject.value;
    return me && me.xp && me.xp[this.favProducts] instanceof Array ? me.xp[this.favProducts] : [];
  }

  get favoriteOrderIDs(): string[] {
    const me = this.userSubject.value;
    return me && me.xp && me.xp[this.favOrders] instanceof Array ? me.xp[this.favOrders] : [];
  }

  setIsFavoriteProduct(isFav: boolean, productID: string) {
    this.setFavoriteValue(this.favProducts, isFav, productID);
  }

  setIsFavoriteOrder(isFav: boolean, orderID: string) {
    this.setFavoriteValue(this.favOrders, isFav, orderID);
  }

  private async setFavoriteValue(XpFieldName: string, isFav: boolean, ID: string): Promise<void> {
    const favorites = this.user.xp[XpFieldName];
    let updatedFavorites: string[];
    if (isFav && favorites.length >= this.MaxFavorites) {
      this.toastrService.info(`You have reached your limit of ${XpFieldName}`);
      return;
    }
    if (isFav) {
      updatedFavorites = [...favorites, ID];
    } else {
      updatedFavorites = favorites.filter((x) => x !== ID);
    }
    const request = { xp: { [XpFieldName]: updatedFavorites } };
    this.user = await this.ocMeService.Patch(request).toPromise();
  }
}
