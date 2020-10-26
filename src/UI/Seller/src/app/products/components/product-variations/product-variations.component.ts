import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, SimpleChanges } from '@angular/core';
import { Variant, SpecOption, Spec, OcSpecService, OcProductService } from '@ordercloud/angular-sdk';
import { faExclamationCircle, faCog, faTrash, faTimesCircle, faCheckDouble, faPlusCircle, faCaretRight, faCaretDown, faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';
import { ToastrService } from 'ngx-toastr';
import { HeadStartSDK, SuperMarketplaceProduct, Asset, MarketplaceVariant } from '@ordercloud/headstart-sdk';
import { AppAuthService } from '@app-seller/auth';
import { SupportedRates } from '@app-seller/shared/models/supported-rates.interface';
import { BehaviorSubject } from 'rxjs';
import { Products } from 'ordercloud-javascript-sdk';
import { truncate } from 'lodash';

@Component({
  selector: 'product-variations-component',
  templateUrl: './product-variations.component.html',
  styleUrls: ['./product-variations.component.scss'],
})
export class ProductVariations implements OnChanges {
  @Input()
  set superMarketplaceProductEditable(superProductEditable: SuperMarketplaceProduct) {
    this.superProductEditable = superProductEditable;
    this.variants.next(superProductEditable?.Variants);
    this.variantInSelection = {};
    this.canConfigureVariations = !!superProductEditable?.Product?.ID;
  };
  @Input()
  set superMarketplaceProductStatic(superProductStatic: SuperMarketplaceProduct) {
    this.superProductStatic = superProductStatic;
  };
  @Input() areChanges: boolean;
  @Input() readonly = false;
  @Input() myCurrency: SupportedRates;
  @Input() checkForChanges;
  @Input() copyProductResource;
  @Input() isCreatingNew = false;
  get specsWithVariations(): Spec[] {
    return this.superProductEditable?.Specs?.filter(s => s.DefinesVariant) as Spec[];
  };
  get specsWithoutVariations(): Spec[] {
    return this.superProductEditable?.Specs?.filter(s => !s.DefinesVariant);
  };
  @Output()
  productVariationsChanged = new EventEmitter<SuperMarketplaceProduct>();
  @Output() skuUpdated = new EventEmitter<SuperMarketplaceProduct>();
  @Output() variantsValidated = new EventEmitter<boolean>();
  @Output() isSpecsEditing = new EventEmitter<boolean>();
  superProductEditable: SuperMarketplaceProduct;
  superProductStatic: SuperMarketplaceProduct;
  variants: BehaviorSubject<MarketplaceVariant[]>;
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
  faCheckCircle = faCheckCircle;
  faExclamationCircle = faExclamationCircle;
  assignVariantImages = false;
  viewVariantDetails = false;
  variantInSelection: Variant;
  imageInSelection: Asset;

  constructor(
    private productService: ProductService, 
    private toasterService: ToastrService, 
    private ocSpecService: OcSpecService, 
    private changeDetectorRef: ChangeDetectorRef, 
    private ocProductService: OcProductService, 
    private appAuthService: AppAuthService) {
      this.variants = new BehaviorSubject<MarketplaceVariant[]>([]);
    }
  
  ngOnChanges(changes: SimpleChanges): void {
     if (changes?.superMarketplaceProductEditable) {
       this.variants.next(changes?.superMarketplaceProductEditable?.currentValue?.Variants);
     }
  }

  getTotalMarkup = (specOptions: SpecOption[]): number => {
    let totalMarkup = 0;
    if (specOptions) {
      specOptions.forEach(opt => opt.PriceMarkup ? totalMarkup = +totalMarkup + +opt.PriceMarkup : 0);
    }
    return totalMarkup;
  }

  toggleEditSpecs(): void {
    this.editSpecs = !this.editSpecs;
    this.isSpecsEditing.emit(this.editSpecs);
  }

  handleDiscardSpecChanges(): void {
    this.editSpecs = !this.editSpecs;
    this.superProductEditable.Specs = this.superProductEditable?.Specs;
    this.isSpecsEditing.emit(this.editSpecs);
    this.checkForSpecChanges();
  }

  checkForSpecChanges(): void {
    this.areSpecChanges = JSON.stringify(this.superProductEditable?.Specs) !== JSON.stringify(this.superProductStatic?.Specs);
  }

  shouldDefinesVariantBeChecked(): boolean {
    if (this.definesVariant) return true;
    if (this.variants?.getValue()?.length >= 100) return false;
  }

  shouldDisableAddSpecOptBtn(spec: Spec): boolean {
    if (!this.variantsValid) {
      return true;
    } else {
      return this.variants?.getValue().length === 100 && spec.DefinesVariant;
    }
  }

  toggleActive(variant: Variant): void {
    variant.Active = !variant.Active;

    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Variants.find(x => x.ID === variant.ID).Active = variant.Active
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
  }

  getVariantStatusDisplay(variant: Variant): string {
    if (variant.Active) {
      return 'Active';
    } else {
      return 'Inactive';
    }
  }

  updateSku($event: any, i: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Variants[i].xp.NewID = $event.target.value.replace(/[^a-zA-Z0-9_-]/g, '');
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
  }

  updateVariantInventory($event: any, i: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    if (updateProductResourceCopy.Variants[i].Inventory === null || updateProductResourceCopy.Variants[i].Inventory === undefined) {
      updateProductResourceCopy.Variants[i].Inventory = { QuantityAvailable: 0 };
    }
    updateProductResourceCopy.Variants[i].Inventory.QuantityAvailable = Number($event.target.value);
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
          SpecValues: [{
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
            SpecValues: [...variant.xp.SpecValues, {
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

  isImageSelected(img: Asset): boolean {
    if (!img.Tags) img.Tags = []
    return img.Tags.includes(this.variantInSelection?.xp?.SpecCombo);
  }

  openVariantDetails(variant: Variant): void {
    const variantBehaviorSubjectValue = this.variants?.getValue();
    this.viewVariantDetails = true;
    this.variantInSelection = variant;
    if (variantBehaviorSubjectValue !== null) {
      this.variantInSelection = variantBehaviorSubjectValue[variantBehaviorSubjectValue?.indexOf(variant)];
    }
  }

  closeVariantDetails(): void {
    this.viewVariantDetails = false;
    this.variantInSelection = null;
  }

  toggleAssignImage(img: Asset, specCombo: string): void {
    this.imageInSelection = img;
    if (!this.imageInSelection.Tags) this.imageInSelection.Tags = [];
    this.imageInSelection.Tags.includes(specCombo) ? this.imageInSelection.Tags.splice(this.imageInSelection.Tags.indexOf(specCombo), 1) : this.imageInSelection.Tags.push(specCombo);
  }

  async updateProductImageTags(): Promise<void> {
    this.assignVariantImages = false;
    // Queue up image/content requests, then send them all at aonce
    // TODO: optimize this so we aren't having to update all images, just 'changed' ones
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const requests = this.superProductEditable.Images.map(i => HeadStartSDK.Assets.Save(i.ID, i, accessToken));
    await Promise.all(requests);
    // Ensure there is no mistaken change detection
    Object.assign(this.superProductStatic.Images, this.superProductEditable.Images);
    this.imageInSelection = {};
  }

  getVariantImages(variant: Variant): Asset[] {
    this.superProductEditable?.Images?.forEach(i => !i.Tags ? i.Tags = [] : null);
    return this.superProductEditable?.Images?.filter(i => i.Tags.includes(variant?.xp?.SpecCombo));
  }

  getVariantDetailColSpan(): number {
    const colSpan = 4 + this.superProductEditable?.Specs?.length;
    if (this.superProductEditable?.Product?.Inventory?.VariantLevelTracking) colSpan + 1;
    return colSpan;
  }

  async variantShippingDimensionUpdate(event: any, field: string, index: number): Promise<void> {
    let partialVariant: Variant = {};
    // If there's no value, or the value didn't change, don't send request.
    if (event.target.value === '') return;
    if (Number(event.target.value) === this.variantInSelection[field]) return;
    const value = Number(event.target.value);
    switch (field) {
      case 'ShipWeight':
        partialVariant = { ShipWeight: value }
        break;
      case 'ShipHeight':
        partialVariant = { ShipHeight: value }
        break;
      case 'ShipWidth':
        partialVariant = { ShipWidth: value }
        break;
      case 'ShipLength':
        partialVariant = { ShipLength: value }
        break;
    }
    try {
      const patchedVariant = await Products.PatchVariant<MarketplaceVariant>(this.superProductEditable.Product?.ID, this.variantInSelection.ID, partialVariant);
      const variants = this.variants?.getValue();
      if (variants !== null) {
        variants[index] = JSON.parse(JSON.stringify(patchedVariant));
        this.variants.next(variants)
        this.variantInSelection = this.variants?.getValue()[index];
      }
      this.toasterService.success('Shipping dimensions updated', 'OK');
    } catch (err) {
      console.log(err)
      this.toasterService.error('Something went wrong', 'Error');
    }
  }
}
