import { Injectable } from '@angular/core';
import { Me, Tokens } from 'ordercloud-javascript-sdk';
import { AppConfig } from '../../shopper-context';
import {
  HeadStartSDK,
  ChiliConfig,
  ChiliSpec,
  ListPage,

  ChiliTemplate
} from '@ordercloud/headstart-sdk';
import { HttpHeaders, HttpClient, HttpParams } from '@angular/common/http';
import { ClaimStatus, LineItemStatus } from '../../../lib/shopper-context';
import { TempSdk } from '../temp-sdk/temp-sdk.service';
@Injectable({
  providedIn: 'root',
})
export class ChiliConfigService {

  constructor(
    private httpClient: HttpClient,
    private appConfig: AppConfig,
    private tempSdk: TempSdk
  ) { }

  async listChiliConfigs(): Promise<ListPage<ChiliConfig>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/chili/config`;
    return this.httpClient
      .get<ListPage<ChiliConfig>>(url, { headers })
      .toPromise();
  }

  async getChiliConfig(id: string): Promise<ChiliConfig> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const params = new HttpParams();
    params.set('id', id);

    const url = `${this.appConfig.middlewareUrl}/chili/config`;
    return this.httpClient
      .get<ChiliConfig>(url, { headers, params })
      .toPromise();
  }

  async getChiliSpec(id: string): Promise<ChiliSpec> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const params = new HttpParams();
    params.set('id', id);
    const url = `${this.appConfig.middlewareUrl}/chili/specs`;
    return this.httpClient
      .get<ChiliSpec>(url, { headers, params })
      .toPromise();
  }
  async getChiliTemplate(id: string): Promise<ChiliTemplate> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/chili/template/${id}`;
    return this.httpClient
      .get<ChiliTemplate>(url, { headers })
      .toPromise();
  }

  async getChiliFrame(id: string, storeid: string): Promise<string> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    let params = new HttpParams();
    params = params.append('id', id);
    params = params.append('storeid', storeid);
    const url = `${this.appConfig.middlewareUrl}/tecra/frame`;
    return this.httpClient
      .get<string>(url, { headers, params })
      .toPromise();
  }

}
