import { LineItemSpec } from '@ordercloud/angular-sdk';

export interface SpecFormEvent {
  type: string;
  specs: Array<LineItemSpec>;
  valid: boolean;
  markup: number;
}
