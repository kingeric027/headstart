import { async, TestBed } from '@angular/core/testing';
import { CarouselSlideDisplayComponent } from '@app-seller/shared/components/carousel-slide-display/carousel-slide-display.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule, FormsModule, FormBuilder } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
describe('CarouselSlideDisplayComponent', function () {
    var component;
    var fixture;
    var toastrService = { warning: jasmine.createSpy('warning') };
    var mockSlide = {
        URL: 'http://example.com',
        headerText: 'text',
        bodyText: 'body',
    };
    var formErrorService = {
        hasPatternError: jasmine.createSpy('hasPatternError'),
    };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            imports: [FontAwesomeModule, ReactiveFormsModule, FormsModule],
            providers: [
                FormBuilder,
                { provide: ToastrService, useValue: toastrService },
                { provide: AppFormErrorService, useValue: formErrorService },
            ],
            declarations: [CarouselSlideDisplayComponent],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(CarouselSlideDisplayComponent);
        component = fixture.componentInstance;
        component.slide = mockSlide;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('ngOnInit', function () {
        it('should setup form', function () {
            expect(component.carouselForm.value).toEqual(mockSlide);
        });
    });
    describe('formUnchanged', function () {
        it('should be true before form changes', function () {
            component.slide = mockSlide;
            expect(component.saveDisabled()).toEqual(true);
        });
        it('should be false after form changes', function () {
            component.slide = {
                URL: 'http://example.com',
                headerText: 'new text',
                bodyText: 'body',
            };
            fixture.detectChanges();
            expect(component.saveDisabled()).toEqual(false);
        });
    });
    describe('textChanges', function () {
        beforeEach(function () {
            component.slide = mockSlide;
            spyOn(component.save, 'emit');
            component.textChanges();
        });
        it('if text hasnt changed do nothing', function () {
            expect(component.save.emit).not.toHaveBeenCalled();
        });
        it('if text has changed emit', function () {
            component.slide = {
                URL: 'http://example.com',
                headerText: 'new text',
                bodyText: 'body',
            };
            fixture.detectChanges();
            component.textChanges();
            expect(component.save.emit).toHaveBeenCalledWith({
                new: mockSlide,
                prev: {
                    URL: 'http://example.com',
                    headerText: 'new text',
                    bodyText: 'body',
                },
            });
        });
    });
    describe('deleteSlide', function () {
        it('should emit delete event', function () {
            spyOn(component.delete, 'emit');
            component.deleteSlide();
            expect(component.delete.emit).toHaveBeenCalledWith({
                prev: component.slide,
            });
        });
    });
});
//# sourceMappingURL=carousel-slide-display.component.spec.js.map