import { Component, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { Variant, SpecOption, Spec, OcSpecService, OcProductService, PartialVariant } from '@ordercloud/angular-sdk';
import { faExclamationCircle, faCog, faTrash, faTimesCircle, faCheckDouble, faPlusCircle, faCaretRight, faCaretDown } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';
import { SuperMarketplaceProduct, Image } from 'marketplace-javascript-sdk/dist/models';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'product-variations-component',
  templateUrl: './product-variations.component.html',
  styleUrls: ['./product-variations.component.scss'],
})
export class ProductVariations {
  @Input()
  set superMarketplaceProductEditable(superProductEditable: SuperMarketplaceProduct) {
    this.superProductEditable = superProductEditable;
    this.variants = superProductEditable?.Variants;
    this.variantInSelection = {};
    this.canConfigureVariations = !!superProductEditable?.Product?.ID;
  };
  @Input()
  set superMarketplaceProductStatic(superProductStatic: SuperMarketplaceProduct) {
    this.superProductStatic = superProductStatic;
  };
  @Input() areChanges: boolean;
  @Input() readonly = false;
  @Input() checkForChanges;
  @Input() copyProductResource;
  @Input() isCreatingNew = false;
  get specsWithVariations() {
    return this.superProductEditable?.Specs?.filter(s => s.DefinesVariant);
  };
  get specsWithoutVariations() {
    return this.superProductEditable?.Specs?.filter(s => !s.DefinesVariant);
  };
  @Output()
  productVariationsChanged = new EventEmitter<SuperMarketplaceProduct>();
  @Output() skuUpdated = new EventEmitter<SuperMarketplaceProduct>();
  @Output() variantsValidated = new EventEmitter<boolean>();
  superProductEditable: SuperMarketplaceProduct;
  superProductStatic: SuperMarketplaceProduct;
  variants: Variant[] = [];
  specOptAdded = new EventEmitter<SpecOption>();
  canConfigureVariations = false;
  areSpecChanges = false;
  definesVariant = false;
  variantsValid = true;
  editSpecs = false;
  faTrash = faTrash;
  faCog = faCog;
  faPlusCircle = faPlusCircle;
  faTimesCircle = faTimesCircle;
  faCheckDouble = faCheckDouble;
  faCaretRight = faCaretRight;
  faCaretDown = faCaretDown;
  faExclamationCircle = faExclamationCircle;
  assignVariantImages = false;
  viewVariantDetails = false;
  variantInSelection: Variant;
  imageInSelection: Image;

  constructor(private productService: ProductService, private toasterService: ToastrService, private ocSpecService: OcSpecService, private changeDetectorRef: ChangeDetectorRef, private _snackBar: MatSnackBar, private ocProductService: OcProductService) {}
  getTotalMarkup = (specOptions: SpecOption[]): number => {
    let totalMarkup = 0;
    if (specOptions) {
      specOptions.forEach(opt => opt.PriceMarkup ? totalMarkup = +totalMarkup + +opt.PriceMarkup : 0);
    }
    return totalMarkup;
  }
  
  toggleEditSpecs(): void {
    this.editSpecs = !this.editSpecs;
  }

  handleDiscardSpecChanges(): void {
    this.editSpecs = !this.editSpecs;
    this.superProductEditable.Specs = this.superProductEditable?.Specs;
    this.checkForSpecChanges();
  }

  checkForSpecChanges(): void {
    this.areSpecChanges = JSON.stringify(this.superProductEditable?.Specs) !== JSON.stringify(this.superProductStatic?.Specs);
  } 

  shouldDefinesVariantBeChecked(): boolean {
    if (this.definesVariant) return true;
    if (this.variants?.length >= 100) return false;
  }

  shouldDisableAddSpecOptBtn(spec: Spec): boolean {
    if (this.variants?.length === 100 && spec.DefinesVariant) return true;
    if (this.variants?.length === 100 && !spec.DefinesVariant) return false;
    if (!this.variantsValid) return true;
  }

  updateSku($event: any, i: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
        this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Variants[i].xp.NewID = $event.target.value.replace(/[^a-zA-Z0-9 -]/g, '');
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
  }

  addSpec(): void {
    const updateProductResourceCopy = this.productService.copyResource(
        this.superProductEditable || this.productService.emptyResource
    );
    const input = (document.getElementById('AddVariation') as any)
    if (input.value === '') {
      this.toasterService.warning('Please name your variation');
      return;
    }
    const newSpec: Spec[] | any = [{
      ID: `${updateProductResourceCopy.Product.ID}${input.value.split(' ').join('-').replace(/[^a-zA-Z0-9 ]/g, '')}`,
      Name: input.value,
      // If this.definesVariant - AllowOptenText _MUST_ be false (platform requirement)
      AllowOpenText: false,
      Required: this.definesVariant,
      DefinesVariant: this.definesVariant,
      ListOrder: (updateProductResourceCopy.Specs?.length || 0) + 1,
      Options: []
    }]
    input.value = '';
    updateProductResourceCopy.Specs = updateProductResourceCopy.Specs.concat(newSpec);
    this.superProductEditable = updateProductResourceCopy;
    this.definesVariant = false;
    this.checkForSpecChanges();
    this.productVariationsChanged.emit(this.superProductEditable);
  }
  addSpecOption(spec: Spec, specIndex: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    const input = (document.getElementById(`${spec.ID}`) as any)
    const markup = (document.getElementById(`${spec.ID}Markup`) as any).value;
    if (input.value === '') {
      this.toasterService.warning('Please name your option');
      return;
    }
    const newOption = [{
      ID: input.value.split(' ').join('-').trim().replace(/[^a-zA-Z0-9 ]/g, ''),
      Value: input.value,
      ListOrder: spec.Options.length + 1,
      IsOpenText: false,
      PriceMarkupType: markup ? 1 : 'NoMarkup',
      PriceMarkup: markup,
      xp: null
    }]
    if (!updateProductResourceCopy.Specs[specIndex].Options.length) updateProductResourceCopy.Specs[specIndex].DefaultOptionID = newOption[0].ID;
    if (!updateProductResourceCopy.Specs[specIndex].DefaultOptionID) updateProductResourceCopy.Specs[specIndex].DefaultOptionID = updateProductResourceCopy.Specs[specIndex].Options[0].ID;
    updateProductResourceCopy.Specs[specIndex].Options = updateProductResourceCopy.Specs[specIndex].Options.concat(newOption);
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
    this.mockVariants();
    this.checkForSpecChanges();
  };

  removeSpecOption(specIndex: number, optionIndex: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    if (updateProductResourceCopy.Specs[specIndex].DefaultOptionID === updateProductResourceCopy.Specs[specIndex].Options[optionIndex].ID) updateProductResourceCopy.Specs[specIndex].DefaultOptionID = null;
    updateProductResourceCopy.Specs[specIndex].Options.splice(optionIndex, 1);
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
    this.mockVariants();
    this.checkForSpecChanges();
  };

  removeSpec(spec: Spec): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Specs = updateProductResourceCopy.Specs.filter(s => s.ID !== spec.ID);
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
    this.mockVariants();
    this.checkForSpecChanges();
  }

  mockVariants(): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Variants = this.generateVariantsFromCurrentSpecs();
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
    this.validateVariants();
    this.variantsValidated.emit(this.variantsValid);
    this.checkForChanges();
  }

  validateVariants(): void {
    this.variantsValid = this.superProductEditable.Variants.length <= 100;
    this.variantsValid = this.superProductEditable?.Variants?.length <= 100;
  }

  generateVariantsFromCurrentSpecs(): Variant[] {
    let specsDefiningVariants = this.specsWithVariations;
    specsDefiningVariants = specsDefiningVariants.sort((a, b) => a.ListOrder - b.ListOrder);
    const firstSpec = specsDefiningVariants[0];
    let variants = this.createVariantsForFirstSpec(firstSpec);
    for (let i = 1; i < specsDefiningVariants.length; i++) {
      variants = this.combineSpecOptions(variants, specsDefiningVariants[i])
    }
    return variants;
  };

  createVariantsForFirstSpec(spec: Spec): Variant[] {
    if (!spec) return [];
    return spec.Options.map(opt => {
      return {
        ID: `${this.superProductEditable.Product.ID}-${opt.ID}`,
        Name: `${this.superProductEditable.Product.ID} ${opt.Value}`,
        Active: true,
        xp: {
          SpecCombo: `${opt.ID}`,
          SpecValues:[{
            SpecName: spec.Name,
            SpecOptionValue: opt.Value,
            PriceMarkup: opt.PriceMarkup
          }]
        }
      }
    })
  }

  combineSpecOptions(workingVariantList: Variant[], spec: Spec): Variant[] {
    const newVariantList = [];
    workingVariantList.forEach(variant => {
      spec.Options.forEach(opt => {
        newVariantList.push({
          ID: `${variant.ID}-${opt.ID}`,
          Name: `${variant.Name} ${opt.Value}`,
          Active: true,
          xp: {
            SpecCombo: `${variant.xp.SpecCombo}-${opt.ID}`,
            SpecValues:[...variant.xp.SpecValues, {
              SpecName: spec.Name,
              SpecOptionValue: opt.Value,
              PriceMarkup: opt.PriceMarkup
            }]
          }
        })
      })
    })
    return newVariantList;
  }

  getPriceMarkup = (specOption: SpecOption): number => !specOption.PriceMarkup ? 0 : specOption.PriceMarkup

  isDefaultSpecOption = (specID: string, optionID: string, specIndex: number): boolean => this.superProductEditable?.Specs[specIndex]?.DefaultOptionID === optionID;
  disableSpecOption = (specIndex: string, option: SpecOption): boolean => this.isCreatingNew ? false : !JSON.stringify(this.superProductStatic?.Specs[specIndex]?.Options)?.includes(JSON.stringify(option));

  stageDefaultSpecOption(specID: string, optionID: string, specIndex: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Specs[specIndex].DefaultOptionID = optionID;
    this.superProductEditable = updateProductResourceCopy;
  }

  async setDefaultSpecOption(specID: string, optionID: string, specIndex: number): Promise<void> {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Specs[specIndex].DefaultOptionID = optionID;
    this.superProductEditable.Specs = updateProductResourceCopy.Specs;
    await this.ocSpecService.Patch(specID, { DefaultOptionID: optionID }).toPromise();
  }
  
  isImageSelected(img: Image): boolean {
    if (!img.Tags) img.Tags = []
    return img.Tags.includes(this.variantInSelection?.xp?.SpecCombo);
  }

  openVariantDetails(variant: Variant): void {
    this.viewVariantDetails = true;
    this.variantInSelection = variant;
  }

  closeVariantDetails(): void {
    this.viewVariantDetails = false;
    this.variantInSelection = null;
  }

  toggleAssignImage(img: Image, specCombo: string): void {
    this.imageInSelection = img;
    if (!this.imageInSelection.Tags) this.imageInSelection.Tags = [];
    this.imageInSelection.Tags.includes(specCombo) ? this.imageInSelection.Tags.splice(this.imageInSelection.Tags.indexOf(specCombo), 1) : this.imageInSelection.Tags.push(specCombo);
  }

  async updateProductImageTags(): Promise<void> {
    this.assignVariantImages = false;
    // Queue up image/content requests, then send them all at aonce
    // TODO: optimize this so we aren't having to update all images, just 'changed' ones
    const requests = this.superProductEditable.Images.map(i => MarketplaceSDK.Images.Post(i));
    await Promise.all(requests);
    // Ensure there is no mistaken change detection
    Object.assign(this.superProductStatic.Images, this.superProductEditable.Images);
    this.imageInSelection = {};
  }

  getVariantImages(variant: Variant): Image[] {
    this.superProductEditable?.Images?.forEach(i => !i.Tags ? i.Tags = [] : null);
    return this.superProductEditable?.Images?.filter(i => i.Tags.includes(variant?.xp?.SpecCombo));
  }

  getVariantDetailColSpan(): number {
    return 3+this.superProductEditable?.Specs?.length;
  }

  async variantShippingDimensionUpdate(event: any, field: string): Promise<void> {
    let partialVariant: PartialVariant = {};
    // If there's no value, or the value didn't change, don't send request.
    if (event.target.value === '') return;
    if (Number(event.target.value) === this.variantInSelection[field]) return;
    const value = Number(event.target.value);
    switch(field) {
      case "ShipWeight": 
        partialVariant = { ShipWeight: value}
        break;
      case "ShipHeight":
        partialVariant = { ShipHeight: value}
        break;
      case "ShipWidth":
        partialVariant = { ShipWidth: value }
        break;
      case "ShipLength":
        partialVariant = { ShipLength: value }
        break;
    }
    try {
      await this.ocProductService.PatchVariant(this.superProductEditable.Product?.ID, this.variantInSelection.ID, partialVariant).toPromise();
      this._snackBar.open("Shipping dimensions updated", "OK", {
        duration: 2000,
      });
    } catch (err) {
      console.log(err)
      this._snackBar.open("Something went wrong", "OK", {
        duration: 2000,
      });
    }
  }

  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
}
