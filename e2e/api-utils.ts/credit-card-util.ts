import { HeadStartSDK } from '@ordercloud/headstart-sdk'

export async function createCreditCard(
	authToken: string,
	firstName: string,
	lastName: string
) {
	//no clue if this is the right route or not
	await HeadStartSDK.MePayments.Post
}
