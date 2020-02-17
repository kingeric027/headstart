import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { ReportsRoutingModule } from './reports-routing.module';
import { OrchestrationLogsTableComponent } from './components/orchestration-logs-table/orchestration-logs-table.component';
var ReportsModule = /** @class */ (function () {
    function ReportsModule() {
    }
    ReportsModule = __decorate([
        NgModule({
            imports: [SharedModule, ReportsRoutingModule],
            declarations: [OrchestrationLogsTableComponent],
        })
    ], ReportsModule);
    return ReportsModule;
}());
export { ReportsModule };
//# sourceMappingURL=reports.module.js.map