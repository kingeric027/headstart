import { Component } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateNativeAdapter, NgbDateCustomParserFormatter } from 'src/app/config/date-picker.config';
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
  isLoggedIn: boolean;

  constructor(private currentUser: CurrentUserService) {
    this.currentUser.onIsLoggedInChange((isLoggedIn) => (this.isLoggedIn = isLoggedIn));
  }
}
