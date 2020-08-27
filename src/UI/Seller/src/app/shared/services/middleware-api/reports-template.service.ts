import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, OcBuyerService, OcSupplierService } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '../current-user/current-user.service';
import { ListPage } from '@ordercloud/headstart-sdk';

export interface ReportTemplate {
  id?: string;
  SellerID?: string;
  ReportType?: ReportType;
  Name: string;
  Description?: string;
  Headers?: string[];
  Filters?: ReportFilters;
  AvailableToSuppliers?: boolean;
}
export enum ReportType {
  BuyerLocation,
}

export interface ReportFilters {
  BuyerID: string[];
  Country: string[];
  State: string[];
}

@Injectable({
  providedIn: 'root',
})
export class ReportsTemplateService extends ResourceCrudService<ReportTemplate> {
  emptyResource = {
    id: '',
    Name: '',
    Description: '',
    AvailableToSuppliers: false,
    Headers: [],
    Filters: {
      BuyerID: [],
      State: [],
      Country: [],
    },
  };

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    currentUserService: CurrentUserService,
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    super(router, activatedRoute, null, currentUserService, '/reports', 'reports', null, 'templates');
    this.ocService = this;
  }

  private buildHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
  }

  async getResourceById(resourceID: string): Promise<ReportTemplate> {
    const url = `${this.appConfig.middlewareUrl}/reports/${resourceID}`;
    return await this.http.get<ReportTemplate>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async list(args: any[]): Promise<ListPage<ReportTemplate>> {
    var list = await this.listReportTemplatesByReportType('BuyerLocation');
    var listPage = {
      Meta: {
        Page: 1,
        PageSize: 100,
        TotalCount: list.length,
        TotalPages: 1,
      },
      Items: list,
    };
    return listPage;
  }

  async deleteResource(templateID: string): Promise<null> {
    const url = `${this.appConfig.middlewareUrl}/reports/${templateID}`;
    return await this.http.delete<null>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async createNewResource(template: ReportTemplate): Promise<ReportTemplate> {
    const routeUrl = this.router.routerState.snapshot.url.split('/');
    const reportType = routeUrl[2];
    const url = `${this.appConfig.middlewareUrl}/reports/${reportType}`;
    return await this.http.post<ReportTemplate>(url, template, { headers: this.buildHeaders() }).toPromise();
  }

  async updateResource(originalID: string, resource: any): Promise<ReportTemplate> {
    originalID = resource.id;
    const url = `${this.appConfig.middlewareUrl}/reports/${originalID}`;
    const newResource = await this.http.put<ReportTemplate>(url, resource, { headers: this.buildHeaders() }).toPromise();
    this.updateResourceSubject(newResource);
    return newResource;
  }

  async listReportTemplatesByReportType(reportType: string): Promise<any[]> {
    const url = `${this.appConfig.middlewareUrl}/reports/${reportType}/listtemplates`;
    return await this.http.get<any[]>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async previewReport(template: any): Promise<object[]> {
    const url = `${this.appConfig.middlewareUrl}/reports/${template.ReportType}/preview/${template.id}`;
    return await this.http.get<object[]>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async downloadReport(template: any): Promise<void> {
    const url = `${this.appConfig.middlewareUrl}/reports/${template.ReportType}/download/${template.id}`;
    const file = await this.http.post<string>(url, template, { headers: this.buildHeaders() }).toPromise();
    this.getSharedAccessSignature(file).subscribe(sharedAccessSignature => {
      const uri = `${this.appConfig.blobStorageUrl}/downloads/${file}${sharedAccessSignature}`;
      const link = document.createElement('a');
      link.download = file;
      link.href = uri;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    });
  }

  private getSharedAccessSignature(fileName: string): Observable<string> {
    return this.http.get<string>(`${this.appConfig.middlewareUrl}/reports/download-shared-access/${fileName}`);
  }
}
