import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { ApiRole } from 'ordercloud-javascript-sdk'

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
