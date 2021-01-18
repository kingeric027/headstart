import { Component, OnInit, Input } from '@angular/core'
import { UserGroup, ProductAssignment } from '@ordercloud/angular-sdk'
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons'
import { HSProduct } from '@ordercloud/headstart-sdk'

@Component({
  selector: 'variable-print-configuration-component',
  templateUrl: './variable-print-configuration.component.html',
  styleUrls: ['./variable-print-configuration.component.scss'],
})
export class VariablePrintConfiguration implements OnInit {
  @Input()
  product: HSProduct
  faExclamationCircle = faExclamationCircle

  _buyerID = ''
  _productID = ''
  _folderName = ''
  catalogs: UserGroup[] = []
  catalogAssignments: ProductAssignment[][] = []

  @Input()
  set productID(value: string) {
    this._productID = value
  }

  @Input()
  set folderName(value: string) {
    this._folderName = value
  }

  @Input()
  set catalogID(value: string) {
    this._buyerID = value
  }

  @Input()
  set availableCatelogs(value: UserGroup[]) {
    this.catalogs = value
  }

  @Input()
  set productAssignments(value: ProductAssignment[][]) {
    this.catalogAssignments = value
  }

  constructor() {}

  ngOnInit(): void {}
}
