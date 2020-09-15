import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { JDocument, HeadStartSDK, ListPage } from '@ordercloud/headstart-sdk';
import { STOREFRONTS_SUB_RESOURCE_LIST } from '../storefronts/storefronts.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
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
    return await HeadStartSDK.Documents.ListDocuments('cms-page-schema', 'ApiClients', args[0] /* The ResourceID */);
  }

  async getResourceById(resourceID: string): Promise<any> {
    const orderDirection = this.optionsSubject.value.OrderDirection;
    const args = await this.createListArgs([resourceID], orderDirection);
    return HeadStartSDK.Documents.Get('cms-page-schema', resourceID);
  }
  // End Overwritten functions
}
