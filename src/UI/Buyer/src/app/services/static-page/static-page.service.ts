import { Injectable } from '@angular/core'
import { HeadStartSDK, JDocument } from '@ordercloud/headstart-sdk'
import { AppConfig } from '../../shopper-context'

@Injectable({
  providedIn: 'root',
})
export class StaticPageService {
  pages: JDocument[] = []

  constructor(public appConfig: AppConfig) {}

  async initialize(): Promise<void> {
    try {
      const pageList = await HeadStartSDK.Documents.ListDocuments(
        'cms-page-schema',
        'ApiClients',
        this.appConfig.clientID
      )
      this.pages = pageList.Items
    } catch (e) {
      // might not be an error if its just not configured
      this.pages = []
    }
  }
}
