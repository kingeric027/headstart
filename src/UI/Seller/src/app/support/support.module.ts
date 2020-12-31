import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SupportRoutingModule } from './support-routing.module';
import { CaseSubmissionComponent } from './case-submission/case-submission.component';



@NgModule({
  declarations: [CaseSubmissionComponent],
  imports: [
    SupportRoutingModule,
    CommonModule
  ]
})
export class SupportModule { }
