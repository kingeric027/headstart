import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { Router, ActivatedRoute } from '@angular/router';
var OrchestrationLogsTableComponent = /** @class */ (function (_super) {
    __extends(OrchestrationLogsTableComponent, _super);
    function OrchestrationLogsTableComponent(buyerService, changeDetectorRef, router, activatedRoute, ngZone) {
        var _this = _super.call(this, changeDetectorRef, buyerService, router, activatedRoute, ngZone) || this;
        _this.buyerService = buyerService;
        _this.route = '/reports/orchestration-logs';
        return _this;
    }
    var _a;
    OrchestrationLogsTableComponent = __decorate([
        Component({
            selector: 'app-orchestration-logs-table',
            templateUrl: './orchestration-logs-table.component.html',
            styleUrls: ['./orchestration-logs-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof BuyerService !== "undefined" && BuyerService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone])
    ], OrchestrationLogsTableComponent);
    return OrchestrationLogsTableComponent;
}(ResourceCrudComponent));
export { OrchestrationLogsTableComponent };
//# sourceMappingURL=orchestration-logs-table.component.js.map