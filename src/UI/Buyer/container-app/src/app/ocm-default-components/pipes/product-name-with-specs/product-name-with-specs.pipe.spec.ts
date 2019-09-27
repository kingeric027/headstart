import { ProductNameWithSpecsPipe } from './product-name-with-specs.pipe';

describe('ProductSpecsDisplayPipe', () => {
  it('create an instance', () => {
    const pipe = new ProductNameWithSpecsPipe();
    expect(pipe).toBeTruthy();
  });
});
