import { async, TestBed } from '@angular/core/testing';
import { OrchestrationLogsTableComponent } from './orchestration-logs-table.component';
describe('OrchestrationLogsTableComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [OrchestrationLogsTableComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(OrchestrationLogsTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=orchestration-logs-table.component.spec.js.map