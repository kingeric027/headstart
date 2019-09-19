import { AbstractControl, ValidationErrors } from '@angular/forms';

export const ErrorDictionary = {
  name: "Name can only contain characters Aa-Zz 0-9 - ' .",
  phone: 'Phone can only contain 20 numbers or "-" chars (no spaces)',
  zip: 'Zip Code is invalid',
  DateError: 'Enter date of the form mm-dd-yyyy',
  date: 'Enter date of the form mm-dd-yyyy',
  required: 'This field is required',
  email: 'Please enter a valid email',
  ocMatchFields: "Passwords don't match",
  strongPassword:
    'Password must be at least eight characters long and include at least one letter and one number. Password can also include special characters.',
};

// only alphanumic and space . '
export function ValidateName(control: AbstractControl): ValidationErrors | null {
  const isValid = /^[a-zA-Z0-9-.'\\s]*$/.test(control.value);
  if (!control.value || isValid) {
    return null;
  }
  return { name: true };
}

// max 20 chars, numbers and -
export function ValidatePhone(control: AbstractControl): ValidationErrors | null {
  const isValid = /^[0-9-]{0,20}$/.test(control.value);
  if (!control.value || isValid) {
    return null;
  }
  return { phone: true };
}

// contains @ and . with text surrounding
export function ValidateEmail(control: AbstractControl): ValidationErrors | null {
  const isValid = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(control.value);
  if (!control.value || isValid) {
    return null;
  }
  return { email: true };
}

// mm-dd-yyyy, all numbers
export function ValidateDate(control: AbstractControl): ValidationErrors | null {
  const isValid = /^[0-9]{2}-[0-9]{2}-[0-9]{4}$/.test(control.value);
  if (!control.value || isValid) {
    return null;
  }
  return { date: true };
}

export function ValidateCAZip(control: AbstractControl): ValidationErrors | null {
  const isValid = /^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$/.test(control.value);
  if (!control.value || isValid) {
    return null;
  }
  return { zip: true };
}

export function ValidateUSZip(control: AbstractControl): ValidationErrors | null {
  const isValid = /^[0-9]{5}$/.test(control.value);
  if (!control.value || isValid) {
    return null;
  }
  return { zip: true };
}

export function ValidateStrongPassword(control: AbstractControl): ValidationErrors | null {
  const hasNumber = /[0-9]/.test(control.value); // TODO - boil this into one regex
  const hasLetter = /[a-zA-Z]/.test(control.value);
  const hasMinLength = control.value && control.value.length >= 8;
  if (!control.value) {
    return null;
  }
  if (hasNumber && hasLetter && hasMinLength) {
    return null;
  }
  return { strongPassword: true };
}

/**
 * Our date inputs use ngbDatepicker but also allow freeform entry.
 * We need to validate the free form entry strings which are converted to date objects
 */

export function DateValidator(control: AbstractControl): ValidationErrors | null {
  // only validate if both fields have been touched
  if (control.value == null || control.value === '') {
    return null;
  }

  if (
    // the user's text input is converted to Date() if days and months check out
    control.value instanceof Date &&
    // validate that the year is also within reasonable range
    control.value.getFullYear().toString().length === 4
  ) {
    return null;
  }

  return { DateError: true };
}
