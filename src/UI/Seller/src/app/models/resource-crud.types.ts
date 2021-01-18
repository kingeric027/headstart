import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs'

export interface Options extends ListArgs {
  OrderDirection?: 'Incoming' | 'Outgoing',
  searchType?: 'AnyTerm' | 'AllTermsAnyField' | 'AllTermsSameField' | 'ExactPhrase' | 'ExactPhrasePrefix'
}

export const GETTING_NEW_ITEMS = 'GETTING_NEW_ITEMS'
export type GETTING_NEW_ITEMS = typeof GETTING_NEW_ITEMS

export const REFRESHING_ITEMS = 'REFRESHING_ITEMS'
export type REFRESHING_ITEMS = typeof REFRESHING_ITEMS

export const FETCHING_SUBSEQUENT_PAGES = 'FETCHING_SUBSEQUENT_PAGES'
export type FETCHING_SUBSEQUENT_PAGES = typeof FETCHING_SUBSEQUENT_PAGES

export const SUCCESSFUL_WITH_ITEMS = 'SUCCESSFUL_WITH_ITEMS'
export type SUCCESSFUL_WITH_ITEMS = typeof SUCCESSFUL_WITH_ITEMS

export const SUCCESSFUL_NO_ITEMS_NO_FILTERS = 'SUCCESSFUL_NO_ITEMS_NO_FILTERS'
export type SUCCESSFUL_NO_ITEMS_NO_FILTERS = typeof SUCCESSFUL_NO_ITEMS_NO_FILTERS

export const SUCCESSFUL_NO_ITEMS_WITH_FILTERS =
  'SUCCESSFUL_NO_ITEMS_WITH_FILTERS'
export type SUCCESSFUL_NO_ITEMS_WITH_FILTERS = typeof SUCCESSFUL_NO_ITEMS_WITH_FILTERS

export const ERROR = 'ERROR'
export type ERROR = typeof ERROR

export type RequestStatus =
  | GETTING_NEW_ITEMS
  | REFRESHING_ITEMS
  | FETCHING_SUBSEQUENT_PAGES
  | SUCCESSFUL_WITH_ITEMS
  | SUCCESSFUL_NO_ITEMS_NO_FILTERS
  | SUCCESSFUL_NO_ITEMS_WITH_FILTERS
  | ERROR