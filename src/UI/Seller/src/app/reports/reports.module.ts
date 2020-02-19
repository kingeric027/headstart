import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { ReportsRoutingModule } from './reports-routing.module';
import { OrchestrationLogsTableComponent } from './components/orchestration-logs-table/orchestration-logs-table.component';

@NgModule({
  imports: [SharedModule, ReportsRoutingModule],
  declarations: [OrchestrationLogsTableComponent],
})
export class ReportsModule {}
