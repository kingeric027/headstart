import { Meta } from '@ordercloud/angular-sdk';

export interface ListPage<T> {
  Meta: Meta;
  Items: T[];
}
