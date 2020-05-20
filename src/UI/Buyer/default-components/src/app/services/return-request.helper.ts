// team-form.service.ts



// import { Injectable } from '@angular/core'
// import { Observable, BehaviorSubject } from 'rxjs'
// import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms'
// import { ReturnRequestForm } from './_models'
// import { MarketplaceLineItem } from 'marketplace'

// @Injectable()
// export class ReturnRequestFormService {
//   private returnRequestForm: BehaviorSubject<FormGroup | undefined> = 
//     new BehaviorSubject(this.fb.group(
//       new ReturnRequestForm(this.orderID) //(new ReturnRequestForm)
//     ));

//   returnRequestForm$: Observable<FormGroup> = this.returnRequestForm.asObservable();

//   constructor(private fb: FormBuilder,
//               private orderID: string) {}
              
//   addLineItemGroup(lineItems: MarketplaceLineItem[]) { //add line item group.  This should take in an array of line items
//     const currentReturnRequest = this.returnRequestForm.getValue();
//     const currentLineItemGroups = currentReturnRequest.get('lineItemGroups') as FormArray; //get line item groups
//     currentLineItemGroups.push(
//       this.fb.group(
//         new LineItemGroupForm(lineItems) //new line item group (new LineItemGroupForm)
//       )
//     );
//     this.returnRequestForm.next(currentReturnRequest);
//   }

// } 



// end class