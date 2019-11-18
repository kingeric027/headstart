import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { ReportListComponent } from './components/report-list/report-list.component';
import { ReportsRoutingModule } from './reports-routing.module';

@NgModule({
  imports: [SharedModule, ReportsRoutingModule],
  declarations: [ReportListComponent],
})
export class ReportsModule {}
