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
