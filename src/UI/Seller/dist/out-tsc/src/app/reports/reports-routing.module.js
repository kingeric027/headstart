import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { OrchestrationLogsTableComponent } from './components/orchestration-logs-table/orchestration-logs-table.component';
var routes = [{ path: 'orchestration-logs', component: OrchestrationLogsTableComponent }];
var ReportsRoutingModule = /** @class */ (function () {
    function ReportsRoutingModule() {
    }
    ReportsRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], ReportsRoutingModule);
    return ReportsRoutingModule;
}());
export { ReportsRoutingModule };
//# sourceMappingURL=reports-routing.module.js.map