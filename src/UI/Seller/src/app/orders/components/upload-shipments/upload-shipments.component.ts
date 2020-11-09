import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { Observable } from 'rxjs';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { DomSanitizer } from '@angular/platform-browser';
import { AppAuthService } from '@app-seller/auth';
import { Asset, AssetUpload, HeadStartSDK } from '@ordercloud/headstart-sdk';
import { getPsHeight } from '@app-seller/shared/services/dom.helper';
import { BatchProcessResult } from './shipment-upload.interface';

@Component({
  selector: 'upload-shipments',
  templateUrl: './upload-shipments.component.html',
  styleUrls: ['./upload-shipments.component.scss'],
})
export class UploadShipmentsComponent {
  constructor(
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private sanitizer: DomSanitizer,
    private appAuthService: AppAuthService
  ) {
    this.contentHeight = getPsHeight('base-layout-item');
  }

  files: FileHandle[] = [];
  contentHeight = 0;
  showUploadSummary = false;
  batchProcessResult: BatchProcessResult;

  downloadTemplate(): void {
    const file = 'Shipment_Import_Template.xlsx';
    this.getSharedAccessSignature(file).subscribe(sharedAccessSignature => {
      const uri = `${this.appConfig.blobStorageUrl}/downloads/Shipment_Import_Template.xlsx${sharedAccessSignature}`;
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

  async manualFileUpload(event, fileType: string): Promise<void> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    let asset: AssetUpload = {};

    if (fileType === 'staticContent') {
      const mappedFiles: FileHandle[] = Array.from(event).map((file: File) => {
        asset = {
          Active: true,
          Title: 'document',
          File: file,
          FileName: 'shipments_to_process',
        } as AssetUpload;
        return { File: file, URL, Filename: 'shipments_to_process' };
      });

      const headers = new HttpHeaders({ Authorization: `Bearer ${accessToken}` });

      const formData = new FormData();

      for (const prop in asset) {
        if (asset.hasOwnProperty(prop)) {
          formData.append(prop, asset[prop]);
        }
      }

      this.http
        .post(this.appConfig.middlewareUrl + '/shipment/batch/uploadshipment', formData, { headers })
        .subscribe((result: BatchProcessResult) => {
          if (result !== null) {
            this.showUploadSummary = true;
            this.batchProcessResult = result;
          }
        });
      console.log(mappedFiles);
    }
  }

  async uploadAsset(productID: string, file: FileHandle, isAttachment = false): Promise<any> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const asset = {
      Active: true,
      Title: isAttachment ? 'Product_Attachment' : null,
      File: file.File,
      FileName: file.Filename,
    } as AssetUpload;
    const newAsset: Asset = await HeadStartSDK.Upload.UploadAsset(asset, accessToken);
    await HeadStartSDK.Assets.SaveAssetAssignment(
      { ResourceType: 'Products', ResourceID: productID, AssetID: newAsset.ID },
      accessToken
    );
    return await HeadStartSDK.Products.Get(productID, accessToken);
  }

  getColumnHeader(columnNumber: number): string {
    //Ensure number is within the amount of columns on the Excel sheet.
    if (columnNumber >= 1 && columnNumber <= 16) {
      return ShipmentImportColumnHeader[columnNumber];
    }
  }

  stageDocument(event): void {
    console.log(event);
  }
}

export enum ShipmentImportColumnHeader {
  OrderID = 1,
  LineItemID = 2,
  QuantityShipped = 3,
  ShipmentID = 4,
  BuyerID = 5,
  Shipper = 6,
  DateShipped = 7,
  DateDelivered = 8,
  TrackingNumber = 9,
  Cost = 10,
  FromAddressID = 11,
  ToAddressID = 12,
  Account = 13,
  XpService = 14,
  ShipmentComment = 15,
  ShipmentLineItemComment = 16,
}
