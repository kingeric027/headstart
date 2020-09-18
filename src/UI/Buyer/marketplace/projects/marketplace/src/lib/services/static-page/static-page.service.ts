import { Injectable } from '@angular/core';
import { HeadStartSDK } from '@ordercloud/headstart-sdk';
import { AppConfig } from '../../shopper-context';


@Injectable({
  providedIn: 'root'
})
export class StaticPageService {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  pages: any[] = [] // TODO: add PageDocument type to cms library so this is strongly typed

  constructor(
    public appConfig: AppConfig
  ) { }

  async initialize(): Promise<void> {
    try {
      const pageList = await HeadStartSDK.Documents.ListDocuments('cms-page-schema', 'ApiClients', this.appConfig.clientID)
      this.pages = pageList.Items;
      console.log(this.pages);
    } catch (e) {
      // might not be an error if its just not configured
      this.pages = [];
      console.log('error retrieving static pages');
    }
  }
}
