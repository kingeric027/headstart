import { ValidatorFn } from '@angular/forms';

export interface FieldConfig {
  disabled?: boolean;
  label?: string;
  name: string;
  options?: any[];
  placeholder?: string;
  min?: number;
  max?: number;
  step?: number;
  rows?: number;
  type: string;
  validation?: ValidatorFn[];
  value?: any;
  currency?: string;
}
