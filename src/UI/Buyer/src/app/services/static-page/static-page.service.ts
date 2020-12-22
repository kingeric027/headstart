import { Injectable } from '@angular/core'
import { AppConfig } from '../../shopper-context'
import { ContentManagementClient, JDocument } from '@ordercloud/cms-sdk'

@Injectable({
  providedIn: 'root',
})
export class StaticPageService {
  pages: JDocument[] = []

  constructor(public appConfig: AppConfig) {}

  async initialize(): Promise<void> {
    try {
      const pageList = await ContentManagementClient.Documents.ListDocuments(
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
