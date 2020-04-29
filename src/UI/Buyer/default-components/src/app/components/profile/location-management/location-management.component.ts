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
import { ActivatedRoute } from '@angular/router';


@Component({
  templateUrl: './location-management.component.html',
  styleUrls: ['./location-management.component.scss'],
})
export class OCMLocationManagement implements OnInit {
  faDownload = faDownload;
  address: MarketplaceAddressBuyer = {};
  certificate: TaxCertificate = {};
  showCertificateForm = false;
  userCanAdmin = false;
  _locationID = '';
  @Input() set locationID(locationID: string) {
    this._locationID = locationID;
    this.getLocationManagementDetails();
  };

  constructor(private context: ShopperContextService, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.userCanAdmin = this.context.currentUser.hasRoles('AddressAdmin');
  }
  
  async getLocationManagementDetails(): Promise<void> {
    this.address = await this.context.addresses.get(this._locationID);
    this.certificate = await this.context.addresses.getCertificate(this.address);
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
