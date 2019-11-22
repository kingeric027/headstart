import { Component, ViewEncapsulation } from '@angular/core';
import { OCMComponent } from '../../base-component';

@Component({
  templateUrl: './profile-nav.component.html',
  styleUrls: ['./profile-nav.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class OCMProfileNav extends OCMComponent {
  ngOnContextSet() {}
}
