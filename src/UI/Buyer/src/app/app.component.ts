import { Component } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateNativeAdapter, NgbDateCustomParserFormatter } from '@app-buyer/config/date-picker.config';
import { Observable } from 'rxjs';
import { CurrentUserService } from './shared/services/current-user/current-user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [
    // allows us to use native date object when interacting with ngb-datepicker
    { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter },
    // defines date format as mm/dd/yyyy
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
  ],
})
export class AppComponent {
  isLoggedIn$: Observable<boolean>;

  constructor(private currentUser: CurrentUserService) {
    this.isLoggedIn$ = this.currentUser.loggedInSubject;
  }
}
