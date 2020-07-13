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
    toPrimaryHeader: 'Code',
    toSecondaryHeader: 'Description',
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
  catalogs: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ID',
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
  queryRestriction?: string;
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
export const BOOLEAN = 'BOOLEAN';
export const BASIC_STRING = 'BASIC_STRING';
export const DATE_TIME = 'DATE_TIME';
export const CURRENCY = 'CURRENCY';
export const COPY_OBJECT = 'COPY_OBJECT';

export const FULL_TABLE_RESOURCE_DICTIONARY: ResourceConfigurationDictionary = {
  products: {
    fields: [
      {
        path: 'Name',
        header: 'HEADERS.NAME',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Active',
        header: 'HEADERS.ACTIVE',
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
        header: 'HEADERS.NAME',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Active',
        header: 'HEADERS.ACTIVE',
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
        header: 'HEADERS.USERNAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  promotions: {
    fields: [
      {
        path: 'Code',
        header: 'HEADERS.CODE',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Description',
        header: 'HEADERS.DESCRIPTION',
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
        header: 'HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
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
        header: 'HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
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
        header: 'HEADERS.ADDRESS_NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
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
        header: 'HEADERS.CARDHOLDER_NAME',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'CardType',
        header: 'HEADERS.CARD_TYPE',
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
        header: 'HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
  catalogs: {
    fields: [
      {
        path: 'Name',
        header: 'HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
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
        header: 'HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Description',
        header: 'HEADERS.DESCRIPTION',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
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
        header: 'HEADERS.FROM_USER_USERNAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'LineItemCount',
        header: 'HEADERS.NUMBER_OF_LINE_ITEMS',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'Total',
        header: 'HEADERS.TOTAL_AMOUNT',
        type: CURRENCY,
        sortable: true,
      },
      {
        path: 'DateSubmitted',
        header: 'HEADERS.TIME_SUBMITTED',
        type: DATE_TIME,
        sortable: true,
      },
      {
        path: 'Status',
        header: 'HEADERS.STATUS',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Comments',
        header: 'HEADERS.COMMENTS',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'xp.OrderReturnInfo.HasReturn',
        header: 'HEADERS.HAS_CLAIMS',
        type: BOOLEAN,
        sortable: false,
        queryRestriction: 'OrderDirection=Incoming',
      },
      {
        path: 'xp.OrderReturnInfo.Comment',
        header: 'HEADERS.RETURN_COMMENT',
        type: BASIC_STRING,
        sortable: false,
        queryRestriction: 'OrderDirection=Incoming',
      },
    ],
    imgPath: '',
  },
  logs: {
    fields: [
      {
        path: 'timeStamp',
        header: 'HEADERS.TIME_STAMP',
        type: DATE_TIME,
        sortable: true,
      },
      {
        path: 'Action',
        header: 'HEADERS.ACTION',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'RecordType',
        header: 'HEADERS.RECORD_TYPE',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'RecordId',
        header: 'HEADERS.RECORD_ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Level',
        header: 'HEADERS.RESULT',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Copy',
        header: 'HEADERS.COPY',
        type: COPY_OBJECT,
        sortable: false,
      },
    ],
    imgPath: '',
  },
};
