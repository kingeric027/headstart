import { TestBed } from '@angular/core/testing';

import { MiddlewareAPIService } from './middleware-api.service';

describe('MiddlewareAPIServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MiddlewareAPIService = TestBed.get(MiddlewareAPIServiceService);
    expect(service).toBeTruthy();
  });
});
