// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrchestrationLogsTableComponent } from './components/orchestration-logs-table/orchestration-logs-table.component';

const routes: Routes = [{ path: 'logs', component: OrchestrationLogsTableComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRoutingModule {}
