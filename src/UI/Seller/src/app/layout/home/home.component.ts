import { Component, OnInit, Inject } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor(
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  ngOnInit() {
  }

}
