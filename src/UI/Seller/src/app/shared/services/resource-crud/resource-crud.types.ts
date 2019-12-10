import { Meta } from '@ordercloud/angular-sdk';

export interface Options {
  page?: number;
  sortBy?: string;
  search?: string;
  filters?: FilterDictionary;
}

export interface FilterDictionary {
  [filterKey: string]: string;
}

export interface ListResource<ResourceType> {
  Meta: Meta;
  Items: ResourceType[];
}

export const GETTING_NEW_ITEMS = 'GETTING_NEW_ITEMS';
export type GETTING_NEW_ITEMS = typeof GETTING_NEW_ITEMS;

export const REFRESHING_ITEMS = 'REFRESHING_ITEMS';
export type REFRESHING_ITEMS = typeof REFRESHING_ITEMS;

export const SUCCESSFUL_WITH_ITEMS = 'SUCCESSFUL_WITH_ITEMS';
export type SUCCESSFUL_WITH_ITEMS = typeof SUCCESSFUL_WITH_ITEMS;

export const SUCCESSFUL_NO_ITEMS_NO_FILTERS = 'SUCCESSFUL_NO_ITEMS_NO_FILTERS';
export type SUCCESSFUL_NO_ITEMS_NO_FILTERS = typeof SUCCESSFUL_NO_ITEMS_NO_FILTERS;

export const SUCCESSFUL_NO_ITEMS_WITH_FILTERS = 'SUCCESSFUL_NO_ITEMS_WITH_FILTERS';
export type SUCCESSFUL_NO_ITEMS_WITH_FILTERS = typeof SUCCESSFUL_NO_ITEMS_WITH_FILTERS;

export const ERROR = 'ERROR';
export type ERROR = typeof ERROR;

export type RequestStatus =
  | GETTING_NEW_ITEMS
  | REFRESHING_ITEMS
  | SUCCESSFUL_WITH_ITEMS
  | SUCCESSFUL_NO_ITEMS_NO_FILTERS
  | SUCCESSFUL_NO_ITEMS_WITH_FILTERS
  | ERROR;
