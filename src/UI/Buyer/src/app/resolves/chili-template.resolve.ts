import { Injectable } from '@angular/core'
import { Resolve, ActivatedRouteSnapshot } from '@angular/router'
import {} from 'ordercloud-javascript-sdk'
import {
  ChiliConfig,
  MeChiliTemplate,
  TecraSpec,
} from '@ordercloud/headstart-sdk'
import {
  SuperHSMeProduct,
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
    )) as SuperHSMeProduct
    template.Product = superProduct
    const templateID = (route.params.documentID
      ? route.params.documentID
      : template.ChiliTemplateID) as string

    if (route.params.documentID) {
      const updatedSpecs = ((await HeadStartSDK.TecraSpecs.Get(
        route.params.documentID
      )) as unknown) as TecraSpec[]

      updatedSpecs.map((spec) => {
        template.TemplateSpecs.forEach((tspec) => {
          if (tspec.Name === spec.name) {
            tspec.DefaultValue = spec.displayValue
          }
        })
      })
    }

    const frame = await this.chili.getChiliFrame(templateID)
    template.Frame = frame
    template.ChiliConfigID = route.params.configurationID as string
    template.ChiliTemplateID = templateID
    return template
  }
}
