import { Injectable } from '@angular/core'
import { Resolve, ActivatedRouteSnapshot } from '@angular/router'
import {} from 'ordercloud-javascript-sdk'
import { ChiliConfig, MeChiliTemplate } from '@ordercloud/headstart-sdk'
import {
  SuperMarketplaceProduct,
  HeadStartSDK,
} from '@ordercloud/headstart-sdk'
import { ChiliConfigService } from '../services/chili-config/chili-config.service'

@Injectable()
export class ChiliTemplateResolver implements Resolve<MeChiliTemplate> {
  constructor(private chili: ChiliConfigService) {}

  async resolve(route: ActivatedRouteSnapshot): Promise<MeChiliTemplate> {
    const template = await this.chili.getChiliTemplate(
      route.params.configurationID
    )
    const superProduct = (await HeadStartSDK.Mes.GetSuperProduct(
      route.params.productID
    )) as any
    template.Product = superProduct
    const frame = await this.chili.getChiliFrame(template.ChiliTemplateID)
    template.Frame = frame
    return template
  }
}
