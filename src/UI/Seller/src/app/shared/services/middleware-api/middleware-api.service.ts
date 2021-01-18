import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Inject, Injectable } from '@angular/core'
import {
  AppConfig,
  applicationConfiguration,
} from '@app-seller/config/app.config'
import {
  MonitoredProductFieldModifiedNotificationDocument,
  NotificationStatus,
} from '@app-seller/shared/models/monitored-product-field-modified-notification.interface'
import { OcTokenService, Order, Product } from '@ordercloud/angular-sdk'
import {
  MarketplaceSupplier,
  ListPage,
  SuperMarketplaceProduct,
  SuperShipment,
  BatchProcessResult,
} from '@ordercloud/headstart-sdk'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class MiddlewareAPIService {
  readonly headers = {
    headers: new HttpHeaders({
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    }),
  }
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  async acknowledgeQuoteOrder(orderID: string): Promise<Order> {
    const url = `${this.appConfig.middlewareUrl}/order/acknowledgequote/${orderID}`
    return await this.http.post<Order>(url, this.headers).toPromise()
  }

  async isLocationDeletable(locationID: string): Promise<boolean> {
    const url = `${this.appConfig.middlewareUrl}/supplier/candelete/${locationID}`
    return await this.http.get<boolean>(url, this.headers).toPromise()
  }

  async updateSupplier(supplierID: string, supplier: any): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/supplier/${supplierID}`
    return await this.http.patch(url, supplier, this.headers).toPromise()
  }

  async getSupplierFilterConfig(): Promise<
    ListPage<SupplierFilterConfigDocument>
  > {
    const url = `${this.appConfig.middlewareUrl}/supplierfilterconfig`
    return await this.http
      .get<ListPage<SupplierFilterConfigDocument>>(url, this.headers)
      .toPromise()
  }

  async getSupplierData(supplierOrderID: string): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/supplier/orderdetails/${supplierOrderID}`
    return await this.http.get<any>(url, this.headers).toPromise()
  }

  async updateProductNotifications(
    notification: MonitoredProductFieldModifiedNotificationDocument
  ): Promise<SuperMarketplaceProduct> {
    return await this.http
      .put<SuperMarketplaceProduct>(
        `${this.appConfig.middlewareUrl}/notifications/monitored-product-field-modified/${notification.ID}`,
        notification,
        this.headers
      )
      .toPromise()
  }

  async getProductNotifications(
    superProduct: SuperMarketplaceProduct
  ): Promise<MonitoredProductFieldModifiedNotificationDocument[]> {
    const productModifiedNotifications = await this.http
      .post<ListPage<MonitoredProductFieldModifiedNotificationDocument>>(
        `${this.appConfig.middlewareUrl}/notifications/monitored-product-notification`,
        superProduct,
        this.headers
      )
      .toPromise()

    return productModifiedNotifications?.Items.filter(
      (i) => i?.Doc?.Status === NotificationStatus.SUBMITTED
    )
  }

  async patchLineItems(
    superShipment: SuperShipment,
    headers: HttpHeaders
  ): Promise<any> {
    return await this.http
      .post(this.appConfig.middlewareUrl + '/shipment', superShipment, {
        headers,
      })
      .toPromise()
  }

  batchShipmentUpload(
    formData: FormData,
    headers: HttpHeaders
  ): Observable<BatchProcessResult> {
    return this.http.post(
      this.appConfig.middlewareUrl + '/shipment/batch/uploadshipment',
      formData,
      { headers }
    )
  }
}
