import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { User, OcAdminUserService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class SellerUserService extends ResourceCrudService<User> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocAdminUserService: OcAdminUserService,
    currentUserService: CurrentUserService, middleware: MiddlewareAPIService,) {
    super(router, activatedRoute, ocAdminUserService, currentUserService, middleware, '/seller-users', 'users');
  }

  emptyResource = {
    Username: '',
    FirstName: '',
    LastName: '',
    Email: '',
    Phone: '',
  };
}
