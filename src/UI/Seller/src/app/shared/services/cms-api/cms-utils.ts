import axios, { AxiosRequestConfig } from 'axios'

interface AccessToken {
  access_token?: string
  expires_in?: number
  token_type?: string
  refresh_token?: string
}

interface SdkConfiguration {
  /**
   * the path that will be used to talk to the ordercloud api.
   * It may be useful to change this to interact with different
   * environments or different versions of the api. At the time of writing
   * there is only one version of the api.
   */
  baseApiUrl?: string

  /**
   * the path that will be used to authenticate into ordercloud api.
   * It may be useful to change this to interact with different
   * environments or different versions of the api. At the time of writing
   * there is only one version of the api.
   */
  baseAuthUrl?: string

  /**
   * when set is used to call the refresh token endpoint to obtain a new access
   * token when exired (provided a refresh token is set in the sdk)
   * this functionality is only intended for users that interact
   * with at most one client per sdk instance
   */
  clientID?: string

  /**
   * specifies the number of milliseconds before the request times out.
   * If the request takes longer than `timeoutInMilliseconds`, the request will be aborted.
   * Default timeout is 10,000 milliseconds or 10 seconds
   */
  timeoutInMilliseconds?: number

  cookieOptions?: CookieOptions
}

export interface CookieOptions {
  /**
   * The cookie will be available only for this domain and its sub-domains
   */
  domain?: string

  /**
   * If true, then the cookie will only be available through a
   * secured connection (generally https)
   */
  secure?: boolean

  /**
   * Defines protocol for how cookies should be sent
   * in first or third party contexts https://adzerk.com/blog/chrome-samesite/
   */
  samesite?: 'none' | 'lax' | 'strict'
}

interface DecodedToken {
  /**
   * the ordercloud username
   */
  usr: string

  /**
   * the client id used when making token request
   */
  cid: string

  /**
   * helpful for identifying user types in an app
   * that may have more than one
   */
  usrtype: 'admin' | 'buyer' | 'supplier'

  /**
   * list of security profile roles that this user
   * has access to, read more about security profile roles
   * [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */
  role: Array<SecurityProfile['Roles']>

  /**
   * the issuer of the token - should always be https://auth.ordercloud.io
   */
  iss: string

  /**
   * the audience - who should be consuming this token
   * this should always be https://marketplace-api-qa.azurewebsites.net (the ordercloud api)
   */
  aud: string

  /**
   * expiration of the token (in seconds) since the
   * UNIX epoch (January 1, 1970 00:00:00 UTC)
   */
  exp: number

  /**
   * point at which token was issued (in seconds) since the
   * UNIX epoch (January 1, 1970 00:00:00 UTC)
   */
  nbf: number

  /**
   * the order id assigned to the anonymous user,
   * this value will *only* exist for anonymous users
   */
  orderid?: string
}

interface SecurityProfile {
  ID?: string
  Name: string
  Roles?:
    | 'DevCenter'
    | 'DevCenterPasswordReset'
    | 'DevCenterValidateEmail'
    | 'GrantForAnyRole'
    | 'ApiClientAdmin'
    | 'ApiClientReader'
    | 'AddressAdmin'
    | 'AddressReader'
    | 'AdminAddressAdmin'
    | 'AdminAddressReader'
    | 'AdminUserAdmin'
    | 'AdminUserGroupAdmin'
    | 'AdminUserGroupReader'
    | 'AdminUserReader'
    | 'ApprovalRuleAdmin'
    | 'ApprovalRuleReader'
    | 'BuyerAdmin'
    | 'BuyerImpersonation'
    | 'BuyerReader'
    | 'BuyerUserAdmin'
    | 'BuyerUserReader'
    | 'CatalogAdmin'
    | 'CatalogReader'
    | 'CategoryAdmin'
    | 'CategoryReader'
    | 'CostCenterAdmin'
    | 'CostCenterReader'
    | 'CreditCardAdmin'
    | 'CreditCardReader'
    | 'FullAccess'
    | 'IncrementorAdmin'
    | 'IncrementorReader'
    | 'InventoryAdmin'
    | 'MeAddressAdmin'
    | 'MeAdmin'
    | 'MeCreditCardAdmin'
    | 'MessageConfigAssignmentAdmin'
    | 'MeXpAdmin'
    | 'OrderAdmin'
    | 'OrderReader'
    | 'OverrideShipping'
    | 'OverrideTax'
    | 'OverrideUnitPrice'
    | 'PasswordReset'
    | 'PriceScheduleAdmin'
    | 'PriceScheduleReader'
    | 'ProductAdmin'
    | 'ProductAssignmentAdmin'
    | 'ProductFacetAdmin'
    | 'ProductFacetReader'
    | 'ProductReader'
    | 'PromotionAdmin'
    | 'PromotionReader'
    | 'SecurityProfileAdmin'
    | 'SecurityProfileReader'
    | 'SetSecurityProfile'
    | 'ShipmentAdmin'
    | 'ShipmentReader'
    | 'Shopper'
    | 'SpendingAccountAdmin'
    | 'SpendingAccountReader'
    | 'SupplierAddressAdmin'
    | 'SupplierAddressReader'
    | 'SupplierAdmin'
    | 'SupplierReader'
    | 'SupplierUserAdmin'
    | 'SupplierUserGroupAdmin'
    | 'SupplierUserGroupReader'
    | 'SupplierUserReader'
    | 'UnsubmittedOrderReader'
    | 'UserGroupAdmin'
    | 'UserGroupReader'
    | 'OpenIDConnectReader'
    | 'OpenIDConnectAdmin'
    | 'MessageSenderReader'
    | 'MessageSenderAdmin'
    | 'XpIndexAdmin'
    | 'WebhookReader'
    | 'WebhookAdmin'
  CustomRoles?: string[]
  PasswordConfig?: PasswordConfig
}

interface PasswordConfig {
  ExpireInDays?: number
}

class CmsConfiguration {
  private config: SdkConfiguration = {
    baseApiUrl: 'https://ordercloud-cms-test.azurewebsites.net',
    baseAuthUrl: 'https://auth.ordercloud.io/oauth/token',
    timeoutInMilliseconds: 120 * 1000,
    clientID: null,
    cookieOptions: {
      samesite: 'lax', // browser default
      secure: false,
      domain: null,
    },
  }

  /**
   * @ignore
   * not part of public api, don't include in generated docs
   */
  constructor() {
    this.Set = this.Set.bind(this)
    this.Get = this.Get.bind(this)
  }

  Set(config: SdkConfiguration): void {
    this.config = { ...this.config, ...config }
  }

  Get(): SdkConfiguration {
    return this.config
  }
}

export const CMSConfiguration = new CmsConfiguration()

class CookieService {
  constructor() {
    this.get = this.get.bind(this)
    this.set = this.set.bind(this)
    this.buildCookieString = this.buildCookieString.bind(this)
    this.remove = this.remove.bind(this)
  }

  public get(cookieName: string): string {
    const rows = document.cookie.split(';')
    for (const row of rows) {
      const [key, val] = row.split('=')
      const cookieKey = decodeURIComponent(key.trim().toLowerCase())
      if (cookieKey === cookieName.toLowerCase()) {
        return decodeURIComponent(val)
      }
    }
    return ''
  }

  public set(cookieName: string, cookieVal: string): void {
    document.cookie = this.buildCookieString(cookieName, cookieVal)
  }

  public remove(cookieName: string): void {
    document.cookie = this.buildCookieString(cookieName, undefined)
  }

  private buildCookieString(name: string, value?: string) {
    const options = CMSConfiguration.Get().cookieOptions || {}
    let expires
    if (!value) {
      expires = new Date('Thu, 01 Jan 1970 00:00:00 GMT')
      value = ''
    } else {
      // set expiration of cookie longer than token
      // so we can parse clientid from token to perform refresh when token has expired
      expires = new Date()
      expires.setFullYear(expires.getFullYear() + 1)
    }

    let str = encodeURIComponent(name) + '=' + encodeURIComponent(value)
    str += options.domain ? ';domain=' + options.domain : ''
    str += expires ? ';expires=' + expires.toUTCString() : ''
    str += options.secure ? ';secure' : ''
    str += options.samesite ? ';samesite=' + options.samesite : ''

    return str
  }
}

const Cookies = new CookieService()

export class TokenService {
  private accessTokenCookieName = `ordercloud.access-token`
  private impersonationTokenCookieName = 'ordercloud.impersonation-token'
  private refreshTokenCookieName = 'ordercloud.refresh-token'

  private accessToken?: string = null
  private impersonationToken?: string = null
  private refreshToken?: string = null
  private isNode = new Function(
    'try {return this===global;}catch(e){return false;}'
  )

  /**
   * @ignore
   * not part of public api, don't include in generated docs
   */
  constructor() {
    this.GetAccessToken = this.GetAccessToken.bind(this)
    this.GetImpersonationToken = this.GetImpersonationToken.bind(this)
    this.GetRefreshToken = this.GetRefreshToken.bind(this)
    this.RemoveAccessToken = this.RemoveAccessToken.bind(this)
    this.RemoveImpersonationToken = this.RemoveImpersonationToken.bind(this)
    this.SetAccessToken = this.SetAccessToken.bind(this)
    this.RemoveRefreshToken = this.RemoveRefreshToken.bind(this)
    this.SetImpersonationToken = this.SetImpersonationToken.bind(this)
    this.SetRefreshToken = this.SetRefreshToken.bind(this)
  }

  /**
   * Manage Access Tokens
   */

  public GetAccessToken(): string {
    return this.isNode()
      ? this.accessToken
      : Cookies.get(this.accessTokenCookieName)
  }

  public SetAccessToken(token: string): void {
    parseJwt(token) // check if token is valid
    this.isNode()
      ? (this.accessToken = token)
      : Cookies.set(this.accessTokenCookieName, token)
  }

  public RemoveAccessToken(): void {
    this.isNode()
      ? (this.accessToken = '')
      : Cookies.remove(this.accessTokenCookieName)
  }

  /**
   * Manage Impersonation Tokens
   */

  public GetImpersonationToken(): string {
    return this.isNode()
      ? this.impersonationToken
      : Cookies.get(this.impersonationTokenCookieName)
  }

  public SetImpersonationToken(token: string): void {
    parseJwt(token) // check if token is valid
    this.isNode()
      ? (this.impersonationToken = token)
      : Cookies.set(this.impersonationTokenCookieName, token)
  }

  public RemoveImpersonationToken(): void {
    this.isNode()
      ? (this.impersonationToken = null)
      : Cookies.remove(this.impersonationTokenCookieName)
  }

  /**
   * Manage Refresh Tokens
   */

  public GetRefreshToken(): string {
    return this.isNode()
      ? this.refreshToken
      : Cookies.get(this.refreshTokenCookieName)
  }

  public SetRefreshToken(token: string): void {
    this.isNode()
      ? (this.refreshToken = token)
      : Cookies.set(this.refreshTokenCookieName, token)
  }

  public RemoveRefreshToken(): void {
    this.isNode()
      ? (this.refreshToken = null)
      : Cookies.remove(this.refreshTokenCookieName)
  }
}

/**
 * @ignore
 * not part of public api, don't include in generated docs
 */
class HttpClient {
  private _auth = new Auth()
  private _token = new TokenService()

  constructor() {
    // create a new instance so we avoid clashes with any
    // configurations done on default axios instance that
    // a consumer of this SDK might use
    if (typeof axios === 'undefined') {
      throw new Error(
        'Ordercloud is missing required peer dependency axios. This must be installed and loaded before the OrderCloud SDK'
      )
    }

    this.get = this.get.bind(this)
    this.put = this.put.bind(this)
    this.post = this.post.bind(this)
    this.patch = this.patch.bind(this)
    this.delete = this.delete.bind(this)
    this._buildRequestConfig = this._buildRequestConfig.bind(this)
    this._getToken = this._getToken.bind(this)
    this._isTokenExpired = this._isTokenExpired.bind(this)
    this._tokenInterceptor = this._tokenInterceptor.bind(this)
    this._tryRefreshToken = this._tryRefreshToken.bind(this)
  }

  public get = async (
    path: string,
    config?: AxiosRequestConfig
  ): Promise<any> => {
    const requestConfig = await this._buildRequestConfig(config)
    const response = await axios.get(
      `${CMSConfiguration.Get().baseApiUrl}${path}`,
      requestConfig
    )
    return response.data
  }

  public post = async (
    path: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<any> => {
    const requestConfig = await this._buildRequestConfig(config)
    const response = await axios.post(
      `${CMSConfiguration.Get().baseApiUrl}${path}`,
      data,
      requestConfig
    )
    return response.data
  }

  public put = async (
    path: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<any> => {
    const requestConfig = await this._buildRequestConfig(config)
    const response = await axios.put(
      `${CMSConfiguration.Get().baseApiUrl}${path}`,
      data,
      requestConfig
    )
    return response.data
  }

  public patch = async (
    path: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<any> => {
    const requestConfig = await this._buildRequestConfig(config)
    const response = await axios.patch(
      `${CMSConfiguration.Get().baseApiUrl}${path}`,
      data,
      requestConfig
    )
    return response.data
  }

  public delete = async (path: string, config: AxiosRequestConfig) => {
    const requestConfig = await this._buildRequestConfig(config)
    const response = await axios.delete(
      `${CMSConfiguration.Get().baseApiUrl}${path}`,
      requestConfig
    )
    return response.data
  }

  // sets the token on every outgoing request, will attempt to
  // refresh the token if the token is expired and there is a refresh token set
  private async _tokenInterceptor(
    config: AxiosRequestConfig
  ): Promise<AxiosRequestConfig> {
    let token = this._getToken(config)
    if (this._isTokenExpired(token)) {
      token = await this._tryRefreshToken(token)
    }
    config.headers.Authorization = `Bearer ${token}`
    return config
  }

  private _getToken(config: AxiosRequestConfig): string {
    let token
    if (config.params.accessToken) {
      token = config.params.accessToken
    } else if (config.params.impersonating) {
      token = this._token.GetImpersonationToken()
    } else {
      token = this._token.GetAccessToken()
    }

    // strip out axios params that we'vee hijacked for our own nefarious purposes
    delete config.params.accessToken
    delete config.params.impersonating
    return token
  }

  private _isTokenExpired(token: string): boolean {
    if (!token) {
      return true
    }
    const decodedToken = parseJwt(token)
    const currentSeconds = Date.now() / 1000
    const currentSecondsWithBuffer = currentSeconds - 10
    return decodedToken.exp < currentSecondsWithBuffer
  }

  private async _tryRefreshToken(accessToken: string): Promise<string> {
    const refreshToken = this._token.GetRefreshToken()
    if (!refreshToken) {
      return accessToken || ''
    }
    const sdkConfig = CMSConfiguration.Get()
    if (!accessToken && !sdkConfig.clientID) {
      return accessToken || ''
    }
    let clientID
    if (accessToken) {
      const decodedToken = parseJwt(accessToken)
      clientID = decodedToken.cid
    }
    if (sdkConfig.clientID) {
      clientID = sdkConfig.clientID
    }
    const refreshRequest = await this._auth.RefreshToken(refreshToken, clientID)
    return refreshRequest.access_token
  }

  private _buildRequestConfig(
    config?: AxiosRequestConfig
  ): Promise<AxiosRequestConfig> {
    const sdkConfig = CMSConfiguration.Get()
    const requestConfig = {
      ...config,
      paramSerializer: ParamSerializer,
      timeout: sdkConfig.timeoutInMilliseconds,
      headers: {
        'Content-Type': 'application/json',
      },
    }
    return this._tokenInterceptor(requestConfig)
  }
}

declare const Buffer
/**
 * @ignore
 * not part of public api, don't include in generated docs
 */
function decodeBase64(str) {
  // atob is defined on the browser, in node we must use buffer
  if (typeof atob !== 'undefined') {
    return atob(str)
  }
  return Buffer.from(str, 'base64').toString('binary')
}

/**
 * @ignore
 * not part of public api, don't include in generated docs
 */
function parseJwt(token: string): DecodedToken {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      decodeBase64(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch (e) {
    throw new Error('Invalid token')
  }
}

/**
 * @ignore
 * not part of public api, don't include in generated docs
 */
function ParamSerializer(params: { [key: string]: any }): string {
  const valuesArray: string[] = []

  // serialize filters first, they are handled specially
  if (params.filters) {
    const filters = flattenFiltersObject(params.filters)
    for (const key in filters) {
      const filterVal = filters[key]
      if (Array.isArray(filterVal)) {
        filterVal.forEach((val) =>
          valuesArray.push(`${key}=${encodeURIComponent(val)}`)
        )
      } else if (filterVal) {
        valuesArray.push(`${key}=${encodeURIComponent(filterVal)}`)
      }
    }
    delete params.filters
  }

  // serialize the rest of the params
  for (const key in params) {
    const val = params[key]
    if (val) {
      valuesArray.push(`${key}=${encodeURIComponent(val)}`)
    }
  }

  return valuesArray.length ? `${valuesArray.join('&')}` : ''
}

/**
 * @ignore
 * not part of public api, don't include in generated docs
 *
 * build a flattened filters object  where each key is the dot-referenced property
 * to filter and the value is the value to filter by
 * this ultimately gets sent to ordercloud as a query param
 */
function flattenFiltersObject(filters) {
  const result = {}
  for (const key in filters) {
    inspectProp(filters[key], key, result)
  }
  return result
}

/**
 * @ignore
 * not part of public api, don't include in generated docs
 */
function inspectProp(propVal, propName, result) {
  const isObject = Object.prototype.toString.call(propVal) === '[object Object]'
  if (isObject) {
    for (const key in propVal) {
      inspectProp(propVal[key], `${propName}.${key}`, result)
    }
  } else {
    if (propVal === null) {
      throw new Error(
        `Null is not a valid filter prop. Use negative filter "!" combined with wildcard filter "*" to define a filter for the absence of a value. \nex: an order list call with { xp: { hasPaid: '!*' } } would return a list of orders where xp.hasPaid is null or undefined\nhttps://ordercloud.io/features/advanced-querying#filtering`
      )
    }
    result[propName] = propVal
  }
}

class Auth {
  constructor() {
    // create a new instance so we avoid clashes with any
    // configurations done on default axios instance that
    // a consumer of this SDK might use
    if (typeof axios === 'undefined') {
      throw new Error(
        'Ordercloud is missing required peer dependency axios. This must be installed and loaded before the OrderCloud SDK'
      )
    }

    /**
     * @ignore
     * not part of public api, don't include in generated docs
     */
    this.Anonymous = this.Anonymous.bind(this)
    this.ClientCredentials = this.ClientCredentials.bind(this)
    this.ElevatedLogin = this.ElevatedLogin.bind(this)
    this.Login = this.Login.bind(this)
    this.RefreshToken = this.RefreshToken.bind(this)
  }

  /**
   * @description this workflow is most appropriate for client apps where user is a human, ie a registered user
   *
   * @param username of the user logging in
   * @param password of the user logging in
   * @param client_id of the application the user is logging into
   * @param scope roles being requested - space delimited string or array
   */
  public async Login(
    username: string,
    password: string,
    clientID: string,
    scope: Array<SecurityProfile['Roles']>
  ): Promise<AccessToken> {
    const body = {
      grant_type: 'password',
      username,
      password,
      client_id: clientID,
      scope: scope.join(' '),
    }
    const configuration = CMSConfiguration.Get()
    const response = await axios.post(
      configuration.baseAuthUrl,
      ParamSerializer(body),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          Accept: 'application/json',
        },
      }
    )
    return response.data
  }

  /**
   * @description similar to login except client secret is also required, adding another level of security
   *
   * @param clientSecret of the application
   * @param username of the user logging in
   * @param password of the user logging in
   * @param clientID of the application the user is logging into
   * @param scope roles being requested - space delimited string or array
   * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
   * @param reportProgress flag to report request and response progress.
   */
  public async ElevatedLogin(
    clientSecret: string,
    username: string,
    password: string,
    clientID: string,
    scope: Array<SecurityProfile['Roles']>
  ): Promise<AccessToken> {
    const body = {
      grant_type: 'password',
      scope: scope.join(' '),
      client_id: clientID,
      username,
      password,
      client_secret: clientSecret,
    }
    const configuration = CMSConfiguration.Get()
    const response = await axios.post(
      configuration.baseAuthUrl,
      ParamSerializer(body),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          Accept: 'application/json',
        },
      }
    )
    return response.data
  }

  /**
   * @description this workflow is best suited for a backend system
   *
   * @param clientSecret of the application
   * @param clientID of the application the user is logging into
   * @param scope roles being requested - space delimited string or array
   * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
   * @param reportProgress flag to report request and response progress.
   */
  public async ClientCredentials(
    clientSecret: string,
    clientID: string,
    scope: Array<SecurityProfile['Roles']>
  ): Promise<AccessToken> {
    const body = {
      grant_type: 'client_credentials',
      scope: scope.join(' '),
      client_id: clientID,
      client_secret: clientSecret,
    }
    const configuration = CMSConfiguration.Get()
    const response = await axios.post(
      configuration.baseAuthUrl,
      ParamSerializer(body),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          Accept: 'application/json',
        },
      }
    )
    return response.data
  }

  /**
   * @description extend your users' session by getting a new access token with a refresh token. refresh tokens must be enabled in the dashboard
   *
   * @param refreshToken of the application
   * @param clientID of the application the user is logging into
   * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
   * @param reportProgress flag to report request and response progress.
   */
  public async RefreshToken(
    refreshToken: string,
    clientID: string
  ): Promise<AccessToken> {
    const body = {
      grant_type: 'refresh_token',
      client_id: clientID,
      refresh_token: refreshToken,
    }
    const configuration = CMSConfiguration.Get()
    const response = await axios.post(
      configuration.baseAuthUrl,
      ParamSerializer(body),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          Accept: 'application/json',
        },
      }
    )
    return response.data
  }

  /**
   * @description allow users to browse your catalog without signing in - must have anonymous template user set in dashboard
   *
   * @param clientID of the application the user is logging into
   * @param scope roles being requested - space delimited string or array
   * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
   * @param reportProgress flag to report request and response progress.
   */
  public async Anonymous(
    clientID: string,
    scope: Array<SecurityProfile['Roles']>
  ): Promise<AccessToken> {
    const body = {
      grant_type: 'client_credentials',
      client_id: clientID,
      scope: scope.join(' '),
    }
    const configuration = CMSConfiguration.Get()
    const response = await axios.post(
      configuration.baseAuthUrl,
      ParamSerializer(body),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          Accept: 'application/json',
        },
      }
    )
    return response.data
  }
}

export const httpClient = new HttpClient()
