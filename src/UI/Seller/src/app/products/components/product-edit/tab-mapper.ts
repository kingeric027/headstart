export function setProductEditTab(section: string, readonly: boolean): number {
  const index = ProductEditTabMapper[section];
  return readonly ? index : index - 1;
}

export const ProductEditTabMapper = {
  undefined: 0,
  'catalog-assignments': 1,
  description: 2,
  price: 3,
  filters: 4,
  variants: 5,
  'images-and-documents': 6,
};

export const TabIndexMapper = {
  1: 'catalog-assignments',
  2: 'description',
  3: 'price',
  4: 'filters',
  5: 'variants',
  6: 'images-and-documents',
};
