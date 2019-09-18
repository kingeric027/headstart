import { Validators } from '@angular/forms';

const HumanNamePattern = Validators.pattern("^[a-zA-Z0-9-.'\\s]*$"); // only alphanumic and space . '
const EmailPattern = Validators.pattern('^.+@.+\\..+$'); // contains @ and . with text surrounding
const PhonePattern = Validators.pattern('^[0-9-]{0,20}$'); // max 20 chars, numbers and -
const DatePattern = Validators.pattern('^[0-9]{2}-[0-9]{2}-[0-9]{4}$'); // mm-dd-yyyy, all numbers
const CAZipPattern = Validators.pattern('^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$');
const USAZipPattern = Validators.pattern('^[0-9]{5}$');

export { HumanNamePattern, EmailPattern, PhonePattern, DatePattern, CAZipPattern, USAZipPattern };
