export interface CountryDefinition {
    label: string
    abbreviation: string
}

export interface StateDefinition {
    label: string
    abbreviation: string
    country: string
}

export interface MerchantDefinition {
    cardConnectMerchantID: string
    currency: string
}

export interface RouteConfig {
    routerCall: string
    displayText: string
    url: string
    showInDropdown: boolean
    // no roles with access means all users will see
    rolesWithAccess?: string[]
    context?: string
  }