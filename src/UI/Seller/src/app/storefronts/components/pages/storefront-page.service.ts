import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import {
  User,
  OcUserService,
  UserGroupAssignment,
  OcUserGroupService,
  ListUserGroup,
  ListUserGroupAssignment,
} from '@ordercloud/angular-sdk';
import { JDocument, HeadStartSDK, ListPage } from '@ordercloud/headstart-sdk';
import { STOREFRONTS_SUB_RESOURCE_LIST } from '../storefronts/storefronts.service';
import { IUserPermissionsService } from '@app-seller/shared/models/user-permissions.interface';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { CatalogsTempService } from '@app-seller/shared/services/middleware-api/catalogs-temp.service';
import { ResourceType } from '@ordercloud/angular-cms-components/shared/models/resource-type.interface';
import { ocAppConfig } from '@app-seller/config/app.config';

// TODO - this service is only relevent if you're already on the storefronts details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class StorefrontPageService extends ResourceCrudService<JDocument> {
  emptyResource: JDocument = {
    ID: '',
    Doc: {
      Title: '',
      Url: '',
      Description: '',
      MetaImageUrl: '',
      DateCreated: '',
      Author: '',
      DateLastUpdated: '',
      LastUpdatedBy: '',
      HeaderEmbeds: '',
      Content: '',
      FooterEmbeds: '',
      Active: false,
      NavigationTitle: '',
    },
    SchemaSpecUrl: `${ocAppConfig.middlewareUrl}/schema-specs/55c72ad7-e65c-4957-b545-0ba187188af8`,
    History: {
      DateCreated: '',
      CreatedByUserID: '',
      DateUpdated: '',
      UpdatedByUserID: '',
    },
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, public currentUserService: CurrentUserService) {
    super(
      router,
      activatedRoute,
      HeadStartSDK.Documents,
      currentUserService,
      '/storefronts',
      'storefronts',
      STOREFRONTS_SUB_RESOURCE_LIST,
      'pages'
    );
  }
  // Overwritten functions
  async list(args: any[]): Promise<ListPage<JDocument>> {
    console.group();
    console.log(this.ocService);
    console.log(args);
    console.groupEnd();
    console.log(this.route);
    const DocResponse = (await HeadStartSDK.Documents.ListDocuments(
      'cms-page-schema',
      args[0] /* The ResourceID */,
      null,
      'ApiClients'
    )) as any;
    return {
      Meta: {
        ItemRange: [1, DocResponse.length],
        Page: 1,
        PageSize: 1,
        TotalCount: DocResponse.length,
        TotalPages: 1,
      },
      Items: DocResponse,
    };
  }

  async getResourceById(resourceID: string): Promise<any> {
    const orderDirection = this.optionsSubject.value.OrderDirection;
    const args = await this.createListArgs([resourceID], orderDirection);
    return HeadStartSDK.Documents.Get('cms-page-schema', resourceID);
  }
  // End Overwritten functions
}
