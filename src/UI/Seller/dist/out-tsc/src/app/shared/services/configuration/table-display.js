import { PRODUCT_IMAGE_PATH_STRATEGY } from '../product/product-image.helper';
export var SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY = {
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
export var STRING_WITH_IMAGE = 'STRING_WITH_IMAGE';
export var BASIC_STRING = 'BASIC_STRING';
export var DATE_TIME = 'DATE_TIME';
export var CURRENCY = 'CURRENCY';
export var FULL_TABLE_RESOURCE_DICTIONARY = {
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
                path: 'ID',
                header: 'ID',
                type: BASIC_STRING,
            },
            {
                path: 'LineItemCount',
                header: '# of Line Items',
                type: BASIC_STRING,
            },
            {
                path: 'Total',
                header: 'Total Amount (USD)',
                type: CURRENCY,
            },
            {
                path: 'DateSubmitted',
                header: 'Time Submitted',
                type: DATE_TIME,
            },
            {
                path: 'Status',
                header: 'Status',
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
//# sourceMappingURL=table-display.js.map