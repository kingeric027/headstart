import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { Observable } from 'rxjs';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { DomSanitizer } from '@angular/platform-browser';
import { AppAuthService } from '@app-seller/auth';
import { Asset, AssetUpload, HeadStartSDK } from '@ordercloud/headstart-sdk';
import paramsSerializer from './ParamsSerializer';

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
  ) {}

  files: FileHandle[] = [];

  async downloadTemplate() {
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

  async manualFileUpload(event, fileType: string) {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    var asset: AssetUpload = {};

    if (fileType === 'staticContent') {
      const mappedFiles: FileHandle[] = Array.from(event.target.files).map((file: File) => {
        asset = {
          Active: true,
          Title: 'document',
          File: file,
          FileName: 'shipments_to_process',
        } as AssetUpload;
        return { File: file, URL, Filename: 'shipments_to_process' };
      });

      const headers = new HttpHeaders({
        Authorization: `Bearer ${accessToken}`,
      });

      const formData = new FormData();

      for (const prop in asset) {
        if (asset.hasOwnProperty(prop)) {
          formData.append(prop, asset[prop]);
        }
      }

      this.http
        .post(this.appConfig.middlewareUrl + '/shipment/batch/uploadshipment', formData, {
          headers,
        })
        .subscribe(result => {
          console.log(result);
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

  stageDocument(event): void {
    console.log(event);
  }
}
