import { FormGroup } from '@angular/forms';

export interface SpecFormEvent {
  type: string;
  valid: boolean;
  form: FormGroup;
}
