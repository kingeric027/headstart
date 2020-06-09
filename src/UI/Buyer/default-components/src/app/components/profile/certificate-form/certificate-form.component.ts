import { Component, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DateValidator } from 'src/app/validators/validators';
import { faCalendar } from '@fortawesome/free-solid-svg-icons';

@Component({
  templateUrl: './certificate-form.component.html',
  styleUrls: ['./certificate-form.component.scss'],
})
export class OCMCertificateForm implements OnInit {
  faCalendar = faCalendar;
  form: FormGroup;
  file: string;

  @Output() submitted = new EventEmitter<CertificateFormOutput>();
  @Output() dismissed = new EventEmitter();

  constructor(private cd: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.setForm();
  }

  setForm(): void {
    this.form = new FormGroup({
      SignedDate: new FormControl(null as Date, [Validators.required, DateValidator]),
      ExpirationDate: new FormControl(null as Date, [Validators.required, DateValidator]),
      pdf: new FormControl(null, Validators.required),
    });
  }

  onSubmit(): void {
    const pdf = this.file.replace('data:application/pdf;base64,', '');
    this.submitted.emit({
      SignedDate: this.form.value.SignedDate.toUTCString(),
      ExpirationDate: this.form.value.ExpirationDate.toUTCString(),
      Base64UrlEncodedPDF: pdf,
    });
  }

  onDismiss(): void {
    this.dismissed.emit();
  }

  // Maybe we can make this function more general?
  onFileChange(event: any): void {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = (): void => {
        this.file = reader.result as string;
        // need to run CD since file load runs outside of zone
        this.cd.markForCheck();
      };
    }
  }
}

export interface CertificateFormOutput {
  ExpirationDate: string;
  SignedDate: string;
  Base64UrlEncodedPDF: string;
}
