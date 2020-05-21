import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MeUser, OcMeService, User, UserGroup } from '@ordercloud/angular-sdk';
import { TokenHelperService } from '../token-helper/token-helper.service';
import { CreditCardService } from './credit-card.service';
<<<<<<< refs/remotes/origin/dev
import { HttpClient } from '@angular/common/http';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
=======
import { PermissionType } from '../../shopper-context';
>>>>>>> remove uis for unpermissioned things

export interface CurrentUser extends MeUser {
  FavoriteProductIDs: string[];
  FavoriteOrderIDs: string[];
  UserGroups: UserGroup<any>[];
}

export interface ICurrentUser {
  cards: CreditCardService;
  get(): CurrentUser;
  patch(user: MeUser): Promise<CurrentUser>;
  onChange(callback: (user: CurrentUser) => void): void; // TODO - replace all these onChange functions with real Observables. More powerful
  isAnonymous(): boolean;
  setIsFavoriteProduct(isFav: boolean, productID: string): void;
  setIsFavoriteOrder(isFav: boolean, orderID: string): void;
  hasLocationAccess(locationID: string, permissionType: string): boolean;
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

  // users for determining location management permissions for a user
  private userGroups: BehaviorSubject<UserGroup[]> = new BehaviorSubject<UserGroup[]>([]);

  constructor(
    private ocMeService: OcMeService,
    private tokenHelper: TokenHelperService,
    public cards: CreditCardService,
    public http: HttpClient
  ) {}

  get(): CurrentUser {
    return this.user;
  }

  async reset(): Promise<void> {
    const requests: any[] = [this.ocMeService.Get().toPromise(), this.ocMeService.ListUserGroups().toPromise()];
    const [user, userGroups] = await Promise.all(requests);
    this.isAnon = this.tokenHelper.isTokenAnonymous();
    this.user = await this.MapToCurrentUser(user);
    this.userGroups.next(userGroups.Items);
  }

  async patch(user: MeUser): Promise<CurrentUser> {
    const patched = await this.ocMeService.Patch(user).toPromise();
    this.user = await this.MapToCurrentUser(patched);
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

  hasRoles(...roles: string[]): boolean {
    return roles.every(role => this.user.AvailableRoles.includes(role));
  }

  hasLocationAccess(locationID: string, permissionType: string): boolean {
    const userGroupIDNeeded = `${locationID}-${permissionType}`;
    return this.userGroups.value.some(u => u.ID === userGroupIDNeeded);
  }
  
  private async MapToCurrentUser(user: MeUser): Promise<CurrentUser> {
    const currentUser = user as CurrentUser;
    const myUserGroups = await this.ocMeService.ListUserGroups().toPromise();
    currentUser.UserGroups = myUserGroups.Items;
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
