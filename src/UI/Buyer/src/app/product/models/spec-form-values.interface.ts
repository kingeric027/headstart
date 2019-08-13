import { LineItemSpec } from '@ordercloud/angular-sdk';

export interface SpecFormEvent {
  type: string;
  quantity: number;
  specs: Array<LineItemSpec>;
  valid: boolean;
  price: number;
}
