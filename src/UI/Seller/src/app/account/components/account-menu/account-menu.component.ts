import { Component, AfterViewChecked, ChangeDetectorRef, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { getPsHeight } from '@app-seller/shared/services/dom.helper';
import { faUserCircle, faEnvelope } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'account-menu',
  templateUrl: './account-menu.component.html',
  styleUrls: ['./account-menu.component.scss'],
})
export class AccountMenuComponent implements AfterViewChecked, OnInit {
  menuHeight: number;
  activePage: string = undefined; /* undefined defaults to the root page 'account' */
  faUserCircle = faUserCircle;
  faEnvelope = faEnvelope;
  constructor(private router: Router, activatedRoute: ActivatedRoute, private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.setActivePage();
  }

  ngAfterViewChecked(): void {
    this.menuHeight = getPsHeight();
    this.changeDetectorRef.detectChanges();
  }

  setActivePage(): void {
    const routeUrl = this.router.routerState.snapshot.url;
    const splitUrl = routeUrl.split('/');
    this.activePage = splitUrl[2];
  }

  toPage(page?: string): void {
    page ? this.router.navigate(['account', page]) : this.router.navigate(['account']);
  }
}
