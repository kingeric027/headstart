import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { } from 'ordercloud-javascript-sdk';
import { ChiliConfig, ChiliTemplate } from '@ordercloud/headstart-sdk';
import { ChiliConfigService } from '../services/chili-config/chili-config.service';

@Injectable()
export class ChiliTemplateResolver implements Resolve<ChiliTemplate> {
  constructor(private chili: ChiliConfigService) { }

  async resolve(route: ActivatedRouteSnapshot): Promise<ChiliTemplate> {
    return await this.chili.getChiliTemplate(route.params.configurationID);
  }
}
