import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { ChiliConfig, ChiliSpec, ChiliSpecOption, ChiliTemplate } from '@ordercloud/headstart-sdk';
import { ListPage } from 'marketplace-javascript-sdk/dist/models/ListPage';

// WHOLE FILE TO BE REPLACED BY SDK


export interface TecraDocument {
    id: string;
    name: string;
    pages: number;
}
export interface TecraSpec {
    name: string;
    dataType: string;
    displayName: string;
    displayValue: string;
    required: string;
    value: string;
    visible: string;
}



@Injectable({
    providedIn: 'root',
})
export class ChiliService {
    constructor(
        private ocTokenService: OcTokenService,
        private http: HttpClient,
        @Inject(applicationConfiguration) private appConfig: AppConfig
    ) { }

    private buildHeaders(): HttpHeaders {
        return new HttpHeaders({
            Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
        });
    }

    //Tecra
    async getDocuments(): Promise<TecraDocument[]> {
        const url = `${this.appConfig.middlewareUrl}/tecra/documents`;
        return await this.http.get<TecraDocument[]>(url, { headers: this.buildHeaders() }).toPromise();
    }
    async getDocumentsByFolder(folderName: string): Promise<TecraDocument[]> {
        const url = `${this.appConfig.middlewareUrl}/tecra/documents/byfolder`;
        return await this.http.get<TecraDocument[]>(url, { headers: this.buildHeaders(), params: {folder: folderName}}).toPromise();
    }
    async getSpecs(docID: string): Promise<TecraSpec[]> {
        const url = `${this.appConfig.middlewareUrl}/tecra/specs`;
        return await this.http.get<TecraSpec[]>(url, { headers: this.buildHeaders(), params: { id: docID } }).toPromise();
    }

    //Chili Config
    async listChiliConfigs(): Promise<ListPage<ChiliConfig>> {
        const url = `${this.appConfig.middlewareUrl}/chili/config`;
        return await this.http.get<ListPage<ChiliConfig>>(url, { headers: this.buildHeaders() }).toPromise();
    }
    async getChiliConfig(id: string): Promise<ChiliConfig> {
        const url = `${this.appConfig.middlewareUrl}/chili/config`;
        return await this.http.get<ChiliConfig>(url, { headers: this.buildHeaders(), params: { id: id } }).toPromise();
    }
    async saveChiliConfig(config: ChiliConfig): Promise<ChiliConfig> {
        const url = `${this.appConfig.middlewareUrl}/chili/config`;
        return await this.http.post<ChiliConfig>(url, config, { headers: this.buildHeaders() }).toPromise();
    }
    async deleteChiliConfig(id: string) {
        const url = `${this.appConfig.middlewareUrl}/chili/config/${id}`;
        return await this.http.delete<ChiliConfig>(url, { headers: this.buildHeaders() }).toPromise();
    }

    //Chili Specs
    async saveChiliSpec(spec: ChiliSpec): Promise<ChiliSpec> {
        const url = `${this.appConfig.middlewareUrl}/chili/specs`;
        return await this.http.post<ChiliSpec>(url, spec, { headers: this.buildHeaders() }).toPromise();
    }
    async deleteChiliSpec(id: string) {
        const url = `${this.appConfig.middlewareUrl}/chili/specs/${id}`;
        return await this.http.delete<ChiliSpec>(url, { headers: this.buildHeaders() }).toPromise();
    }

    //Chili Spec Options
    async saveChiliSpecOption(specID: string, spec: ChiliSpecOption): Promise<ChiliSpecOption> {
        const url = `${this.appConfig.middlewareUrl}/chili/specs/${specID}/options`;
        return await this.http.post<ChiliSpecOption>(url, spec, { headers: this.buildHeaders() }).toPromise();
    }
    async updateChiliSpecOption(specID: string, spec: ChiliSpecOption): Promise<ChiliSpecOption> {
        const url = `${this.appConfig.middlewareUrl}/chili/specs/${specID}/options/${spec.ID}`;
        return await this.http.put<ChiliSpecOption>(url, spec, { headers: this.buildHeaders() }).toPromise();
    }
    async deleteChiliSpecOption(specID: string, optionID: string) {
        const url = `${this.appConfig.middlewareUrl}/chili/specs/${specID}/options/${optionID}`;
        return await this.http.delete<ChiliSpecOption>(url, { headers: this.buildHeaders() }).toPromise();
    }

    //Chili Template
    async getChiliTemplate(id: string): Promise<ChiliTemplate> {
        const url = `${this.appConfig.middlewareUrl}/chili/template/${id}`;
        return await this.http.get<ChiliTemplate>(url, { headers: this.buildHeaders() }).toPromise();
    }
}
