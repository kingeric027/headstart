import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Meta } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './generic-list.component.html',
  styleUrls: ['./generic-list.component.scss'],
})
export class OCMGenericList {
  /**
   *  Nearly every endpoint in the OrderCloud API can be passed a common set of options.
   *  This includes things like search, filter, orderBy, & page. The idea behind this Component
   *  is to package the UI elements assosiated with those actions in a single place.
   *
   *  Right now it includes functionality for searching and paginating a list of objects, for example, addresses.
   */
  constructor() {}

  // Information about pagination
  @Input() meta: Meta;

  @Input() searchPlaceholder: string;

  // Event to capture search changes or page changes
  @Output()
  requestOptionsUpdated = new EventEmitter<{
    page?: number;
    search?: string;
  }>();
}
