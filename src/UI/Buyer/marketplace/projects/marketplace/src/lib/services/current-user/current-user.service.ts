import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MeUser, OcMeService, User } from '@ordercloud/angular-sdk';
import { TokenHelperService } from '../token-helper/token-helper.service';
import { CurrentUserAddressService, ICurrentUserAddress } from './address.service';
import { ICreditCards, CreditCardService } from './credit-card.service';

export interface CurrentUser extends MeUser {
  FavoriteProductIDs: string[];
  FavoriteOrderIDs: string[];
}

export interface ICurrentUser {
  addresses: ICurrentUserAddress;
  cards: ICreditCards;
  get(): CurrentUser;
  patch(user: MeUser): Promise<CurrentUser>;
  onChange(callback: (user: CurrentUser) => void): void; // TODO - replace all these onChange functions with real Observables. More powerful
  isAnonymous(): boolean;
  setIsFavoriteProduct(isFav: boolean, productID: string): void;
  setIsFavoriteOrder(isFav: boolean, orderID: string): void;
}

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService implements ICurrentUser {
  private readonly MaxFavorites: number = 40;
  private readonly favOrdersXP = 'FavoriteOrders';
  private readonly favProductsXP = 'FavoriteProducts';

  private isAnon: boolean = null;
  private userSubject: BehaviorSubject<CurrentUser> = new BehaviorSubject<CurrentUser>(null);

  constructor(
    private ocMeService: OcMeService,
    private tokenHelper: TokenHelperService,
    public addresses: CurrentUserAddressService,
    public cards: CreditCardService
  ) {}

  get(): CurrentUser {
    return this.user;
  }

  async reset(): Promise<void> {
    const user = await this.ocMeService.Get().toPromise();
    this.isAnon = this.tokenHelper.isTokenAnonymous();
    this.user = this.MapToCurrentUser(user);
  }

  async patch(user: MeUser): Promise<CurrentUser> {
    const patched = await this.ocMeService.Patch(user).toPromise();
    this.user = this.MapToCurrentUser(patched);
    return this.user;
  }

  isAnonymous(): boolean {
    return this.isAnon !== null ? this.isAnon : this.tokenHelper.isTokenAnonymous();
  }

  onChange(callback: (user: CurrentUser) => void) {
    this.userSubject.subscribe(callback);
  }

  setIsFavoriteProduct(isFav: boolean, productID: string) {
    this.setFavoriteValue(this.favProductsXP, isFav, productID);
  }

  setIsFavoriteOrder(isFav: boolean, orderID: string) {
    this.setFavoriteValue(this.favOrdersXP, isFav, orderID);
  }

  private MapToCurrentUser(user: MeUser): CurrentUser {
    const currentUser = user as CurrentUser;
    currentUser.FavoriteOrderIDs = this.getFavorites(user, this.favOrdersXP);
    currentUser.FavoriteProductIDs = this.getFavorites(user, this.favProductsXP);
    return currentUser;
  }

  private get user(): CurrentUser {
    return this.userSubject.value;
  }

  private set user(value: CurrentUser) {
    this.userSubject.next(value);
  }

  private getFavorites(user: User, XpFieldName: string): string[] {
    return user && user.xp && user.xp[XpFieldName] instanceof Array ? user.xp[XpFieldName] : [];
  }

  private async setFavoriteValue(XpFieldName: string, isFav: boolean, ID: string): Promise<void> {
    if (!this.user.xp || !this.user.xp[XpFieldName]) {
      this.patch({ xp: { [XpFieldName]: [] } });
    }
    let favorites = this.user.xp[XpFieldName] || [];
    if (isFav && favorites.length >= this.MaxFavorites) {
      throw Error(`You have reached your limit of ${XpFieldName}`);
    }
    if (isFav) {
      favorites = [...favorites, ID];
    } else {
      favorites = favorites.filter(x => x !== ID);
    }
    this.patch({ xp: { [XpFieldName]: favorites } });
  }
}
