import { Injectable } from '@angular/core'
import { Tokens } from 'ordercloud-javascript-sdk'
import {
  ChiliConfig,
  ChiliSpec,
  ListPage,
  MeChiliTemplate,
} from '@ordercloud/headstart-sdk'
import { HttpHeaders, HttpClient, HttpParams } from '@angular/common/http'
import { TempSdk } from '../temp-sdk/temp-sdk.service'
import { AppConfig } from 'src/app/models/environment.types'

@Injectable({
  providedIn: 'root',
})
export class ChiliConfigService {
  constructor(
    private httpClient: HttpClient,
    private appConfig: AppConfig,
    private tempSdk: TempSdk
  ) {}

  async listChiliConfigs(): Promise<ListPage<ChiliConfig>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    const url = `${this.appConfig.middlewareUrl}/chili/config`
    return this.httpClient
      .get<ListPage<ChiliConfig>>(url, { headers })
      .toPromise()
  }

  async getChiliConfig(id: string): Promise<ChiliConfig> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    const params = new HttpParams()
    params.set('id', id)

    const url = `${this.appConfig.middlewareUrl}/chili/config`
    return this.httpClient
      .get<ChiliConfig>(url, { headers, params })
      .toPromise()
  }

  async getChiliSpec(id: string): Promise<ChiliSpec> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    const params = new HttpParams()
    params.set('id', id)
    const url = `${this.appConfig.middlewareUrl}/chili/specs`
    return this.httpClient
      .get<ChiliSpec>(url, { headers, params })
      .toPromise()
  }
  async getChiliTemplate(id: string): Promise<MeChiliTemplate> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    const url = `${this.appConfig.middlewareUrl}/chili/template/me/${id}`
    return this.httpClient
      .get<MeChiliTemplate>(url, { headers })
      .toPromise()
  }

  async getChiliFrame(id: string): Promise<string> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    let params = new HttpParams()
    params = params.append('id', id)
    const url = `${this.appConfig.middlewareUrl}/tecra/frame`
    return this.httpClient
      .get<string>(url, { headers, params })
      .toPromise()
  }

  async getChiliProof(id: string): Promise<string> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    let params = new HttpParams()
    params = params.append('id', id)
    const url = `${this.appConfig.middlewareUrl}/tecra/proof`
    return this.httpClient
      .get<string>(url, { headers, params })
      .toPromise()
  }

  async getChiliPDF(id: string): Promise<string> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    })
    const url = `${this.appConfig.middlewareUrl}/tecra/pdf/${id}`
    return this.httpClient
      .get<string>(url, { headers })
      .toPromise()
  }
}
