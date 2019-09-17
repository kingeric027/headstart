import { TestBed, inject } from '@angular/core/testing';

import { AppGeographyService } from 'src/app/shared/services/geography/geography.service';

describe('OcGeographyService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [],
    });
  });

  it('should be created', inject(
    [AppGeographyService],
    (service: AppGeographyService) => {
      expect(service).toBeTruthy();
    }
  ));
});
