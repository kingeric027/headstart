import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { adminClientAuth } from '../api-utils.ts/auth-util'
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
		testConfig.adminClientID,
		testConfig.adminClientSecret,
		adminRoles
	)

	return adminClientToken
}

const adminRoles: ApiRole[] = ['BuyerUserAdmin', 'UserGroupAdmin']

export function setStagingUrl() {
	const config: SdkConfiguration = {
		baseApiUrl: 'https://stagingapi.ordercloud.io/v1',
		baseAuthUrl: 'https://stagingauth.ordercloud.io/oauth/token',
	}
	Configuration.Set(config)
}

export async function loginTestSetup(authToken: string) {
	await t.maximizeWindow()
	const user: OrderCloudSDK.User = await createUser(authToken, '0007')
	await saveUserAssignment(user.ID, '0007-0001', '0007', authToken)
	return user
}

export async function loginTestCleanup(
	userID: string,
	buyerID: string,
	authToken: string
) {
	await deleteUser(userID, buyerID, authToken)
}
