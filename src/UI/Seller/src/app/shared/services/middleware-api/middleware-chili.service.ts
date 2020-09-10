import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService } from '@ordercloud/angular-sdk';

// WHOLE FILE TO BE REPLACED BY SDK

export interface ChiliDocuments {
    Documents: TDocument[];
}

export interface ChiliSpecs {
    Specs: TSpec[];
}

interface TDocument {
    id: string;
    name: string;
    pages: number;
}

interface TSpec {
    name: string;
    displayName: string;
    type: string;
    value: string;
    displayValue: string;
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

    async getDocuments(folderName: string): Promise<ChiliDocuments> {
        const url = `${this.appConfig.middlewareUrl}/tecra/documents`;
        return await this.http.get<ChiliDocuments>(url, { headers: this.buildHeaders(), params: {folder: folderName}}).toPromise();
    }

    async getSpecs(docID: string): Promise<ChiliSpecs> {
        const url = `${this.appConfig.middlewareUrl}/tecra/specs`;
        return await this.http.get<ChiliSpecs>(url, { headers: this.buildHeaders(), params: {id: docID}}).toPromise();
    }
}
