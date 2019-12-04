import { TestBed, async } from '@angular/core/testing';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { applicationConfiguration } from 'src/app/config/app.config';

import { OcAuthService, Configuration } from '@ordercloud/angular-sdk';
import { RouterTestingModule } from '@angular/router/testing';

describe('BaseResolveService', () => {
  const tokenWithOrderId =
    'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c3IiOiJhbm9uX3VzZXIiLCJjaWQiOiI4MDIxODkzNi0zNTBiLTQxMDUtYTFmYy05NjJhZjAyM2Q2NjYiLCJvcmRlcmlkIjoiSVlBSnFOWVVpRVdyTy1Lei1TalpqUSIsInVzcnR5cGUiOiJidXllciIsInJvbGUiOlsiQnV5ZXJSZWFkZXIiLCJNZUFkbWluIiwiTWVDcmVkaXRDYXJkQWRtaW4iLCJNZUFkZHJlc3NBZG1pbiIsIk1lWHBBZG1pbiIsIlBhc3N3b3JkUmVzZXQiLCJTaGlwbWVudFJlYWRlciIsIlNob3BwZXIiLCJBZGRyZXNzUmVhZGVyIl0sImlzcyI6Imh0dHBzOi8vYXV0aC5vcmRlcmNsb3VkLmlvIiwiYXVkIjoiaHR0cHM6Ly9hcGkub3JkZXJjbG91ZC5pbyIsImV4cCI6MTUyNzA5Nzc0MywibmJmIjoxNTI2NDkyOTQzfQ.MBV7dqBq8RXSZZ8vEGidcfH8vSwOR55yHzvAq1w2bLc';
  const tokenWithoutOrderID =
    'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c3IiOiJjaGlwb3RsZWNyaGlzdGlhbiIsImNpZCI6Ijc1NzMwMTc4LWU0MjQtNGM0OS1iM2Q3LTE3Mzg1Nzg0YjE5MSIsInVzcnR5cGUiOiJidXllciIsInJvbGUiOlsiQnV5ZXJSZWFkZXIiLCJDYXRhbG9nUmVhZGVyIiwiTWVBZG1pbiIsIk1lQ3JlZGl0Q2FyZEFkbWluIiwiTWVBZGRyZXNzQWRtaW4iLCJNZVhwQWRtaW4iLCJQYXNzd29yZFJlc2V0IiwiU2hpcG1lbnRSZWFkZXIiLCJTaG9wcGVyIiwiT3JkZXJSZWFkZXIiLCJBZGRyZXNzQWRtaW4iLCJVc2VyR3JvdXBBZG1pbiJdLCJpc3MiOiJodHRwczovL2F1dGgub3JkZXJjbG91ZC5pbyIsImF1ZCI6Imh0dHBzOi8vYXBpLm9yZGVyY2xvdWQuaW8iLCJleHAiOjE1MjY1Mjg3NzQsIm5iZiI6MTUyNjQ5Mjc3NH0.uqh3_yLXTCSpzLxk6B4gbPX0wmQF4JIZTEHRXvPD9r0';

  beforeEach(async(() => {
    const authService = { isUserAnon: jasmine.createSpy('isUserAnon') };
    const appConfig = { appname: 'mgr-dev' };
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        { provide: OcAuthService, useValue: authService },
        HttpClient,
        HttpHandler,
        { provide: Configuration, useValue: new Configuration() },
        { provide: applicationConfiguration, useValue: appConfig },
      ],
    });
  }));
});