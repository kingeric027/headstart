import { PRODUCT_IMAGE_PATH_STRATEGY } from '@app-seller/products/product-image.helper';
import { SUPPLIER_LOGO_PATH_STRATEGY } from '@app-seller/suppliers/supplier-logo.helper';

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
    toImage: SUPPLIER_LOGO_PATH_STRATEGY,
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
  kitproducts: {
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
    toSecondaryHeader: 'xp.SubmittedOrderStatus',
    toImage: '',
    toExpandable: false,
  },
  storefronts: {
    toPrimaryHeader: 'AppName',
    toSecondaryHeader: 'ID',
    toImage: '',
    toExpandable: false,
  },
  templates: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ReportCategory',
    toImage: '',
    toExpandable: false,
  },
  reports: {
    toPrimaryHeader: 'Name',
    toSecondaryHeader: 'ReportCategory',
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
export const IMPERSONATE_BUTTON = 'IMPERSONATE_BUTTON';

export const FULL_TABLE_RESOURCE_DICTIONARY: ResourceConfigurationDictionary = {
  products: {
    fields: [
      {
        path: 'Name',
        header: 'ADMIN.HEADERS.NAME',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Active',
        header: 'ADMIN.HEADERS.ACTIVE',
        type: BASIC_STRING,
        sortable: false,
      },
    ],
    imgPath: PRODUCT_IMAGE_PATH_STRATEGY,
  },
  kitproducts: {
    fields: [
      {
        path: 'Product.Name',
        header: 'ADMIN.HEADERS.NAME',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'Product.ID',
        header: 'ADMIN.HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: PRODUCT_IMAGE_PATH_STRATEGY,
  },
  suppliers: {
    fields: [
      {
        path: 'Name',
        header: 'ADMIN.HEADERS.NAME',
        type: STRING_WITH_IMAGE,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Active',
        header: 'ADMIN.HEADERS.ACTIVE',
        type: BASIC_STRING,
        sortable: false,
      },
    ],
    imgPath: SUPPLIER_LOGO_PATH_STRATEGY,
  },
  users: {
    fields: [
      {
        path: 'Username',
        header: 'ADMIN.HEADERS.USERNAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Impersonate',
        header: '',
        type: IMPERSONATE_BUTTON,
        sortable: false,
      },
    ],
    imgPath: '',
  },
  promotions: {
    fields: [
      {
        path: 'Code',
        header: 'ADMIN.HEADERS.CODE',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Description',
        header: 'ADMIN.HEADERS.DESCRIPTION',
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
        header: 'ADMIN.HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
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
        header: 'ADMIN.HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
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
        header: 'ADMIN.HEADERS.ADDRESS_NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
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
        header: 'ADMIN.HEADERS.CARDHOLDER_NAME',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'CardType',
        header: 'ADMIN.HEADERS.CARD_TYPE',
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
        header: 'ADMIN.HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
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
        header: 'ADMIN.HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
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
        header: 'ADMIN.HEADERS.NAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Description',
        header: 'ADMIN.HEADERS.DESCRIPTION',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
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
        header: 'ADMIN.HEADERS.FROM_USER_USERNAME',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'LineItemCount',
        header: 'ADMIN.HEADERS.NUMBER_OF_LINE_ITEMS',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'Total',
        header: 'ADMIN.HEADERS.TOTAL_AMOUNT',
        type: CURRENCY,
        sortable: true,
      },
      {
        path: 'DateSubmitted',
        header: 'ADMIN.HEADERS.TIME_SUBMITTED',
        type: DATE_TIME,
        sortable: true,
      },
      {
        path: 'Status',
        header: 'ADMIN.HEADERS.STATUS',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Comments',
        header: 'ADMIN.HEADERS.COMMENTS',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'xp.OrderReturnInfo.HasReturn',
        header: 'ADMIN.HEADERS.HAS_CLAIMS',
        type: BOOLEAN,
        sortable: false,
        queryRestriction: 'OrderDirection=Incoming',
      },
      {
        path: 'xp.OrderReturnInfo.Comment',
        header: 'ADMIN.HEADERS.RETURN_COMMENT',
        type: BASIC_STRING,
        sortable: false,
        queryRestriction: 'OrderDirection=Incoming',
      },
    ],
    imgPath: '',
  },
  storefronts: {
    fields: [
      {
        path: 'AppName',
        header: 'Name',
        type: BASIC_STRING,
        sortable: false,
      },
      {
        path: 'ID',
        header: 'ADMIN.HEADERS.ID',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
  },
  logs: {
    fields: [
      {
        path: 'timeStamp',
        header: 'ADMIN.HEADERS.TIME_STAMP',
        type: DATE_TIME,
        sortable: true,
      },
      {
        path: 'Action',
        header: 'ADMIN.HEADERS.ACTION',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'RecordType',
        header: 'ADMIN.HEADERS.RECORD_TYPE',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'RecordId',
        header: 'ADMIN.HEADERS.RECORD_ID',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Level',
        header: 'ADMIN.HEADERS.RESULT',
        type: BASIC_STRING,
        sortable: true,
      },
      {
        path: 'Copy',
        header: 'ADMIN.HEADERS.COPY',
        type: COPY_OBJECT,
        sortable: false,
      },
    ],
    imgPath: '',
  },
  templates: {
    fields: [
      {
        path: 'Name',
        header: 'Name',
        type: BASIC_STRING,
        sortable: true,
      },
    ],
    imgPath: '',
  },
};
