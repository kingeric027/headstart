import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { Observable } from 'rxjs';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { DomSanitizer } from '@angular/platform-browser';
import { AppAuthService } from '@app-seller/auth';

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
    const httpOptions = {
  headers: new HttpHeaders({
    'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    Authorization: 'Bearer ' + accessToken,
  }),
    };

    if (fileType === 'staticContent') {
      const mappedFiles: FileHandle[] = Array.from(event.target.files).map((file: File) => {
        const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
        return { File: file, URL, Filename: 'shipments_to_process' };
      });
      const formData = new FormData();
      formData.append('file', mappedFiles[0]?.File);
      this.http.post(this.appConfig.middlewareUrl + '/shipment/batch/uploadshipment', mappedFiles[0]?.File, httpOptions).subscribe(result => {
                console.log(result)
      }
      );
      console.log(mappedFiles);
    }
  }

  stageDocument(event): void {
    console.log(event);
  }
}
