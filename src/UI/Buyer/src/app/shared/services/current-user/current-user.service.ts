import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MeUser, OcMeService, OcTokenService, User } from '@ordercloud/angular-sdk';
import * as jwtDecode from 'jwt-decode';
import { isUndefined as _isUndefined } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { ICurrentUser } from 'src/app/ocm-default-components/shopper-context';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService implements ICurrentUser {
  private readonly MaxFavorites: number = 40;
  private readonly favOrdersXP = 'FavoriteOrders';
  private readonly favProductsXP = 'FavoriteProducts';

  private userSubject: BehaviorSubject<MeUser> = new BehaviorSubject<MeUser>(null);
  private isAnonSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  private loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  // todo - this data is in user also. remove duplicate?
  private favoriteOrdersSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  private favoriteProductsSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);

  constructor(private ocMeService: OcMeService, private ocTokenService: OcTokenService, private toastrService: ToastrService) {}

  async reset(): Promise<void> {
    this.isAnonymous = !_isUndefined(this.getOrderIDFromToken());
    this.user = await this.ocMeService.Get().toPromise();
    this.isLoggedIn = this.user.Active;
  }

  get isLoggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  set isLoggedIn(value: boolean) {
    this.loggedInSubject.next(value);
  }

  onIsLoggedInChange(callback: (isLoggedIn: boolean) => void): void {
    this.loggedInSubject.subscribe(callback);
  }

  get isAnonymous(): boolean {
    return this.isAnonSubject.value;
  }

  set isAnonymous(value: boolean) {
    this.isAnonSubject.next(value);
  }

  onIsAnonymousChange(callback: (isAnonymous: boolean) => void): void {
    this.isAnonSubject.subscribe(callback);
  }

  get user(): User {
    return this.userSubject.value;
  }

  set user(value: User) {
    this.favoriteOrdersSubject.next(this.getFavorites(value, this.favOrdersXP));
    this.favoriteProductsSubject.next(this.getFavorites(value, this.favProductsXP));
    this.userSubject.next(value);
  }

  onUserChange(callback: (user: User) => void) {
    this.userSubject.subscribe(callback);
  }

  get favoriteProductIDs(): string[] {
    return this.favoriteProductsSubject.value;
  }

  setIsFavoriteProduct(isFav: boolean, productID: string) {
    this.setFavoriteValue(this.favProductsXP, isFav, productID);
  }

  onFavoriteProductsChange(callback: (productIDs: string[]) => void) {
    this.favoriteProductsSubject.subscribe(callback);
  }

  get favoriteOrderIDs(): string[] {
    return this.favoriteOrdersSubject.value;
  }

  setIsFavoriteOrder(isFav: boolean, orderID: string) {
    this.setFavoriteValue(this.favOrdersXP, isFav, orderID);
  }

  onFavoriteOrdersChange(callback: (orderIDs: string[]) => void) {
    this.favoriteOrdersSubject.subscribe(callback);
  }

  private getFavorites(user: User, XpFieldName: string): string[] {
    return user && user.xp && user.xp[XpFieldName] instanceof Array ? user.xp[XpFieldName] : [];
  }

  private async setFavoriteValue(XpFieldName: string, isFav: boolean, ID: string): Promise<void> {
    let favorites = this.user.xp[XpFieldName];
    if (isFav && favorites.length >= this.MaxFavorites) {
      this.toastrService.info(`You have reached your limit of ${XpFieldName}`);
      return;
    }
    if (isFav) {
      favorites = [...favorites, ID];
    } else {
      favorites = favorites.filter((x) => x !== ID);
    }
    this.user = await this.ocMeService.Patch({ xp: { [XpFieldName]: favorites } }).toPromise();
  }

  getOrderIDFromToken(): string | void {
    return jwtDecode(this.ocTokenService.GetAccess()).orderid;
  }
}
