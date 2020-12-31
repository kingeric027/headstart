import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
  

  constructor(private currentUserService: CurrentUserService, private formBuilder: FormBuilder) { }

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
      Message: ['', Validators.required]
    })
  }

  sendCaseSubmission(): void {
    console.log('sent!')
  }

  ngOnDestroy(): void {
    this.alive = false
  }

}
