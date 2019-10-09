import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MeUser, OcMeService, OcTokenService, User } from '@ordercloud/angular-sdk';
import * as jwtDecode from 'jwt-decode';
import { isUndefined as _isUndefined } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { ICurrentUser } from 'shopper-context-interface';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService implements ICurrentUser {
  private readonly MaxFavorites: number = 40;
  private readonly favOrdersXP = 'FavoriteOrders';
  private readonly favProductsXP = 'FavoriteProducts';

  private userSubject: BehaviorSubject<MeUser> = new BehaviorSubject<MeUser>(null);
  private isAnonSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  private loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private favoriteOrdersSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  private favoriteProductsSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);

  constructor(private ocMeService: OcMeService, private ocTokenService: OcTokenService, private toastrService: ToastrService) {}

  async reset(): Promise<void> {
    this.isAnonymous = this.isTokenAnonymous();
    this.user = await this.ocMeService.Get().toPromise();
    this.isLoggedIn = this.user.Active;
  }

  get isLoggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  set isLoggedIn(value: boolean) {
    this.loggedInSubject.next(value);
  }

  get isAnonymous(): boolean {
    const anon = this.isAnonSubject.value;
    return anon === null ? this.isTokenAnonymous() : anon;
  }

  set isAnonymous(value: boolean) {
    this.isAnonSubject.next(value);
  }

  get favoriteOrderIDs(): string[] {
    return this.favoriteOrdersSubject.value;
  }

  get favoriteProductIDs(): string[] {
    return this.favoriteProductsSubject.value;
  }

  private get user(): MeUser {
    return this.userSubject.value;
  }

  private set user(value: MeUser) {
    this.favoriteOrdersSubject.next(this.getFavorites(value, this.favOrdersXP));
    this.favoriteProductsSubject.next(this.getFavorites(value, this.favProductsXP));
    this.userSubject.next(value);
  }

  get(): MeUser {
    return this.user;
  }

  async patch(user: MeUser): Promise<MeUser> {
    return (this.user = await this.ocMeService.Patch(user).toPromise());
  }

  onIsLoggedInChange(callback: (isLoggedIn: boolean) => void): void {
    this.loggedInSubject.subscribe(callback);
  }

  onIsAnonymousChange(callback: (isAnonymous: boolean) => void): void {
    this.isAnonSubject.subscribe(callback);
  }

  onUserChange(callback: (user: User) => void) {
    this.userSubject.subscribe(callback);
  }

  setIsFavoriteProduct(isFav: boolean, productID: string) {
    this.setFavoriteValue(this.favProductsXP, isFav, productID);
  }

  onFavoriteProductsChange(callback: (productIDs: string[]) => void) {
    this.favoriteProductsSubject.subscribe(callback);
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
    this.patch({ xp: { [XpFieldName]: favorites } });
  }

  private isTokenAnonymous(): boolean {
    return !_isUndefined(this.getOrderIDFromToken());
  }

  getOrderIDFromToken(): string | void {
    const jwt = this.ocTokenService.GetAccess();
    if (!jwt) return null;
    return jwtDecode(jwt).orderid;
  }
}
