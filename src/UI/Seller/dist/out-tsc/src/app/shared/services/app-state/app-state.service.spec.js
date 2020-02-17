import { TestBed, inject } from '@angular/core/testing';
import { AppStateService } from '@app-seller/shared/services/app-state/app-state.service';
describe('AppStateService', function () {
    beforeEach(function () {
        TestBed.configureTestingModule({
            providers: [],
        });
    });
    it('should be created', inject([AppStateService], function (service) {
        expect(service).toBeTruthy();
    }));
});
//# sourceMappingURL=app-state.service.spec.js.map