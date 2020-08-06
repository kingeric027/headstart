import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { ApiRole } from 'ordercloud-javascript-sdk'
import testConfig from '../testConfig'
import { ClientFunction, t } from 'testcafe'

export async function adminClientAuth(
	clientID: string,
	clientSecret: string,
	scope: ApiRole[]
) {
	const response = await OrderCloudSDK.Auth.ClientCredentials(
		clientSecret,
		clientID,
		scope
	)

	const token = response['access_token']

	return token
}

export async function userClientAuth(
	clientID: string,
	username: string,
	password: string,
	scope: ApiRole[]
) {
	const response = await OrderCloudSDK.Auth.Login(
		username,
		password,
		clientID,
		scope
	)

	const token = response['access_token']

	return token
}

export async function authAdminBrowser(
	user: Partial<OrderCloudSDK.User>
) {
	const userToken = await userClientAuth(
		testConfig.adminAppClientID,
		user.Username,
		user.Password,
		adminUserRoles
	)

	await setBrowserAuthCookie(userToken, 'marketplace_admin_test.token')
	//Below cookie is set on the browser when logging in, but does not seem to be needed
	await setBrowserAuthCookie(userToken, 'ordercloud.access-token')

	t.ctx.userAuth = userToken
}

export async function setBrowserAuthCookie(token: string, tokenName: string) {
	const setCookieFunction = ClientFunction((tokenName, token) => {
		document.cookie = `${tokenName}=${token}`
	})

	await setCookieFunction(tokenName, token)
}


export const adminUserRoles: ApiRole[] = [
    'AdminAddressReader',
    'BuyerUserAdmin',
    'UserGroupAdmin',
    'ProductAdmin',
    'PriceScheduleAdmin',
    'SupplierReader',
    'BuyerAdmin',
    'OrderAdmin',
    'AddressAdmin',
    'CategoryAdmin',
    'CatalogAdmin',
    'PromotionAdmin',
    'ApprovalRuleAdmin',
    'CreditCardAdmin',
    'SupplierAdmin',
    'SupplierUserAdmin',
    'SupplierUserGroupAdmin',
    'SupplierAddressAdmin',
    'AdminUserAdmin',
    'ProductFacetAdmin',
	'ShipmentAdmin',
	// @ts-ignore
	'MPProductAdmin',
	// @ts-ignore
	'MPPromotionAdmin',
	// @ts-ignore
	'MPCategoryAdmin',
	// @ts-ignore
	'MPOrderAdmin',
	// @ts-ignore
	'MPShipmentAdmin',
	// @ts-ignore
	'MPBuyerAdmin',
	// @ts-ignore
	'MPSellerAdmin',
	// @ts-ignore
	'MPSupplierAdmin',
	// @ts-ignore
    'MPSupplierUserGroupAdmin'
  ]
