import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { MeUser } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'support-case-submission',
  templateUrl: './case-submission.component.html',
  styleUrls: ['./case-submission.component.scss']
})
export class CaseSubmissionComponent implements OnInit {
  alive = true
  caseSubmissionForm: FormGroup
  subjectOptions: string[] = [
    'General Question',
    'Report an Error/Bug',
    'Payment, Billing, or Refunds'
  ]
  user: MeUser
  imageFile;
  
  constructor(
    private currentUserService: CurrentUserService,
    private formBuilder: FormBuilder,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
  ) { }

  ngOnInit(): void {
    this.currentUserService.userSubject.pipe(takeWhile(() => this.alive)).subscribe((user) => {
      this.user = user
      this.setForm()
    })
  }

  setForm(): void {
    this.caseSubmissionForm = this.formBuilder.group({
      FirstName: ['', Validators.required],
      LastName: ['', Validators.required],
      Email: ['', Validators.required],
      Vendor: ['', Validators.required],
      Subject: [null, Validators.required],
      Message: ['', Validators.required],
      File: [null]
    })
  }

  onImageUpload(event): void {
    this.imageFile = event.target.files[0]
    this.caseSubmissionForm.controls['File'].setValue(this.imageFile)
  }

  stageImages(file): void {
    this.imageFile = file[0].File;
    this.caseSubmissionForm.controls['File'].setValue(this.imageFile)
  }

  sendCaseSubmission() {
    const form = new FormData();
    Object.keys(this.caseSubmissionForm.value).forEach(key => {
      if (key === 'File') {
        form.append('file', this.caseSubmissionForm.value[key])
      } else {
        form.append(key, this.caseSubmissionForm.value[key])
      }
    })
    return this.http.post(`${this.appConfig.middlewareUrl}/support/submitcase`, form).subscribe((response) => {
      console.log(response);
    })
  }

  ngOnDestroy(): void {
    this.alive = false
  }

}
