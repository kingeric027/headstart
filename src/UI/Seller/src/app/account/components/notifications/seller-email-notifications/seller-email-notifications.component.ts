import { Component, Output, EventEmitter, Input } from '@angular/core'
import {
  faExclamationCircle,
  faTimesCircle,
} from '@fortawesome/free-solid-svg-icons'
import { ToastrService } from 'ngx-toastr'
import { MeUser } from '@ordercloud/angular-sdk'
import { FormGroup } from '@angular/forms'

@Component({
  selector: 'seller-email-notifications',
  templateUrl: './seller-email-notifications.component.html',
  styleUrls: ['./seller-email-notifications.component.scss'],
})
export class SellerEmailNotifications {
  constructor(private toastrService: ToastrService) {}
  faExclamationCircle = faExclamationCircle
  faTimesCircle = faTimesCircle
  // Inputs
  @Input()
  user: MeUser
  @Input()
  sellerUserForm: FormGroup
  // Event Outputs
  @Output()
  toggleReceiveOrderEmails = new EventEmitter<boolean>()
  @Output()
  toggleReceiveProductEmails = new EventEmitter<boolean>()
  @Output()
  toggleReceiveRequestInfoEmails = new EventEmitter<boolean>()
  @Output()
  addRcpt = new EventEmitter<void>()
  @Output()
  removeRcpt = new EventEmitter<number>()
  // Ouput Functions
  toggleOrderEmails = (event: Event): void =>
    this.toggleReceiveOrderEmails.emit(
      (event.target as HTMLInputElement).checked
    )
  toggleRequestInfoEmails = (event: Event): void =>
    this.toggleReceiveRequestInfoEmails.emit(
      (event.target as HTMLInputElement).checked
    )

  toggleProductEmails = (event: Event): void =>
    this.toggleReceiveProductEmails.emit(
      (event.target as HTMLInputElement).checked
    )

  removeAddtlRcpt = (index: number): void => this.removeRcpt.emit(index)
  addAddtlRcpt = (): void => this.addRcpt.emit()
}
