import { Component, Input, OnInit } from '@angular/core';
import { Address,  } from '@ordercloud/angular-sdk';
import { BuyerLocationWithCert, ShopperContextService } from 'marketplace';
import { faDownload } from '@fortawesome/free-solid-svg-icons';

import {
  MarketplaceAddressBuyer,
  TaxCertificate,
} from '../../../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';
import { CertificateFormOutput } from '../certificate-form/certificate-form.component';
import { GeographyConfig } from '../../../config/geography.class';


@Component({
  templateUrl: './location-details.component.html',
  styleUrls: ['./location-details.component.scss'],
})
export class OCMLocationDetails implements OnInit {
  faDownload = faDownload;
  address: MarketplaceAddressBuyer;
  certificate: TaxCertificate;
  showCertificateForm = false;
  userCanAdmin = false;

  @Input() set location(value: BuyerLocationWithCert) {
    this.address = value.location;
    this.certificate = value.certificate;
  }
  @Input() highlight?: boolean;

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.userCanAdmin = this.context.currentUser.hasRoles('AddressAdmin');
  }

  // make into pipe?
  getFullName(address: Address): string {
    const fullName = `${address?.FirstName || ''} ${address?.LastName || ''}`;
    return fullName.trim();
  }

  async saveCertificate(formOutput: CertificateFormOutput): Promise<void> {
    // TODO - how do tax certs work in Canada?
    const state = GeographyConfig.getStates('US').find(s => this.address.State === s.abbreviation); 
    const certificate: TaxCertificate = {
      ExpirationDate: formOutput.ExpirationDate,
      SignedDate: formOutput.ExpirationDate,
      Base64UrlEncodedPDF: formOutput.Base64UrlEncodedPDF,
      ExposureZoneName: state.label,
      ExemptionNumber: this.address.ID
    }; 
    this.certificate = await this.context.addresses.createCertificate(this.address.ID, certificate);
    this.dismissCertificateForm();
  }

  dismissCertificateForm(): void {
    this.showCertificateForm = false;
  }

  revealCertificateForm(): void {
    this.showCertificateForm = true;
  }

  hasCertificate(): boolean {
    return (this.certificate !== null && this.certificate !== undefined);
  }
}
