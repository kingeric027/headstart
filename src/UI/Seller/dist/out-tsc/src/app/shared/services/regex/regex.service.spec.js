import { TestBed, inject } from '@angular/core/testing';
import { RegexService } from '@app-seller/shared/services/regex/regex.service';
describe('RegexService', function () {
    beforeEach(function () {
        TestBed.configureTestingModule({
            providers: [],
        });
    });
    it('should be created', inject([RegexService], function (service) {
        expect(service).toBeTruthy();
    }));
});
//# sourceMappingURL=regex.service.spec.js.map