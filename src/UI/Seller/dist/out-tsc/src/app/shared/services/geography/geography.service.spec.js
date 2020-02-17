import { TestBed, inject } from '@angular/core/testing';
import { AppGeographyService } from '@app-seller/shared/services/geography/geography.service';
describe('OcGeographyService', function () {
    beforeEach(function () {
        TestBed.configureTestingModule({
            providers: [],
        });
    });
    it('should be created', inject([AppGeographyService], function (service) {
        expect(service).toBeTruthy();
    }));
});
//# sourceMappingURL=geography.service.spec.js.map