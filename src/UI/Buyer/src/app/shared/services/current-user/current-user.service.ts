import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MeUser, OcMeService, OcTokenService, User } from '@ordercloud/angular-sdk';
import * as jwtDecode from 'jwt-decode';
import { isUndefined as _isUndefined } from 'lodash';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  public userSubject: BehaviorSubject<MeUser> = new BehaviorSubject<MeUser>(null);
  public isAnonSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private ocMeService: OcMeService, private ocTokenService: OcTokenService) {}

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
}
