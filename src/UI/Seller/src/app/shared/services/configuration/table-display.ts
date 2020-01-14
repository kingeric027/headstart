import { PRODUCT_IMAGE_PATH_STRATEGY } from '../product/product-image.helper';

export interface SummaryResourceInfoPaths {
  toPrimaryHeader: string;
  toSecondaryHeader: string;
  toImage: string;
}

export interface SummaryResourceInfoPathsDictionary {
  [resourceType: string]: SummaryResourceInfoPaths;
}

export const SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY: SummaryResourceInfoPathsDictionary = {
  suppliers: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: 'xp.LogoUrl',
  },
  users: {
    toPrimaryHeader: 'Username',
    toSecondaryHeader: 'ID',
    toImage: '',
  },
  products: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: PRODUCT_IMAGE_PATH_STRATEGY,
  },
  promotions: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'Code',
    toImage: '',
  },
  buyers: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
    toImage: '',
  },
  locations: {
    toPrimaryHeader: 'AddressName',
    toSecondaryHeader: 'ID',
    toImage: '',
  },
  payments: {
    toPrimaryHeader: 'CardholderName',
    toSecondaryHeader: 'CardType',
    toImage: '',
  },
  approvals: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ApprovalRuleID',
    toImage: '',
  },
  orders: {
    toPrimaryHeader: 'FromUser.Username',
    toSecondaryHeader: 'Status',
    toImage: '',
  },
};

export interface ResourceColumnConfiguration {
  path: string;
  header: string;
  type: string;
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

export const FULL_TABLE_RESOURCE_DICTIONARY: ResourceConfigurationDictionary = {
  products: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: STRING_WITH_IMAGE,
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
      },
      {
        path: 'Active',
        header: 'Active',
        type: BASIC_STRING,
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
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
      },
      {
        path: 'Active',
        header: 'Active',
        type: BASIC_STRING,
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
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
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
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
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
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
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
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
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
      },
      {
        path: 'CardType',
        header: 'Card Type',
        type: BASIC_STRING,
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
      },
      {
        path: 'ID',
        header: 'ID',
        type: BASIC_STRING,
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
      },
      {
        path: 'Comments',
        header: 'Comments',
        type: BASIC_STRING,
      },
    ],
    imgPath: '',
  },
};
