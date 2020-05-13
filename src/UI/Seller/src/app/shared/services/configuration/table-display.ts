import { PRODUCT_IMAGE_PATH_STRATEGY } from '@app-seller/products/product-image.helper';

export interface SummaryResourceInfoPaths {
  toPrimaryHeader: string;
  toSecondaryHeader: string;
  toImage: string;
  toExpandable: boolean;
}

export interface SummaryResourceInfoPathsDictionary {
  [resourceType: string]: SummaryResourceInfoPaths;
}

export const SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY: SummaryResourceInfoPathsDictionary = {
  suppliers: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: 'xp.LogoUrl',
    toExpandable: false,
  },
  users: {
    toPrimaryHeader: 'Username',
    toSecondaryHeader: 'ID',
    toImage: '',
    toExpandable: false,
  },
  products: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: PRODUCT_IMAGE_PATH_STRATEGY,
    toExpandable: false,
  },
  promotions: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'Code',
    toImage: '',
    toExpandable: false,
  },
  facets: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: '',
    toExpandable: false,
  },
  buyers: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: '',
    toExpandable: false,
  },
  locations: {
    toPrimaryHeader: 'AddressName',
    toSecondaryHeader: 'ID',
    toImage: '',
    toExpandable: false,
  },
  payments: {
    toPrimaryHeader: 'CardholderName',
    toSecondaryHeader: 'CardType',
    toImage: '',
    toExpandable: false,
  },
  approvals: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ApprovalRuleID',
    toImage: '',
    toExpandable: false,
  },
  categories: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: '',
    toExpandable: true,
  },
  orders: {
    toPrimaryHeader: 'ID',
    toSecondaryHeader: 'Status',
    toImage: '',
    toExpandable: false,
  },
};

export interface ResourceColumnConfiguration {
  path: string;
  header: string;
  type: string;
  sortable: boolean;
}

export interface ResourceConfiguration {
  fields: ResourceColumnConfiguration[];
  imgPath?: string;
}

export interface ResourceCell {
  type: string;
  value: any;
}

export interface ResourceRow {
  resource: any;
  cells: ResourceCell[];
  imgPath?: string;
}

export interface ResourceConfigurationDictionary {
  [resourceType: string]: ResourceConfiguration;
}

export const STRING_WITH_IMAGE = 'STRING_WITH_IMAGE';
export const BASIC_STRING = 'BASIC_STRING';
export const DATE_TIME = 'DATE_TIME';
export const CURRENCY = 'CURRENCY';
export const COPY_OBJECT = 'COPY_OBJECT';

export const FULL_TABLE_RESOURCE_DICTIONARY: ResourceConfigurationDictionary = {
  products: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Active',
        header: 'Active',
        type: BASIC_STRING,
        sortable: false,
      },
    ],
    imgPath: PRODUCT_IMAGE_PATH_STRATEGY,
  },
  suppliers: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Active',
        header: 'Active',
        type: BASIC_STRING,
        sortable: false,
      },
    ],
    imgPath: 'xp.LogoUrl',
  },
  users: {
    fields: [
      {
        path: 'Username',
        header: 'Username',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  promotions: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  facets: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  buyers: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  locations: {
    fields: [
      {
        path: 'AddressName',
        header: 'Address Name',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  payments: {
    fields: [
      {
        path: 'CardholderName',
        header: 'Cardholder Name',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'CardType',
        header: 'Card Type',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  approvals: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  categories: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Description',
        header: 'Description',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  orders: {
    fields: [
      {
        path: 'FromUser.Username',
        header: 'From User Username',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'LineItemCount',
        header: '# of Line Items',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'Total',
        header: 'Total Amount (USD)',
        type: CURRENCY,
        sortable: true,
      },
      {
        path: 'DateSubmitted',
        header: 'Time Submitted',
        type: DATE_TIME,
        sortable: true,
      },
      {
        path: 'Status',
        header: 'Status',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Comments',
        header: 'Comments',
        type: BASIC_STRING,
        sortable: false,
      },
    ],
    imgPath: '',
  },
  logs: {
    fields: [
      {
        path: 'timeStamp',
        header: 'Time Stamp',
        type: DATE_TIME,
        sortable: true,
      },
      {
        path: 'Action',
        header: 'Action',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'RecordType',
        header: 'Record Type',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'RecordId',
        header: 'Record ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Level',
        header: 'Result',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Copy',
        header: 'Copy',
        type: COPY_OBJECT,
        sortable: false,
      },
    ],
    imgPath: '',
  },
};
