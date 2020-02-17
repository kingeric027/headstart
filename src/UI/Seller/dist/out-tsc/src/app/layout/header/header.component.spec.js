import { async, TestBed } from '@angular/core/testing';
import { HeaderComponent } from '@app-seller/layout/header/header.component';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { AppStateService } from '@app-seller/shared';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
describe('HeaderComponent', function () {
    var component;
    var fixture;
    var ocTokenService = {
        RemoveAccess: jasmine.createSpy('RemoveAccess'),
    };
    var appStateService = { isLoggedIn: new BehaviorSubject(false) };
    var router = { navigate: jasmine.createSpy('navigate'), url: '' };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [HeaderComponent],
            providers: [
                { provide: Router, useValue: router },
                { provide: applicationConfiguration, useValue: {} },
                { provide: AppStateService, useValue: appStateService },
                { provide: OcTokenService, useValue: ocTokenService },
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(HeaderComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('logout', function () {
        beforeEach(function () {
            router.navigate.calls.reset();
        });
        it('should remove token', function () {
            component.logout();
            expect(ocTokenService.RemoveAccess).toHaveBeenCalled();
        });
        it('should refresh current user if user is anonymous', function () {
            appStateService.isLoggedIn.next(true);
            component.logout();
            expect(appStateService.isLoggedIn.value).toEqual(false);
        });
        it('should route to login if user is profiled', function () {
            component.logout();
            expect(router.navigate).toHaveBeenCalledWith(['/login']);
        });
    });
});
//# sourceMappingURL=header.component.spec.js.map