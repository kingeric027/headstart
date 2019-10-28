import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CreditCardFormComponent } from './credit-card-form.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { AppFormErrorService } from '@app/shared';
import { AuthorizeNetService } from '@app/shared/services/authorize-net/authorize-net.service';
import { CreditCardFormatPipe } from '@app/shared/pipes/credit-card-format/credit-card-format.pipe';

describe('CreditCardFormComponent', () => {
  let component: CreditCardFormComponent;
  let fixture: ComponentFixture<CreditCardFormComponent>;
  const thisYear = new Date().getFullYear();
  const thisMonth = new Date().getMonth();
  const formErrorService = {
    hasRequiredError: jasmine.createSpy('hasRequiredError'),
    displayFormErrors: jasmine
      .createSpy('displayFormErrors')
      .and.returnValue(of(null)),
    hasInvalidCreditCardType: jasmine.createSpy('hasInvalidCreditCardType'),
    hasInvalidCreditCardLength: jasmine.createSpy('hasInvalidCreditCardLength'),
    hasInvalidCreditCardNumber: jasmine.createSpy('hasInvalidCreditCardNumber'),
  };
  const authorizeNetService = {
    validateCardNumberFormControl: jasmine
      .createSpy('validateCardNumberFormControl')
      .and.returnValue(of(null)),
    getCardType: jasmine.createSpy('getCardType').and.returnValue('Visa'),
    removeSpacesFrom: jasmine
      .createSpy('removeSpacesFrom')
      .and.returnValue('4653465346534653'),
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CreditCardFormComponent, CreditCardFormatPipe],
      imports: [FontAwesomeModule, ReactiveFormsModule],
      providers: [
        CreditCardFormatPipe,
        { provide: AppFormErrorService, useValue: formErrorService },
        { provide: AuthorizeNetService, useValue: authorizeNetService },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreditCardFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.card = null;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    beforeEach(() => {
      component.ngOnInit();
    });
    it('should set the form values to correct defaults', () => {
      expect(component.cardForm.value).toEqual({
        CardNumber: '',
        CardholderName: '',
        expMonth: `01`,
        expYear: `${thisYear + 1}`.slice(-2),
        CardCode: '',
      });
    });
    it('should set month dropdown options correctly', () => {
      expect(component.monthOptions[0]).toEqual(`01`);
      expect(component.monthOptions.length).toEqual(12);
    });
    it('should set year dropdown options correctly', () => {
      expect(component.yearOptions[0]).toEqual(`${thisYear}`);
      expect(component.yearOptions[19]).toEqual(`${thisYear + 19}`);
      expect(component.yearOptions.length).toEqual(20);
    });
  });

  describe('onSubmit', () => {
    beforeEach(() => {
      component.cardForm.setValue({
        CardNumber: '4653465346534653',
        CardholderName: 'test',
        expMonth: '02',
        expYear: '18',
        CardCode: '627',
      });
    });
    it('should call displayFormErrors if form is invalid', () => {
      component.cardForm.setErrors({ validLength: true });
      component.onSubmit();
      expect(formErrorService.displayFormErrors).toHaveBeenCalled();
    });
    it('should emit the correct value', () => {
      spyOn(component.formSubmitted, 'emit');
      component.onSubmit();
      fixture.detectChanges();
      expect(component.formSubmitted.emit).toHaveBeenCalledWith({
        CardNumber: '4653465346534653',
        CardholderName: 'test',
        ExpirationDate: '0218',
        CardType: 'Visa',
        CardCode: '627',
        PartialAccountNumber: '4653',
      });
    });
  });
});