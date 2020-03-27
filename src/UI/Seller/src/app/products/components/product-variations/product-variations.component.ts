import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Variant, SpecOption, Spec } from '@ordercloud/angular-sdk';
import { faExclamationCircle, faCog, faTrash, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { SuperMarketplaceProduct } from 'marketplace-javascript-sdk/dist/models';

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
    superProductEditable?.Product?.ID ? this.canConfigureVariations = true : this.canConfigureVariations = false;
  };
  @Input()
  set superMarketplaceProductStatic(superProductStatic: SuperMarketplaceProduct) {
    this.superProductStatic = superProductStatic;
  };
  @Input() areChanges: boolean;
  @Input() checkForChanges;
  @Input() copyProductResource;
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
  faTimesCircle = faTimesCircle;
  faExclamationCircle = faExclamationCircle;

  constructor(private productService: ProductService) {}
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
    updateProductResourceCopy.Variants[i].xp.NewID = $event.target.value.replace(/[^a-zA-Z0-9 -]/g, "");
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
  }

  addSpec(): void {
    const updateProductResourceCopy = this.productService.copyResource(
        this.superProductEditable || this.productService.emptyResource
    );
    let input = (document.getElementById('AddVariation') as any)
    const newSpec: Spec[] = [{
      ID: `${updateProductResourceCopy.Product.ID}${input.value.split(' ').join('-').replace(/[^a-zA-Z0-9 ]/g, "")}`,
      Name: input.value,
      AllowOpenText: false,
      DefinesVariant: this.definesVariant,
      ListOrder: (updateProductResourceCopy.Specs?.length || 0) + 1,
      Options: []
    }]
    input.value = '';
    updateProductResourceCopy.Specs = newSpec.concat(updateProductResourceCopy.Specs);
    this.superProductEditable = updateProductResourceCopy;
    this.definesVariant = false;
    this.checkForSpecChanges();
    this.productVariationsChanged.emit(this.superProductEditable);
  }
  addSpecOption(spec: Spec, specIndex: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
    let input = (document.getElementById(`${spec.ID}`) as any)
    let markup = (document.getElementById(`${spec.ID}Markup`) as any).value;
    const newOption = [{
      ID: input.value.split(' ').join('-').trim().replace(/[^a-zA-Z0-9 ]/g, ""),
      Value: input.value,
      ListOrder: spec.Options.length + 1,
      IsOpenText: false,
      PriceMarkupType: markup ? 1 : "NoMarkup",
      PriceMarkup: markup,
      xp: null
    }]
    updateProductResourceCopy.Specs[specIndex].Options = newOption.concat(updateProductResourceCopy.Specs[specIndex].Options);
    this.superProductEditable = updateProductResourceCopy;
    this.productVariationsChanged.emit(this.superProductEditable);
    this.mockVariants();
    this.checkForSpecChanges();
  };

  removeSpecOption(specIndex: number, optionIndex: number): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this.superProductEditable || this.productService.emptyResource
    );
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
    for (var i = 1; i < specsDefiningVariants.length; i++) {
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
    let newVariantList = [];
    workingVariantList.forEach(variant => {
      spec.Options.forEach(opt => {
        newVariantList.push({
          ID: `${variant.ID}-${opt.ID}`,
          Name: `${variant.Name} ${opt.Value}`,
          Active: true,
          xp: {
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
}