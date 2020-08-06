import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { adminClientAuth, authAdminBrowser } from '../api-utils.ts/auth-util'
import testConfig from '../testConfig'
import {
	ApiRole,
	Configuration,
	SdkConfiguration,
} from 'ordercloud-javascript-sdk'
import { axiosSetup } from './axios-helper'
import { createUser, deleteUser } from '../api-utils.ts/users-util'
import { saveUserAssignment } from '../api-utils.ts/usergroups-helper'
import { t } from 'testcafe'

export async function adminClientSetup() {
	await axiosSetup()

	setStagingUrl()

	const adminClientToken = await adminClientAuth(
		testConfig.automationClientID,
		testConfig.automationClientSecret,
		adminRoles
	)

	return adminClientToken
}

const adminRoles: ApiRole[] = [
	'SupplierAdmin',
	'BuyerUserAdmin',
	'UserGroupAdmin',
	'SupplierUserReader',
	'SupplierUserAdmin',
	'SupplierAddressReader',
	'SupplierAddressAdmin',
]

export function setStagingUrl() {
	const config: SdkConfiguration = {
		baseApiUrl: 'https://stagingapi.ordercloud.io/v1',
		baseAuthUrl: 'https://stagingapi.ordercloud.io/oauth/token',
	}
	Configuration.Set(config)
}

export async function loginTestSetup(authToken: string) {
	await t.maximizeWindow()
	const user: OrderCloudSDK.User = await createUser(authToken, '0005')
	await saveUserAssignment(user.ID, '0005-0001', '0005', authToken)
	return user
}

export async function loginTestCleanup(
	userID: string,
	buyerID: string,
	authToken: string
) {
	await deleteUser(userID, buyerID, authToken)
}

export async function adminTestSetup() {
	await t.maximizeWindow()

	const user: Partial<OrderCloudSDK.User> = {
		Username: testConfig.adminSellerUsername,
		Password: testConfig.adminSellerPassword,
	}

	await authAdminBrowser(user)

	await t.navigateTo(`${testConfig.adminAppUrl}home`)
}
